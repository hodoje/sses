using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    internal static class ClientAppConfig
    {

        public const string MasterCardServiceAddressName = "MasterCardServiceAddress";
        public const string TransactionServiceAddressName = "TransactionServiceAddress";
        public const string ServiceCertificateCNName = "ServiceCertificateCN";
        public const string CertificatePathName = "CertificatePath";

        static ClientAppConfig()
        {
            MasterCardServiceAddress = ConfigurationManager.AppSettings.Get(MasterCardServiceAddressName);
            TransactionServiceAddress = ConfigurationManager.AppSettings.Get(TransactionServiceAddressName);
            ServiceCertificateCN = ConfigurationManager.AppSettings.Get(ServiceCertificateCNName);
            CertificatePath = ConfigurationManager.AppSettings.Get(CertificatePathName);
        }

        public static string MasterCardServiceAddress { get; private set; }
        public static string TransactionServiceAddress { get; private set; }
        public static string ServiceCertificateCN { get; private set; }
        public static string CertificatePath { get; private set; }



    }
}
