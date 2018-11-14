using System.Configuration;

namespace BankAuditServiceApp
{
    internal static class BankAuditServiceConfig
    {
        public const string LogNameConfigName = "LogName";
        public const string BankAuditServiceAddressConfigName = "BankAuditServiceAddress";
        public const string BankAuditServiceEndpointNameConfigName = "BankAuditServiceEndpointName";

        public static string LogName { get; private set; }
        public static string BankAuditServiceAddress { get; private set; }
        public static string BankAuditServiceEndpointName { get; private set; }

        static BankAuditServiceConfig()
        {
            LogName = ConfigurationManager.AppSettings[LogNameConfigName];
            BankAuditServiceAddress = ConfigurationManager.AppSettings[BankAuditServiceAddressConfigName];
            BankAuditServiceEndpointName = ConfigurationManager.AppSettings[BankAuditServiceEndpointNameConfigName];
        }
    }
}
