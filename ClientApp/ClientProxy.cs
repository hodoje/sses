using Common.CertificateManager;
using Common.DataEncapsulation;
using Common.ServiceContracts;
using Common.Transaction;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        private NetTcpBinding binding = new NetTcpBinding();

        private IBankTransactionService transactionServiceFactory = null;
        private IBankMasterCardService cardServiceFactory = null;
        private EndpointAddress cardServiceEndpointAddress = null;
        private EndpointAddress transationServiceEndpointAddress = null;
        private X509Certificate2 servCert = null;



        public ClientProxy()
        {
            
            SetUpBinding();
            //SetUpEndpointAddress();
            //transactionServiceFactory = ChannelFactory<IBankTransactionService>.CreateChannel(binding, transactionServiceEndpointAddress);
            var cardServiceFactory = new ChannelFactory<IBankMasterCardService>(binding, cardServiceEndpointAddress);
            cardServiceFactory.Credentials.Windows.ClientCredential.UserName = @"bankclient";
            cardServiceFactory.Credentials.Windows.ClientCredential.Password = "123";
            this.cardServiceFactory = cardServiceFactory.CreateChannel();
            this.cardServiceFactory.Login();
        }

     

        private void SetUpEndpointAddress()
        {
            servCert = CertificateManager.Instance.GetCertificateFromStore(StoreLocation.LocalMachine, StoreName.My, ClientAppConfig.CertificatePath);
            // Sta ce ti certifikat ovde koristis windows auth
            cardServiceEndpointAddress = new EndpointAddress(
                new Uri(ClientAppConfig.MasterCardServiceAddress));

            transationServiceEndpointAddress = new EndpointAddress(
                new Uri(ClientAppConfig.TransactionServiceAddress),
                new X509CertificateEndpointIdentity(servCert));
        }

        private void SetUpBinding()
        {
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel =
            System.Net.Security.ProtectionLevel.EncryptAndSign;

            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
        }


        public bool ExecuteTransaction(byte[] signiture,ITransaction transaction)
        {
            bool Result = false;

            try
            {
                Result = transactionServiceFactory.ExecuteTransaction(signiture,transaction);
            }
            catch (FaultException ex)
            {

                Console.WriteLine("Error: {0}", ex.Message);
            }
            return Result;
        }

        public decimal CheckBalance(byte[] signiture, ITransaction transaction)
        {
            decimal Result = 0;
            try
            {
                Result = transactionServiceFactory.CheckBalance(signiture, transaction);
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
                newCardResults = cardServiceFactory.RequestNewCard(password);
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
                Result = cardServiceFactory.RevokeExistingCard(pin);
            }
            catch (FaultException ex)
            {

                Console.WriteLine("Error: {0}", ex.Message);
            }

            return Result;
        }

        public NewCardResults RequestResetPin(string pin)
        {
            NewCardResults newCardResults = new NewCardResults();
            try
            {
                newCardResults = cardServiceFactory.RequestResetPin(pin);
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
                cardServiceFactory.Login();
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
