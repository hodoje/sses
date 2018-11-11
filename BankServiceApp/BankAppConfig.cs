using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServiceApp
{
    internal static class BankAppConfig
    {
        public const string ServiceAddressConfigName = "Address";
        public const string InstanceNumberConfigName = "Instances";
        public const string ReplicatorConfigName = "ReplicationEndpointName";
        public const string MasterCardServiceConfigName = "MasterCardServiceEndpointName";
        public const string TransactionServiceConfigName = "TransactionServiceEndpointName";
        public const string BankAuditServiceAddressConfigName = "BankAuditServiceAddress";
        public const string BankTransactionServiceCertificatePathConfigName = "ServiceCertificatePath";
        public const string BankTransactionServiceCertificateSubjectNameConfigName = "ServiceCertificateSubjectName";
        public const string BankTransactionServiceCertificatePasswordConfigName = "ServiceCertificatePass";
        public const string CACertificatePathConfigName = "CACertificatePath";
        public const string CACertificatePassConfigName = "CACertificatePass";
        public const string BankCachePathConfigName = "BankCachePath";
        static BankAppConfig()
        {
            ReplicatorName = ConfigurationManager.AppSettings.Get(ReplicatorConfigName);
            MasterCardServiceName = ConfigurationManager.AppSettings.Get(MasterCardServiceConfigName);
            TransactionServiceName = ConfigurationManager.AppSettings.Get(TransactionServiceConfigName);

            InstanceNo = int.Parse(ConfigurationManager.AppSettings.Get(InstanceNumberConfigName));
            Endpoints = new List<string>(InstanceNo);

            for (int i = 0; i < InstanceNo; i++)
            {
                Endpoints.Add(ConfigurationManager.AppSettings.Get($"{ServiceAddressConfigName}{i}"));
            }

            BankAuditServiceEndpoint = ConfigurationManager.AppSettings.Get(BankAuditServiceAddressConfigName);
            BankTransactionServiceCertificatePath =
                ConfigurationManager.AppSettings.Get(BankTransactionServiceCertificatePathConfigName);
            BankTransactionServiceSubjectName =
                ConfigurationManager.AppSettings.Get(BankTransactionServiceCertificateSubjectNameConfigName);
            BankTransactionServiceCertificatePassword =
                ConfigurationManager.AppSettings.Get(BankTransactionServiceCertificatePasswordConfigName);
            CACertificatePath = ConfigurationManager.AppSettings.Get(CACertificatePathConfigName);
            CACertificatePass = ConfigurationManager.AppSettings.Get(CACertificatePassConfigName);
            BankCachePath = ConfigurationManager.AppSettings.Get(BankCachePathConfigName);
        }

        public static string ReplicatorName { get; private set; }

        public static string MasterCardServiceName { get; private set; }

        public static string TransactionServiceName { get; private set; }

        public static int InstanceNo { get; private set; }

        public static List<string> Endpoints { get; private set; }

        public static string BankAuditServiceEndpoint { get; private set; }

        public static string BankTransactionServiceCertificatePath { get; private set; }

        public static string BankTransactionServiceSubjectName { get; private set; }

        public static string BankTransactionServiceCertificatePassword { get; private set; }

        public static string CACertificatePath { get; private set; }

        public static string CACertificatePass { get; private set; }

        public static string BankCachePath { get; private set; }

        public static string MyAddress { get; set; } = null;
    }
}
