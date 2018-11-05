using Common.CertificateManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServiceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var issuer = CertificateManager.Instance.GetPrivateCertificateFromFile(
                @"C:\Users\srki1\source\repos\CertMakerTesting\bin64\certs\TestCA.pfx",
                "password");

            var certPath =
                CertificateManager.Instance.CreateAndStoreNewClientCertificate("CN=newCertificate", "pass", issuer);
            Console.WriteLine(certPath);
            Console.ReadLine();
        }
    }
}
