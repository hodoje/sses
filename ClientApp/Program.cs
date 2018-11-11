using Common.CertificateManager;
using Common.DataEncapsulation;
using Common.Transaction;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
            decimal amount = 0;
            bool Result;
            byte[] signature;
            string pin = String.Empty;
            ClientProxy clientProxy = new ClientProxy();
            NewCardResults newCardResults = new NewCardResults();
            X509Certificate2 cert = new X509Certificate2();
            ITransaction transaction;
        
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
                    case '1'://Withdrawal
                        if ((cert = TryGetCertifacate(ClientAppConfig.CertificatePath)) != null)
                        {
                            Console.WriteLine("How much money do you wish to withdrawal?");
                            decimal.TryParse(Console.ReadLine(), out amount);
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
                            Console.WriteLine("You currently dont own a MasterCard!! If you wish to requet new one select option 5.");
                        }
                        break;

                    case '2'://Deposit
                        if ((cert = TryGetCertifacate(ClientAppConfig.CertificatePath)) != null)
                        {
                            Console.WriteLine("How much money do you wish to deposit?");
                            decimal.TryParse(Console.ReadLine(), out amount);
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
                            Console.WriteLine("You currently dont own a MasterCard!! If you wish to request new one select option 5.");
                        }
                        break;

                    case '3'://CheckBalance
                        if ((cert = TryGetCertifacate(ClientAppConfig.CertificatePath)) != null)
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
                            Console.WriteLine("You currently dont own a MasterCard!! If you wish to request new one select option 5.");
                        }
                        break;

                    case '4'://Revoke MasterCard
                        if ((cert = TryGetCertifacate(ClientAppConfig.CertificatePath)) != null)
                        {
                            Console.WriteLine("Enter your pin:");
                            pin = Console.ReadLine();
                            clientProxy.RevokeExistingCard(pin);
                        }
                        else
                        {
                            Console.WriteLine("You currently don't own a MasterCard!! If you wish to request new one select option 5.");
                        }
                        break;

                    case '5'://Request new MasterCard
                        Console.WriteLine("Enter key encryption password:");
                        string password = Console.ReadLine();

                        // Ovo ne valja onemogucavas vise klijenata da se konektuju kroz istu aplikaciju
                        // Treba da iz ovog dir iscitas cert koji je vezan za trenutnog korisnika
                        // Nakon sto implementiras login na pocetku client app
                        if ((cert = TryGetCertifacate(ClientAppConfig.CertificatePath, password)) == null)
                        {
                            newCardResults = clientProxy.RequestNewCard(password);
                            Console.WriteLine("Pin code for your new MasterCard is: {0}", newCardResults.PinCode);
                        }
                        else
                        {
                            Console.WriteLine("You already own a MasterCard!!");
                        }
                        break;

                    case '6'://Reset Pin
                        if ((cert = TryGetCertifacate(ClientAppConfig.CertificatePath)) != null)
                        {
                            Console.WriteLine("Enter your pin:");
                            pin = Console.ReadLine();
                            newCardResults = clientProxy.RequestResetPin(pin);
                            Console.WriteLine("Your new pin code is {0}", newCardResults.PinCode);
                        }
                        else
                        {
                            Console.WriteLine("You currently dont own a MasterCard!! If you wish to request new one select option 5.");
                        }
                        break;
                }
            } while (odg[0] != '7');

        }
        private static byte[] Sign(X509Certificate2 cert,ITransaction transaction)
        {
            byte[] signature;
            using(SHA512Cng hasAlg = new SHA512Cng())
            {
                using(var stream = new MemoryStream())
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

        private static X509Certificate2 TryGetCertifacate(string pathCert, string password = null)
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
    }
}
