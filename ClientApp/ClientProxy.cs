using Common.CertificateManager;
using Common.DataEncapsulation;
using Common.ServiceContracts;
using Common.Transaction;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class ClientProxy : IBankMasterCardService, IBankTransactionService
    {
        private IBankMasterCardService _cardServiceProxy = null;
        private EndpointAddress _cardServiceEndpointAddress = null;

        private IBankTransactionService _transactionServiceProxy;
        private ChannelFactory<IBankTransactionService> _transactionServiceProxyFactory;
        private EndpointAddress _transactionServiceEndpointAddress = null;

        private X509Certificate2 _serverCertificate;

        public ClientProxy(string username,SecureString password)
        {
            SetUpEndpoints();
            var cardServiceFactory = new ChannelFactory<IBankMasterCardService>(SetUpWindowsAuthBinding(), _cardServiceEndpointAddress);
            cardServiceFactory.Credentials.Windows.ClientCredential.UserName = username;
            cardServiceFactory.Credentials.Windows.ClientCredential.SecurePassword = password;
            this._cardServiceProxy = cardServiceFactory.CreateChannel();
        }

        private void SetUpEndpoints()
        {
            _cardServiceEndpointAddress = new EndpointAddress(
                new Uri(ClientAppConfig.MasterCardServiceAddress));

            _serverCertificate = CertificateManager.Instance.GetCertificateFromStore(
                StoreLocation.LocalMachine, 
                StoreName.Root,
                ClientAppConfig.ServiceCertificateCN);

            _transactionServiceEndpointAddress = new EndpointAddress(
                new Uri(ClientAppConfig.TransactionServiceAddress),
                new X509CertificateEndpointIdentity(_serverCertificate));
        }

        public void OpenTransactionServiceProxy(X509Certificate2 clientCertificate)
        {
            _transactionServiceProxyFactory =
                new ChannelFactory<IBankTransactionService>(SetupCertAuthBinding(), _transactionServiceEndpointAddress);
            _transactionServiceProxyFactory.Credentials.ClientCertificate.Certificate = clientCertificate;
            _transactionServiceProxy = _transactionServiceProxyFactory.CreateChannel();
        }

        private NetTcpBinding SetUpWindowsAuthBinding()
        {
            var binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ProtectionLevel =
            System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            return binding;
        }

        private NetTcpBinding SetupCertAuthBinding()
        {
            var binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
            return binding;
        }

        public bool ExecuteTransaction(byte[] signature, ITransaction transaction)
        {
            bool Result = false;

            try
            {
                Result = _transactionServiceProxy.ExecuteTransaction(signature,transaction);
            }
            catch (FaultException ex)
            {

                Console.WriteLine("Error: {0}", ex.Message);
            }
            return Result;
        }

        public decimal CheckBalance(byte[] signature, ITransaction transaction)
        {
            decimal Result = 0;
            try
            {
                Result = _transactionServiceProxy.CheckBalance(signature, transaction);
            }
            catch (FaultException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
            return Result;
        }

        public NewCardResults RequestNewCard(string password)
        {
            NewCardResults newCardResults = new NewCardResults();
            try
            {
                newCardResults = _cardServiceProxy.RequestNewCard(password);
            }
            catch (FaultException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                Console.ReadLine();
            }

            return newCardResults;
        }

        public bool RevokeExistingCard(string pin)
        {
            bool Result = false;
            try
            {
                Result = _cardServiceProxy.RevokeExistingCard(pin);
            }
            catch (FaultException ex)
            {

                Console.WriteLine("Error: {0}", ex.Message);
            }

            return Result;
        }

        public NewCardResults RequestResetPin()
        {
            NewCardResults newCardResults = new NewCardResults();
            try
            {
                newCardResults = _cardServiceProxy.RequestResetPin();
            }
            catch (FaultException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }

            return newCardResults;
        }

        public void Login()
        {
            try
            {
                _cardServiceProxy.Login();
            }
            catch (SecurityAccessDeniedException ex)
            {
                Console.WriteLine($"User failed to login reason: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}
