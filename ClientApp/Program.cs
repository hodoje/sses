using Common.CertificateManager;
using Common.DataEncapsulation;
using Common.Transaction;
using System;
using System.Collections.Generic;
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
            string pathCert = String.Empty;
            decimal amount = 0;
            byte[] signature;
            string pin = String.Empty;
            ClientProxy clientProxy = new ClientProxy();
            NewCardResults newCardResults = new NewCardResults();
            Console.WriteLine("Welcome to BankService");
            X509Certificate2 cert = new X509Certificate2();
            ITransaction transaction;
            //We trust our users :D
            Console.WriteLine("Do you have MasterCard?");
            Console.WriteLine("1.Yes");
            Console.WriteLine("2.No");
            odg = Console.ReadLine();

            if (odg[0] == '2')
            {
                newCardResults = clientProxy.RequestNewCard();
                Console.WriteLine("Pin code of your new MasterCard is: {0}",newCardResults.PinCode);
            }
            //We do not trust our users :(
           /* Console.WriteLine("Enter your password");
            bool failed = true;
            password = Console.ReadLine();
            try
            {
                cerPrivate = CertificateManager.Instance.GetPrivateCertificateFromFile(pathCert, password);
                failed = false;
            }
            catch (CryptographicException ex)
            {

               
            }

            if (failed)
            {
                Console.WriteLine("Do you want to make new MasterCard?");
                Console.WriteLine("1.Yes");
                Console.WriteLine("2.No");
                odg = Console.ReadLine();
            }
            
            if(odg[0] == '1')
            {
                newCardResults = clientProxy.RequestNewCard();
                Console.WriteLine("Pin code of your new MasterCard is: {0}", newCardResults.PinCode);
            }
            else
            {
                return;
            }*/
            //if(CertificateManager.Instance.GetPrivateCertificateFromFile())
           //Dodati dodaj novi masterCard option,proveriti 1 2 3 5 da li postoji tj nmz nista radiiti bez cert,gornje se moze sve obrisati
            do
            {
                System.Console.Clear();
                Console.WriteLine("Welcome to BankService");
                Console.WriteLine("Chose one of the following options:");
                Console.WriteLine("1.Withdrawal.");
                Console.WriteLine("2.Deposit.");
                Console.WriteLine("3.Check balance.");
                Console.WriteLine("4.Revoke your MasterCard.");
                Console.WriteLine("5.Reset your pin code.");
                Console.WriteLine("6.Exit");
                odg = Console.ReadLine();
              
                switch (odg[0])
                {
                    case '1'://Withdrawal
                        cert = GetCert(pathCert);
                        Console.WriteLine("How much money do you wish to withdrawal?");
                        decimal.TryParse(Console.ReadLine(),out amount);
                        transaction = new Transaction(TransactionType.Withdrawal, amount);
                        signature = Sign(cert, transaction);
                        Console.WriteLine("Enter your pin:");
                        pin = Console.ReadLine();
                        amount = clientProxy.ExecuteTransaction(signature, pin);
                        Console.WriteLine("Your new balance is {0}",amount);
                        break;

                    case '2'://Deposit
                        cert = GetCert(pathCert);
                        Console.WriteLine("How much money do you wish to deposit?");
                        decimal.TryParse(Console.ReadLine(), out amount);
                        transaction = new Transaction(TransactionType.Deposit, amount);
                        signature = Sign(cert, transaction);
                        Console.WriteLine("Enter your pin:");
                        pin = Console.ReadLine();
                        amount = clientProxy.ExecuteTransaction(signature, pin);
                        Console.WriteLine("Your new balance is {0}", amount);

                        break;

                    case '3'://CheckBalance
                        cert = GetCert(pathCert);
                        transaction = new Transaction(TransactionType.CheckBalance, 0);
                        signature = Sign(cert, transaction);
                        Console.WriteLine("Enter your pin:");
                        pin = Console.ReadLine();
                        amount = clientProxy.ExecuteTransaction(signature, pin);
                        Console.WriteLine("Your current balance is {0}", amount);
                        break;

                    case '4'://Revoke MasterCard
                        Console.WriteLine("Enter your pin:");
                        pin = Console.ReadLine();
                        clientProxy.RevokeExistingCard(pin);
                        Console.WriteLine("Do you wish to request new MasterCard?");
                        Console.WriteLine("1.Yes");
                        Console.WriteLine("2.No");
                        string response = Console.ReadLine();
                        if(response[0] == '1')
                        {
                            newCardResults = clientProxy.RequestNewCard();
                            Console.WriteLine("Pin code of your new MasterCard is: {0}", newCardResults.PinCode);
                        }
                        else
                        {
                            return;
                        }
                        break;

                    case '5'://Reset Pin
                        Console.WriteLine("Enter your pin:");
                        pin = Console.ReadLine();
                        newCardResults = clientProxy.RequestResetPin(pin);
                        Console.WriteLine("Your new pin code is {0}",newCardResults.PinCode);
                        break;

                    default:
                        break;
                }

            
            } while (odg[0] != '6');



          
           

       
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
        private static X509Certificate2 GetCert(string pathCert)
        {
            X509Certificate2 cert = null;
            Console.WriteLine("Enter your password");
            string  password = Console.ReadLine();
            cert = CertificateManager.Instance.GetPrivateCertificateFromFile(pathCert, password);
            return cert;
        }
    }
}
