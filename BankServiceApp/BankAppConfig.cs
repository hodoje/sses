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
        }

        public static string ReplicatorName { get; private set; }

        public static string MasterCardServiceName { get; private set; }

        public static string TransactionServiceName { get; private set; }

        public static int InstanceNo { get; private set; }

        public static List<string> Endpoints { get; private set; }
    }
}
