using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common.EventLogData;
using Common.ServiceContracts;

namespace BankServiceApp
{
    public class BankAuditServiceProxy : IBankAuditService, IDisposable
    {
        private readonly ChannelFactory<IBankAuditService> _channelFactory;
        private IBankAuditService _auditProxy;

        public BankAuditServiceProxy()
        {
            var binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            EndpointAddress address = new EndpointAddress(BankAppConfig.BankAuditServiceEndpoint);

            _channelFactory = new ChannelFactory<IBankAuditService>(binding, address);
        }

        public void Log(EventLogData eventLogData)
        {
            try
            {
                if (_auditProxy == null)
                {
                    _auditProxy = _channelFactory.CreateChannel();
                }
                _auditProxy.Log(eventLogData);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error logging data on audit service: {e.Message}");
            }
        }

        public void Dispose()
        {
            (_channelFactory as IDisposable).Dispose();
        }
    }
}
