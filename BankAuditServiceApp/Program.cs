using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAuditServiceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            BankAuditServiceHost bankAuditServiceHost = new BankAuditServiceHost();
            bankAuditServiceHost.OpenService();
            Console.WriteLine("BankAuditService is open...");
            Console.ReadLine();
            bankAuditServiceHost.CloseService();
        }
    }
}
