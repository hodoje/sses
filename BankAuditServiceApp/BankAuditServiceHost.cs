using System;
using System.Net.Security;
using System.ServiceModel;
using BankServiceApp.ServiceHosts;
using Common.ServiceContracts;

namespace BankAuditServiceApp
{
    public class BankAuditServiceHost : IServiceHost, IDisposable
    {
        private readonly ServiceHost _bankAuditServiceHost;
        private string _bankAuditServiceAddress;
        private string _bankAuditServiceEndpointName;
        private NetTcpBinding _binding;

        public BankAuditServiceHost()
        {
            _bankAuditServiceAddress = BankAuditServiceConfig.BankAuditServiceAddress;
            _bankAuditServiceEndpointName = BankAuditServiceConfig.BankAuditServiceEndpointName;
            _binding = SetUpBinding();
            _bankAuditServiceHost = new ServiceHost(typeof(BankAuditService));
            _bankAuditServiceHost.AddServiceEndpoint(typeof(IBankAuditService), _binding, $"{_bankAuditServiceAddress}/{_bankAuditServiceEndpointName}");
        }

        private NetTcpBinding SetUpBinding()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            return binding;
        }

        public void OpenService()
        {
            try
            {
                _bankAuditServiceHost.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BankAuditServiceHost failed to open with an error: {ex.Message}");
            }
        }

        public void CloseService()
        {
            try
            {
                _bankAuditServiceHost.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BankAuditServiceHost failed to close with an error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            (_bankAuditServiceHost as IDisposable).Dispose();
        }
    }
}
