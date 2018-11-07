using Common.CertificateManager;
using Common.DataEncapsulation;
using Common.ServiceContracts;
using Common.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class ClientProxy : IBankMasterCardService, IBankTransactionService
    {
        private NetTcpBinding binding = new NetTcpBinding();

        private IBankTransactionService transactionServiceFactory = null;
        private IBankMasterCardService cardServiceFactory = null;
        private string addressCardService = "net.tcp://localhost:9999/BankMasterCardService";
        private string addressTransationService = String.Empty;
        private EndpointAddress cardServiceEndpointAddress = null;
        private EndpointAddress transationServiceEndpointAddress = null;
        private X509Certificate2 servCert = null;
        private string srvCerCn = String.Empty;



        public ClientProxy()
        {
            SetUpBinding();
            //SetUpEndpointAddress();
            //transactionServiceFactory = ChannelFactory<IBankTransactionService>.CreateChannel(binding, transationServiceEndpointAddress);
            cardServiceFactory = ChannelFactory<IBankMasterCardService>.CreateChannel(binding, new EndpointAddress(addressCardService));

        }

    
        private void SetUpEndpointAddress()
        {
            servCert = CertificateManager.Instance.GetCertificateFromStore(StoreLocation.LocalMachine, StoreName.My, srvCerCn);
            cardServiceEndpointAddress = new EndpointAddress(
                new Uri(addressCardService),
                new X509CertificateEndpointIdentity(servCert));
            transationServiceEndpointAddress = new EndpointAddress(
                new Uri(addressTransationService),
                new X509CertificateEndpointIdentity(servCert));
        }
        private void SetUpBinding()
        {
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel =
            System.Net.Security.ProtectionLevel.EncryptAndSign;

            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
        }


        public bool ExecuteTransaction(ITransaction transaction)
        {
            bool Result = false;

            try
            {
                Result = transactionServiceFactory.ExecuteTransaction(transaction);
            }
            catch (FaultException ex)
            {

                Console.WriteLine("Error: {0}", ex.Message);
            }
            return Result;
        }

        public NewCardResults RequestNewCard()
        {
            NewCardResults newCardResults = new NewCardResults();
            try
            {
                newCardResults = cardServiceFactory.RequestNewCard();
            }
            catch (FaultException ex)
            {

                Console.WriteLine("Error: {0}",ex.Message);
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
    }
}
