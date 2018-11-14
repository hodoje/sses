using Common.CertificateManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BankServiceApp.AccountStorage;
using BankServiceApp.Arbitration;
using BankServiceApp.Replication;
using Common;
using System.DirectoryServices.AccountManagement;
using Common.ServiceContracts;

namespace BankServiceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            if (!principal.IsInRole("BankServices"))
            {
                Console.WriteLine($"Only user in BankServices role can run {nameof(BankServiceApp)}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
                return;
            }

            using (ICache bankCache = new BankCache())
            {
                ServiceLocator.RegisterService(bankCache);
                using (IArbitrationServiceProvider arbitrationService = new ArbitrationServiceProvider())
                {
                    ServiceLocator.RegisterService(arbitrationService);
                    using (ReplicatorProxy replicatorProxy = new ReplicatorProxy())
                    {
                        ProxyPool.RegisterProxy<IReplicator>(replicatorProxy);

                        using (BankAuditServiceProxy auditProxy = new BankAuditServiceProxy())
                        {
                            ProxyPool.RegisterProxy<IBankAuditService>(auditProxy);

                            if (CertificateManager.Instance.GetCACertificate() == null)
                            {
                                var caCertificate = CertificateManager.Instance.GetPrivateCertificateFromFile(
                                    BankAppConfig.CACertificatePath,
                                    BankAppConfig.CACertificatePass);
                                CertificateManager.Instance.SetCACertificate(caCertificate);
                            }

                            arbitrationService.RegisterService(new BankServicesHost());
                            arbitrationService.OpenServices();
                            Console.ReadLine();
                        }
                    }
                }
            }
        }
    }
}
