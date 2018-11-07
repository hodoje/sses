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
    //TO DO:Client proxy(BankService and BankTransaction),reg(first if there is cert if not make new ifthere is cert log with pin),uplata,isplata,check stanje,izdaj novi cert,izdaj novi pin(u switch)
    class Program
    {
        static void Main(string[] args)
        {

            ClientProxy clientProxy = new ClientProxy();
            Console.WriteLine("Connected to service..");
            Console.ReadLine();
       
        }
    }
}
