using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAuditServiceApp
{
    internal static class BankAuditServiceConfig
    {
        private const string LogNameConfigName = "LogName";
        private const string BankAuditServiceAddressConfigName = "BankAuditServiceAddress";
        private const string BankAuditServiceEndpointNameConfigName = "BankAuditServiceEndpointName";

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
