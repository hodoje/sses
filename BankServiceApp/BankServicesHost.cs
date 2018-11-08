using BankServiceApp.BankServices;
using Common.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BankServiceApp.ServiceHosts;
using Common.CertificateManager;
using System.Security.Cryptography.X509Certificates;

namespace BankServiceApp
{
    public class BankServicesHost : IServiceHost, IDisposable
    {
        private ServiceHost cardHost = null;
        private string cardAddress = String.Empty;
        private string srvCerCn = String.Empty;
        private NetTcpBinding binding = new NetTcpBinding();

        public BankServicesHost()
        {
            SetUpBinding();
            cardHost = new ServiceHost(typeof(BankMasterCardService));
            cardHost.AddServiceEndpoint(typeof(IBankMasterCardService), binding, cardAddress);
           // cardHost.Credentials.ServiceCertificate.Certificate = CertificateManager.Instance.GetCertificateFromStore(StoreLocation.LocalMachine, StoreName.My, srvCerCn);
        }
        private void ReadConfig()
        {
            cardAddress = BankAppConfig.Endpoints[0] + BankAppConfig.MasterCardServiceName;
            srvCerCn = BankAppConfig.ServiceCertCN;
        }
            
        private void SetUpBinding()
        {
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel =
            System.Net.Security.ProtectionLevel.EncryptAndSign;

            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
        }

        private void CardServiceOpen()
        {
            try
            {
                cardHost.Open();
            }
            catch (Exception ex)
            {

                Console.WriteLine("cardHost failed to open with an error: {0}",ex.Message);
            }
        }

        private void CardServiceClose()
        {
            try
            {
                cardHost.Close();
            }
            catch (Exception ex)
            {

                Console.WriteLine("cardHost failed to close with an error: {0}", ex.Message);
            }
        }

        private void TransationServiceOpen()
        {
            try
            {
                //transationHost.Open();
            }
            catch (Exception ex)
            {

                Console.WriteLine("transationHost failed to open with an error: {0}", ex.Message);
            }
        }

        private void TransationServiceClose()
        {
            try
            {
                //transationHost.Open();
            }
            catch (Exception ex)
            {

                Console.WriteLine("transationHost failed to close with an error: {0}", ex.Message);
            }
        }

        public void OpenService()
        {
            CardServiceOpen();
            Console.WriteLine("CardServiceHost is opened..");
            TransationServiceOpen();
            Console.WriteLine("TransationServiceHost is opened..");
        }

        public void CloseService()
        {
            TransationServiceClose();
            CardServiceClose();
        }

        public void Dispose()
        {
            (cardHost as IDisposable).Dispose();
        }
    }
}
