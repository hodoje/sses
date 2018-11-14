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

namespace BankServiceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Thread.CurrentPrincipal.IsInRole("BankServices"))
            {
                Console.WriteLine("Only user in BankServices role can run BankServiceApp.");
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
