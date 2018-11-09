﻿using BankServiceApp.BankServices;
using Common.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Security;
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
        private readonly ServiceHost _cardHost;
        private readonly ServiceHost _transactionServiceHost;
        private readonly X509Certificate2 _transactionServiceCertificate;

        public BankServicesHost()
        {
            #region MasterCardServiceSetup

            var masterCardHostBinding = SetupWindowsAuthBinding();
            var masterCardServiceEndpoint = $"{BankAppConfig.MyAddress}/{BankAppConfig.MasterCardServiceName}";

            _cardHost = new ServiceHost(typeof(BankMasterCardService));
            _cardHost.AddServiceEndpoint(typeof(IBankMasterCardService), masterCardHostBinding, masterCardServiceEndpoint);

            #endregion

            #region TransactionServiceSetup

            _transactionServiceCertificate = LoadServiceCertificate(BankAppConfig.BankTransactionServiceCertificatePath,
                BankAppConfig.BankTransactionServiceSubjectName,
                BankAppConfig.BankTransactionServiceCertificatePassword);

            var transactionHostBinding = SetupCertificateAuthBinding();
            var transactionServiceEndpoint = $"{BankAppConfig.MyAddress}/{BankAppConfig.TransactionServiceName}";
            _transactionServiceHost = new ServiceHost(typeof(BankTransactionService));
            _transactionServiceHost.AddServiceEndpoint(typeof(IBankTransactionService), transactionHostBinding,
                transactionServiceEndpoint);
            _transactionServiceHost.Credentials.ServiceCertificate.Certificate = _transactionServiceCertificate;

            #endregion
        }

        private NetTcpBinding SetupWindowsAuthBinding()
        {
            var binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ProtectionLevel =
            System.Net.Security.ProtectionLevel.EncryptAndSign;

            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            return binding;
        }

        private NetTcpBinding SetupCertificateAuthBinding()
        {
            var binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
            return binding;
        }

        private X509Certificate2 LoadServiceCertificate(string path, string name, string password)
        {
            X509Certificate2 certificate = null;
            try
            {
                certificate = CertificateManager.Instance.GetPrivateCertificateFromFile($"{path}{name}.pfx", password);
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to find service certificate creating new...");
                var issuer = CertificateManager.Instance.GetPrivateCertificateFromFile(BankAppConfig.CACertificatePath,
                    BankAppConfig.CACertificatePass);
                Console.WriteLine($"New certificate at: {CertificateManager.Instance.CreateNewCertificate(name, password, issuer)}");
                try
                {
                    certificate = CertificateManager.Instance.GetPrivateCertificateFromFile($"{path}{name}.pfx", password);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }

            return certificate;
        }

        private void CardServiceOpen()
        {
            try
            {
                _cardHost.Open();
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
                _cardHost.Close();
            }
            catch (Exception ex)
            {

                Console.WriteLine("cardHost failed to close with an error: {0}", ex.Message);
                throw;
            }
        }

        private void TransactionServiceOpen()
        {
            try
            {
                _transactionServiceHost.Open();
            }
            catch (Exception ex)
            {

                Console.WriteLine("transationHost failed to open with an error: {0}", ex.Message);
                throw;
            }
        }

        private void TransactionServiceClose()
        {
            try
            {
                _transactionServiceHost.Open();
            }
            catch (Exception ex)
            {

                Console.WriteLine("transationHost failed to close with an error: {0}", ex.Message);
                throw;
            }
        }

        #region IServiceHost Methods

        public void OpenService()
        {
            CardServiceOpen();
            Console.WriteLine("CardServiceHost is opened..");
            TransactionServiceOpen();
            Console.WriteLine("TransationServiceHost is opened..");
        }

        public void CloseService()
        {
            TransactionServiceClose();
            CardServiceClose();
        }

        #endregion

        #region IDisposable Methods

        public void Dispose()
        {
            (_cardHost as IDisposable).Dispose();
            (_transactionServiceHost as IDisposable).Dispose();
        }

        #endregion
    }
}
