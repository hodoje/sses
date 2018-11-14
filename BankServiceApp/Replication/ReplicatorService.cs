using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using BankServiceApp.AccountStorage;
using BankServiceApp.Arbitration;
using Common;
using Common.UserData;

namespace BankServiceApp.Replication
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    public class ReplicatorService : IReplicator
    {
        private readonly IArbitrationServiceProvider _arbitrationServiceProvider;
        private readonly ICache _bankCache;

        public ReplicatorService()
        {
            _arbitrationServiceProvider = ServiceLocator.GetInstance<IArbitrationServiceProvider>();
            _bankCache = ServiceLocator.GetInstance<ICache>();
        }

        #region IReplicator

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "BankServices")]
        public void ReplicateData(IReplicationItem replicationData)
        {
            var principal = Thread.CurrentPrincipal;

            if ((replicationData.Type & ReplicationType.UserData) != 0)
            {
                ReplicateCacheData(replicationData, principal);
            }

            if ((replicationData.Type & ReplicationType.CertificateData) != 0)
            {
                if ((replicationData.Type & ReplicationType.RevokeCertificate) == 0)
                {
                    RevokeOldAndPlaceNewCertificate(replicationData.Certificate);
                }
                else
                {
                    RevokeOldCertificate(replicationData.Certificate);
                }
            }
        }

        private static void RevokeOldCertificate(X509Certificate2 certificate)
        {
            Console.WriteLine($"Replicating certificate data for {certificate.Subject}");
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.MaxAllowed);
                var foundCert = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, certificate.SubjectName, true)?[0];

                if (foundCert != null)
                {
                    store.Remove(certificate);
                }

                store.Close();
            }

        }

        private static void RevokeOldAndPlaceNewCertificate(X509Certificate2 certificate)
        {
            Console.WriteLine($"Replicating certificate data for {certificate.Subject}");
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.MaxAllowed);
                var foundCert = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, certificate.SubjectName, true)?[0];

                if (foundCert == null)
                {
                    store.Add(certificate);
                }
                else if (foundCert.SerialNumber != certificate.SerialNumber)
                {
                    store.Remove(foundCert);
                    store.Add(certificate);
                }

                store.Close();
            }
        }

        /// <summary>
        /// Replicate client account cache data.
        /// </summary>
        /// <param name="replicationData"> Replication data. </param>
        /// <param name="principal"> The bank service user replicating data. </param>
        private void ReplicateCacheData(IReplicationItem replicationData, System.Security.Principal.IPrincipal principal)
        {
            Console.WriteLine($"Replicating cache data from {principal.Identity.Name}");
            var client = BankCache.GetClientFromCache(_bankCache, replicationData.Client.Name);
            client.Pin = replicationData.Client.Pin;
            client.Account = new Account(replicationData.Client.Account.Balance);
            _bankCache.StoreData();
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "BankServices")]
        public ServiceState CheckState()
        {
            return _arbitrationServiceProvider.State;
        }

        #endregion
    }
}
