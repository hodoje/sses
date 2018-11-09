using Common.CertificateManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BankServiceApp.Replicator;

namespace BankServiceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ReplicationService replicationService = new ReplicationService())
            {
                replicationService.RegisterService(new BankServicesHost());
                replicationService.OpenServices();
                Console.ReadLine();
            }
        }
    }
}
