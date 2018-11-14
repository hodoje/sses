using Common.CertificateManager;
using Common.DataEncapsulation;
using Common.ServiceContracts;
using System;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.ServiceModel;
using System.Threading;
using BankServiceApp.AccountStorage;
using BankServiceApp.Arbitration;
using Common;
using Common.UserData;

namespace BankServiceApp.BankServices
{
    public class BankMasterCardService : IBankMasterCardService
    {
        private readonly ICache _bankCache;
        private readonly IArbitrationServiceProvider _arbitrationServiceProvider;

        public BankMasterCardService()
        {
            _bankCache = ServiceLocator.GetInstance<ICache>();
            _arbitrationServiceProvider = ServiceLocator.GetInstance<IArbitrationServiceProvider>();

            var caCertificate = CertificateManager.Instance.GetCACertificate();
            if (caCertificate == null)
            {
                throw new Exception("Certificate manager returned null for CA certificate.");
            }
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

                RevokeCertificate(clientName);

                var CACertificate = CertificateManager.Instance.GetCACertificate();
                CertificateManager.Instance.CreateAndStoreNewCertificate(
                    clientName, 
                    password, 
                    CACertificate,
                    BankAppConfig.BankTransactionServiceCertificatePath);

                var resultData = new NewCardResults()
                {
                    PinCode = GenerateRandomPin()
                };

                var client = BankCache.GetClientFromCache(_bankCache, clientName);
                client.ResetPin(null, resultData.PinCode);

                _bankCache.StoreData();

                return resultData;
            }
            catch(ArgumentNullException ane)
            {
                throw new FaultException<CustomServiceException>(new CustomServiceException(ane.Message + "was null!"),
                    $"{ane.Message} was null!");
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

                    return RevokeCertificate(clientName);
                }
                else
                {
                    throw new SecurityException("Invalid pin.");
                }
            }
            catch (ArgumentNullException ane)
            {
                throw new FaultException<CustomServiceException>(new CustomServiceException(ane.Message + "was null!"),
                    $"{ane.Message} was null!");
            }
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
                var results = new NewCardResults() {PinCode = GenerateRandomPin()};

                client.ResetPin(null, results.PinCode);

                return results;

            }
            catch (ArgumentNullException ane)
            {
                throw new FaultException<CustomServiceException>(new CustomServiceException(ane.Message + "was null!"),
                    $"{ane.Message} was null!");
            }
        }

        public void Login()
        {
            var principal = Thread.CurrentPrincipal;
            if (!principal.IsInRole("Clients"))
            {
                // Audit failed login
                throw new SecurityException("User isn't in Clients role.");
            }
            else
            {
                // Audit success login
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

    }
}
