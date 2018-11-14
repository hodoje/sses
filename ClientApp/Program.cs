using Common.CertificateManager;
using Common.DataEncapsulation;
using Common.Transaction;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string odg;
            string username = String.Empty;
            string UserPath = String.Empty;
            SecureString securePw = new SecureString();
            ConsoleKeyInfo key;
            Console.WriteLine("Enter your username: ");
            username = Console.ReadLine();
            Console.WriteLine("Enter your password: ");
            while(true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (securePw.Length > 0)
                    {
                        securePw.RemoveAt(securePw.Length - 1);
                        Console.Write("\b \b");
                    }

                    continue;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }

                securePw.AppendChar(key.KeyChar);
                Console.Write("*");
            };

            try
            {

                ClientProxy clientProxy = new ClientProxy(username, securePw);
                clientProxy.Login();
                UserPath = $"{ClientAppConfig.CertificatePath}{username}.pfx";

                do
                {
                    System.Console.Clear();
                    Console.WriteLine("Welcome to BankService");
                    Console.WriteLine("Chose one of the following options:");
                    Console.WriteLine("1.Withdrawal.");
                    Console.WriteLine("2.Deposit.");
                    Console.WriteLine("3.Check balance.");
                    Console.WriteLine("4.Revoke your MasterCard.");
                    Console.WriteLine("5.Request new MasterCard.");
                    Console.WriteLine("6.Reset your pin code.");
                    Console.WriteLine("7.Exit");
                    odg = Console.ReadLine();

                    switch (odg[0])
                    {
                        case '1': //Withdrawal
                            Withdrawal(clientProxy, UserPath);
                            break;

                        case '2': //Deposit
                            Deposit(clientProxy, UserPath);
                            break;

                        case '3': //CheckBalance
                            CheckBalance(clientProxy, UserPath);
                            break;

                        case '4': //Revoke MasterCard
                            RevokeMasterCard(clientProxy, UserPath);
                            break;

                        case '5': //Request new MasterCard
                            RequestNewMasterCard(clientProxy, UserPath);
                            break;

                        case '6': //Reset Pin
                            ResetPin(clientProxy, UserPath);
                            break;
                    }

                    Console.ReadLine();
                } while (odg[0] != '7');
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error: {ex.Message}");

            }
        }

        private static byte[] Sign(X509Certificate2 cert, ITransaction transaction)
        {
            byte[] signature;
            using (SHA512Cng hasAlg = new SHA512Cng())
            {
                using (var stream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, transaction);

                    var hash = hasAlg.ComputeHash(stream);
                    signature = cert.GetRSAPrivateKey()
                        .SignHash(hash, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
                }


            }

            return signature;
        }

        private static X509Certificate2 TryGetCertificate(string pathCert, string password = null)
        {
            X509Certificate2 cert = null;
            if (password == null)
            {
                Console.WriteLine("Enter your password");
                password = Console.ReadLine();
            }

            try
            {
                cert = CertificateManager.Instance.GetPrivateCertificateFromFile(pathCert, password);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return cert;
        }


        private static void Withdrawal(ClientProxy clientProxy, string path)
        {
            string pin;
            ITransaction transaction;
            byte[] signature;
            X509Certificate2 cert = null;
            bool Result;

            if ((cert = TryGetCertificate(path)) != null)
            {
                Console.WriteLine("How much money do you wish to withdrawal?");
                decimal.TryParse(Console.ReadLine(), out decimal amount);
                Console.WriteLine("Enter your pin:");
                pin = Console.ReadLine();
                transaction = new Transaction(TransactionType.Withdrawal, amount, pin);
                signature = Sign(cert, transaction);
                Result = clientProxy.ExecuteTransaction(signature, transaction);
                if (Result)
                {
                    Console.WriteLine("Successful withdrawal.");
                }
                else
                {
                    Console.WriteLine("You dont have enough amount of money on your MasterCard.");
                }
            }
            else
            {
                Console.WriteLine(
                    "You currently dont own a MasterCard!! If you wish to requet new one select option 5.");
            }
        }

        private static void Deposit(ClientProxy clientProxy, string path)
        {
            string pin;
            ITransaction transaction;
            byte[] signature;
            X509Certificate2 cert = null;
            bool Result;

            if ((cert = TryGetCertificate(path)) != null)
            {
                Console.WriteLine("How much money do you wish to deposit?");
                decimal.TryParse(Console.ReadLine(), out decimal amount);
                Console.WriteLine("Enter your pin:");
                pin = Console.ReadLine();
                transaction = new Transaction(TransactionType.Deposit, amount, pin);
                signature = Sign(cert, transaction);
                Result = clientProxy.ExecuteTransaction(signature, transaction);
                if (Result)
                {
                    Console.WriteLine("Successful deposit.");
                }
                else
                {
                    Console.WriteLine("You dont have enough amount of money on your MasterCard.");
                }
            }
            else
            {
                Console.WriteLine(
                    "You currently dont own a MasterCard!! If you wish to request new one select option 5.");
            }
        }

        private static void CheckBalance(ClientProxy clientProxy, string path)
        {
            decimal amount;
            string pin;
            ITransaction transaction;
            byte[] signature;
            X509Certificate2 cert = null;

            if ((cert = TryGetCertificate(path)) != null)
            {

                Console.WriteLine("Enter your pin:");
                pin = Console.ReadLine();
                transaction = new Transaction(TransactionType.CheckBalance, 0, pin);
                signature = Sign(cert, transaction);
                amount = clientProxy.CheckBalance(signature, transaction);
                Console.WriteLine("Your current balance is {0}", amount);
            }
            else
            {
                Console.WriteLine(
                    "You currently dont own a MasterCard!! If you wish to request new one select option 5.");
            }
        }

        private static void RevokeMasterCard(ClientProxy clientProxy, string path)
        {
            string pin;
            X509Certificate2 cert = null;

            if ((cert = TryGetCertificate(path)) != null)
            {
                Console.WriteLine("Enter your pin:");
                pin = Console.ReadLine();
                clientProxy.RevokeExistingCard(pin);
            }
            else
            {
                Console.WriteLine(
                    "You currently don't own a MasterCard!! If you wish to request new one select option 5.");
            }
        }

        private static void RequestNewMasterCard(ClientProxy clientProxy, string path)
        {

            X509Certificate2 cert = null;
            NewCardResults newCardResults = new NewCardResults();
            Console.WriteLine("Enter key encryption password:");
            string password = Console.ReadLine();

            if ((cert = TryGetCertificate(path, password)) == null)
            {
                newCardResults = clientProxy.RequestNewCard(password);
                Console.WriteLine("Pin code for your new MasterCard is: {0}", newCardResults.PinCode);
            }
            else
            {
                Console.WriteLine("You already own a MasterCard!!");
            }
        }

        private static void ResetPin(ClientProxy clientProxy, string path)
        {
            NewCardResults newCardResults = new NewCardResults();
            string pin;
            X509Certificate2 cert = null;


            if ((cert = TryGetCertificate(path)) != null)
            {
                Console.WriteLine("Enter your pin:");
                pin = Console.ReadLine();
                newCardResults = clientProxy.RequestResetPin(pin);
                Console.WriteLine("Your new pin code is {0}", newCardResults.PinCode);
            }
            else
            {
                Console.WriteLine(
                    "You currently dont own a MasterCard!! If you wish to request new one select option 5.");
            }
        }
    }
}
