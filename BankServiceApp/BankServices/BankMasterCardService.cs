using Common.CertificateManager;
using Common.DataEncapsulation;
using Common.ServiceContracts;
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.ServiceModel;
using System.Threading;
using BankServiceApp.AccountStorage;
using BankServiceApp.Arbitration;
using Common;
using Common.UserData;
using System.Threading.Tasks;
using BankServiceApp.Replication;

namespace BankServiceApp.BankServices
{
    public class BankMasterCardService : IBankMasterCardService
    {
        private readonly ICache _bankCache;
        private readonly IArbitrationServiceProvider _arbitrationServiceProvider;
        private readonly IReplicator _replicatorProxy;
        private readonly string _applicationName = BankAppConfig.BankName;//System.AppDomain.CurrentDomain.FriendlyName;
        
        public BankMasterCardService()
        {
            var caCertificate = CertificateManager.Instance.GetCACertificate();
            if (caCertificate == null)
            {
                throw new Exception("Certificate manager returned null for CA certificate.");
            }

            _bankCache = ServiceLocator.GetInstance<ICache>();
            _arbitrationServiceProvider = ServiceLocator.GetInstance<IArbitrationServiceProvider>();
            _replicatorProxy = ProxyPool.GetProxy<IReplicator>();
        }

        #region IBankMasterCardService Methods

        public NewCardResults RequestNewCard(string password)
        {
            if (!Thread.CurrentPrincipal.IsInRole("Clients"))
            {
                throw new SecurityException("Principal isn't part of Clients role.");
            }

            try
            {
                var clientName = ExtractUsernameFromFullName(Thread.CurrentPrincipal.Identity.Name);

                Console.WriteLine($"Client {clientName} requested new card.");

                Task.Run(() =>
                {
                    ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                    _applicationName,
                    clientName,
                    "Request for new card!",
                    System.Diagnostics.EventLogEntryType.Information));
                });

                RevokeCertificate(clientName);

                var CACertificate = CertificateManager.Instance.GetCACertificate();
                CertificateManager.Instance.CreateAndStoreNewCertificate(
                    clientName, 
                    password, 
                    CACertificate,
                    BankAppConfig.BankTransactionServiceCertificatePath);

                var cert = CertificateManager.Instance.GetCertificateFromStore(
                    StoreLocation.LocalMachine,
                    StoreName.My,
                    clientName);

                if (cert == null)
                {
                    throw new ArgumentNullException(nameof(cert));
                }

                var resultData = new NewCardResults()
                {
                    PinCode = GenerateRandomPin()
                };

                var client = BankCache.GetClientFromCache(_bankCache, clientName);
                client.ResetPin(null, resultData.PinCode);

                Task.Run(() =>
                {
                    ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                    _applicationName,
                    clientName,
                    "Successfully created a card!",
                    System.Diagnostics.EventLogEntryType.Information));
                });

                _bankCache.StoreData();
                
                var replicationData = new ReplicationItem(
                    client, 
                    ReplicationType.UserData | ReplicationType.CertificateData, 
                    cert);
                _replicatorProxy.ReplicateData(replicationData);


                return resultData;
            }
            catch(ArgumentNullException ane)
            {
                Task.Run(() =>
                {
                    ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                        _applicationName,
                        Thread.CurrentPrincipal.Identity.Name,
                        ane.Message,
                        System.Diagnostics.EventLogEntryType.Error));
                });

                throw new FaultException<CustomServiceException>(new CustomServiceException(ane.Message + "was null!"));
            }
        }

        public bool RevokeExistingCard(string pin)
        {
            if (!Thread.CurrentPrincipal.IsInRole("Clients"))
            {
                throw new SecurityException("Client isn't in required role.");
            }

            try
            {
                // Check if client exists
                var clientName = ExtractUsernameFromFullName(Thread.CurrentPrincipal.Identity.Name);
                var client = default(IClient);
                client = BankCache.GetClientFromCache(_bankCache,clientName);

                if (client == null)
                    return false;

                // if he exists in the system, authorize him
                if (client.CheckPin(pin))
                {
                    Console.WriteLine($"Client {clientName} requested card revocation.");
                    Task.Run(() =>
                    {
                        ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                                _applicationName,
                                clientName,
                                "Requested card revocation.",
                                System.Diagnostics.EventLogEntryType.Information));
                    });

                    var cert = CertificateManager.Instance.GetCertificateFromStore(
                        StoreLocation.LocalMachine,
                        StoreName.My, 
                        clientName);

                    if (cert == null)
                    {
                        return false;
                    }

                    bool revoked = RevokeCertificate(clientName);

                    if (revoked)
                    {
                        Task.Run(() =>
                        {
                            ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                                _applicationName,
                                clientName,
                                "Successfully revoked the card.",
                                System.Diagnostics.EventLogEntryType.Information));
                        });

                        var replicationData = new ReplicationItem(
                            null, 
                            ReplicationType.CertificateData | ReplicationType.RevokeCertificate, 
                            cert);
                        _replicatorProxy.ReplicateData(replicationData);
                    }

                    return revoked;
                }
                else
                {
                    Task.Run(() =>
                    {
                        ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                                _applicationName,
                                clientName,
                                "Invalid pin.",
                                System.Diagnostics.EventLogEntryType.Error));
                    });

                    throw new SecurityException("Invalid pin.");
                }
            }
            catch (ArgumentNullException ane)
            {
                Task.Run(() =>
                {
                    ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                            _applicationName,
                            Thread.CurrentPrincipal.Identity.Name,
                            ane.Message,
                            System.Diagnostics.EventLogEntryType.Error));
                });

                throw;
            }
        }

        public NewCardResults RequestResetPin()
        {
            if (!Thread.CurrentPrincipal.IsInRole("Clients"))
            {
                throw new SecurityException("Client isn't in required role.");
            }

            string clientName = ExtractUsernameFromFullName(Thread.CurrentPrincipal.Identity.Name);
            try
            {
                var client = BankCache.GetClientFromCache(_bankCache, clientName);

                Console.WriteLine("Client requested pin reset.");

                Task.Run(() =>
                {
                    ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                                _applicationName,
                                clientName,
                                "Requested pin reset.",
                                System.Diagnostics.EventLogEntryType.Information));
                });

                var results = new NewCardResults() {PinCode = GenerateRandomPin()};

                client.ResetPin(null, results.PinCode);
                Task.Run(() =>
                {
                    ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                            _applicationName,
                            clientName,
                            "New pin generated.",
                            System.Diagnostics.EventLogEntryType.Information));
                });

                _replicatorProxy.ReplicateData(new ReplicationItem(client));

                return results;

            }
            catch (ArgumentNullException ane)
            {
                Task.Run(() =>
                {
                    ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                            _applicationName,
                            clientName,
                            ane.Message,
                            System.Diagnostics.EventLogEntryType.Error));
                });

                throw new ArgumentNullException(ane.Message);
            }
        }

        public bool ExtendCard(string password)
        {
            var clientName = ExtractUsernameFromFullName(Thread.CurrentPrincipal.Identity.Name);

            if (!Thread.CurrentPrincipal.IsInRole("Clients"))
            {
                Task.Run(() =>
                {
                    ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                        _applicationName,
                        clientName,
                        $"({nameof(BankServiceApp)}) [BankMasterCardService] Client isn't part of Clients group.",
                        System.Diagnostics.EventLogEntryType.FailureAudit));
                });
                throw new SecurityException("Principal isn't part of Clients role.");
            }

            try
            {
                Console.WriteLine($"Client {clientName} requested extension.");

                Task.Run(() =>
                {
                    ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                    _applicationName,
                    clientName,
                    "Requested extension.",
                    System.Diagnostics.EventLogEntryType.Information));
                });

                RevokeCertificate(clientName);

                var CACertificate = CertificateManager.Instance.GetCACertificate();
                CertificateManager.Instance.CreateAndStoreNewCertificate(
                    clientName,
                    password,
                    CACertificate,
                    BankAppConfig.BankTransactionServiceCertificatePath);

                var newCert = CertificateManager.Instance.GetCertificateFromStore(
                    StoreLocation.LocalMachine,
                    StoreName.My,
                    clientName);

                if (newCert == null)
                    return false;

                Task.Run(() =>
                {
                    ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                    _applicationName,
                    clientName,
                    "Successfully extended the card.",
                    System.Diagnostics.EventLogEntryType.Information));
                });

                var replicationItem = new ReplicationItem(null, ReplicationType.CertificateData, newCert);
                _replicatorProxy.ReplicateData(replicationItem);

                return true;
            }
            catch (ArgumentNullException ane)
            {
                Task.Run(() =>
                {
                    ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                        _applicationName,
                        Thread.CurrentPrincipal.Identity.Name,
                        ane.Message,
                        System.Diagnostics.EventLogEntryType.Error));
                });

                throw new FaultException<CustomServiceException>(new CustomServiceException(ane.Message + "was null!"),
                    $"{ane.Message} was null!");
            }
        }

        public void Login()
        {
            var principal = Thread.CurrentPrincipal;
            if (!principal.IsInRole("Clients"))
            {
                Task.Run(() =>
                {
                    ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                            _applicationName,
                            Thread.CurrentPrincipal.Identity.Name,
                            "User isn't in Clients role.",
                            System.Diagnostics.EventLogEntryType.Error));
                });

                throw new SecurityException("User isn't in Clients role.");
            }
            else
            {
                Task.Run(() =>
                {
                    ProxyPool.GetProxy<IBankAuditService>().Log(new Common.EventLogData.EventLogData(
                            _applicationName,
                            Thread.CurrentPrincipal.Identity.Name,
                            "Successfully logged in.",
                            System.Diagnostics.EventLogEntryType.Information));
                });
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "Clients")]
        public ServiceState CheckState()
        {
            return _arbitrationServiceProvider.State;
        }

        #endregion

        private string GenerateRandomPin()
        {
            string newPin = "";

            Random randomValues = new Random((int)DateTime.Now.Ticks);
            
            for (int i = 0; i < 4; ++i)
                newPin += randomValues.Next(0, 9).ToString();

            return newPin;
        }

        private string ExtractUsernameFromFullName(string fullName)
        {
            var index = fullName.LastIndexOf("\\");
            return fullName.Substring(index + 1, fullName.Length - index - 1);
        }

        private static bool RevokeCertificate(string clientName)
        {
            var success = false;

            var subject = $"CN={clientName}";

            using (X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.MaxAllowed);
                // Returns all certificates containing substring with subject name
                //Eg. clientname, clientname2, clientname3 are returned when just clientname is queried
                var certificates = store.Certificates.Find(X509FindType.FindBySubjectName, clientName, true);

                foreach (var certificate in certificates)
                {
                    if (subject.Equals(certificate.Subject))
                    {
                        store.Remove(certificate);
                        success = true;
                    }
                }

                store.Close();
            }

            return success;
        }
    }
}
