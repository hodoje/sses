using System;
using System.Diagnostics;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Threading.Tasks;
using BankServiceApp.AccountStorage;
using BankServiceApp.Replication;
using Common;
using Common.CertificateManager;
using Common.EventLogData;
using Common.ServiceContracts;
using Common.Transaction;
using Common.UserData;
using System.Timers;

namespace BankServiceApp.BankServices
{
    public class BankTransactionService : IBankTransactionService
    {
        private static readonly double timeInterval = 60.0;
        private static readonly int withdrawLimitForAudit = 5;

        private System.Timers.Timer checkingTimer = new System.Timers.Timer(timeInterval);

        private readonly string _applicationName = BankAppConfig.BankName; //System.AppDomain.CurrentDomain.FriendlyName;

        private readonly ICache _bankCache;

        private readonly IReplicator _replicatorProxy;

        private void CheckingTimerLogic(object sender, ElapsedEventArgs e)
        {
            foreach (var client in _bankCache.GetAllClients())
            {
                if (client.Withdraw >= withdrawLimitForAudit)
                {
                    Task.Run(() =>
                    {
                        ProxyPool.GetProxy<BankAuditServiceProxy>().Log(new Common.EventLogData.EventLogData(
                            System.AppDomain.CurrentDomain.FriendlyName,
                            client.Name,
                            $"{client.Withdraw} transactions made in past {timeInterval} seconds.",
                            System.Diagnostics.EventLogEntryType.Warning));
                    });
                }
                client.Withdraw = 0;
            }
        }

        public BankTransactionService()
        {
            _bankCache = ServiceLocator.GetInstance<ICache>();
            _replicatorProxy = ProxyPool.GetProxy<IReplicator>();
            checkingTimer.Elapsed += new System.Timers.ElapsedEventHandler(CheckingTimerLogic);
        }

        public decimal CheckBalance(byte[] signature, ITransaction transaction)
        {
            var clientName = GetClientNameFromAuthContext(ServiceSecurityContext.Current.AuthorizationContext);

            var hash = HashData(transaction);
            VerifySignature(signature, hash, clientName);

            var client = BankCache.GetClientFromCache(_bankCache, clientName);

            Task.Run(() =>
            {
                ProxyPool.GetProxy<IBankAuditService>().Log(new EventLogData(
                    _applicationName,
                    clientName,
                    "Requested check balance.",
                    EventLogEntryType.Information));
            });

            if (client != null)
            {
                if (!client.CheckPin(transaction.Pin))
                {
                    Task.Run(() =>
                    {
                        ProxyPool.GetProxy<IBankAuditService>().Log(new EventLogData(
                            _applicationName,
                            clientName,
                            "Invalid Pin.",
                            EventLogEntryType.Error));
                    });

                    throw new SecurityException("Invalid Pin.");
                }
            }
            else
            {
                Task.Run(() =>
                {
                    ProxyPool.GetProxy<IBankAuditService>().Log(new EventLogData(
                        _applicationName,
                        clientName,
                        "Client not found in cache. Possibly using old certificate.",
                        EventLogEntryType.Error));
                });

                throw new FaultException("Client not found in cache. Possibly using old certificate.");
            }

            return client.Account.Balance;
        }

        public bool ExecuteTransaction(byte[] signature, ITransaction transaction)
        {
            var clientName = GetClientNameFromAuthContext(ServiceSecurityContext.Current.AuthorizationContext);

            var hash = HashData(transaction);
            VerifySignature(signature, hash, clientName);

            var client = BankCache.GetClientFromCache(_bankCache, clientName);

            if (client != null)
                if (!client.CheckPin(transaction.Pin))
                {
                    Task.Run(() =>
                    {
                        ProxyPool.GetProxy<IBankAuditService>().Log(new EventLogData(
                            _applicationName,
                            clientName,
                            "Invalid Pin.",
                            EventLogEntryType.Error));
                    });

                    throw new SecurityException("Invalid Pin.");
                }

            var success = false;

            switch (transaction.TransactionType)
            {
                case TransactionType.Deposit:
                    client.Account.Deposit(transaction.Amount);
                    success = true;

                    Task.Run(() =>
                    {
                        ProxyPool.GetProxy<BankAuditServiceProxy>().Log(new Common.EventLogData.EventLogData(
                        _applicationName,
                        clientName,
                        $"Deposit made with {transaction.Amount} amount.",
                        System.Diagnostics.EventLogEntryType.Information));
                    });

                    break;
                case TransactionType.Withdrawal:
                    if (client.Account.Balance >= transaction.Amount)
                    {
                        client.Account.Withdraw(transaction.Amount);
                        ++client.Withdraw;
                        success = true;
                        

                        Task.Run(() =>
                        {
                            ProxyPool.GetProxy<BankAuditServiceProxy>().Log(new Common.EventLogData.EventLogData(
                            _applicationName,
                            clientName,
                            $"Withdrawal made with {transaction.Amount} amount.",
                            System.Diagnostics.EventLogEntryType.Information));
                        });
                    }

                    break;
                default:
                    throw new InvalidOperationException("Invalid operation. For check balance use dedicated method.");
            }

            if (success)
            {
                _bankCache.StoreData();
                _replicatorProxy.ReplicateData(new ReplicationItem(client));
            }

            return success;
        }

        private static string GetClientNameFromAuthContext(AuthorizationContext context)
        {
            var subjectCert =
                (context.ClaimSets[0] as X509CertificateClaimSet)?
                .X509Certificate;

            var subjectNameCN = subjectCert?.Subject.Split('=')[1];
            return subjectNameCN;
        }

        private void VerifySignature(byte[] signature, byte[] hash, string clientName)
        {
            var localCert =
                CertificateManager.Instance.GetCertificateFromStore(StoreLocation.LocalMachine, StoreName.My,
                    clientName);

            if (!localCert.GetRSAPublicKey()
                .VerifyHash(hash, signature, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1))
            {
                Task.Run(() =>
                {
                    ProxyPool.GetProxy<IBankAuditService>().Log(new EventLogData(
                        _applicationName,
                        clientName,
                        "Invalid transaction signature.",
                        EventLogEntryType.Error));
                });

                throw new SecurityException("Invalid transaction signature.");
            }
        }

        private byte[] HashData<T>(T data)
        {
            byte[] hash;
            using (var hasAlg = new SHA512Cng())
            {
                using (var stream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, data);

                    hash = hasAlg.ComputeHash(stream);
                }
            }

            return hash;
        }
    }
}