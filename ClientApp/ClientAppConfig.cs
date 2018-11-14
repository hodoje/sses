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
        public const string ServiceAddressConfigName = "Address";
        public const string InstanceNumberConfigName = "Instances";
        public const string MasterCardServiceConfigName = "MasterCardServiceEndpointName";
        public const string TransactionServiceConfigName = "TransactionServiceEndpointName";
        public const string ServiceCertificateCNName = "ServiceCertificateCN";
        public const string CertificatePathName = "CertificatePath";

        static ClientAppConfig()
        {
            MasterCardServiceName = ConfigurationManager.AppSettings.Get(MasterCardServiceConfigName);
            TransactionServiceName = ConfigurationManager.AppSettings.Get(TransactionServiceConfigName);
            ServiceCertificateCN = ConfigurationManager.AppSettings.Get(ServiceCertificateCNName);
            CertificatePath = ConfigurationManager.AppSettings.Get(CertificatePathName);
            InstanceNo = int.Parse(ConfigurationManager.AppSettings.Get(InstanceNumberConfigName));
            Endpoints = new List<string>(InstanceNo);
            for (int i = 0; i < InstanceNo; i++)
            {
                Endpoints.Add(ConfigurationManager.AppSettings.Get($"{ServiceAddressConfigName}{i}"));
            }
            MasterCardServiceAddress = new List<string>(InstanceNo);
            TransactionServiceAddress = new List<string>(InstanceNo);
            for (int i = 0; i < InstanceNo; i++)
            {
                MasterCardServiceAddress.Add($"{Endpoints[i]}/{MasterCardServiceName}");
                TransactionServiceAddress.Add($"{Endpoints[i]}/{TransactionServiceName}");
            }

        }

        public static List<string> MasterCardServiceAddress { get; private set; }
        public static List<string> TransactionServiceAddress { get; private set; }
        public static string ServiceCertificateCN { get; private set; }
        public static string CertificatePath { get; private set; }

        public static int InstanceNo { get; private set; }
        public static List<string> Endpoints { get; private set; }
        public static string MasterCardServiceName { get; private set; }

        public static string TransactionServiceName { get; private set; }

    }
}
