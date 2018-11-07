using BankServiceApp.BankServices;
using Common.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BankServiceApp.ServiceHosts;

namespace BankServiceApp
{
    public class BankServicesHost : IServiceHost, IDisposable
    {
        private ServiceHost cardHost = null;
        private string cardAddress = String.Empty;

        private NetTcpBinding binding = new NetTcpBinding();

        public BankServicesHost()
        {
            InitBinding();
            cardHost = new ServiceHost(typeof(BankMasterCardService));
            cardHost.AddServiceEndpoint(typeof(IBankMasterCardService), binding, cardAddress);
        }

        private void InitBinding()
        {
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel =
            System.Net.Security.ProtectionLevel.EncryptAndSign;

            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
        }

        private void CardOpen()
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

        private void CardClose()
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

        private void TransationOpen()
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

        private void TransationClose()
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
            CardOpen();
            TransationOpen();
        }

        public void CloseService()
        {
            TransationClose();
            CardClose();
        }

        public void Dispose()
        {
            (cardHost as IDisposable).Dispose();
        }
    }
}
