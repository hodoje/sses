using Common.CertificateManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BankServiceApp.Arbitration;
using BankServiceApp.Replication;
using Common;

namespace BankServiceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (IArbitrationServiceProvider arbitrationService = new ArbitrationServiceProvider())
            {
                ServiceLocator.RegisterService(arbitrationService);
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
