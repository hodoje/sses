using Common.ServiceContracts;
using Common.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ClientProxy(EndpointAddress adressCardService,EndpointAddress addressTransactionService)
        {

            transactionServiceFactory = ChannelFactory<IBankTransactionService>.CreateChannel(binding, addressTransactionService);
            cardServiceFactory = ChannelFactory<IBankMasterCardService>.CreateChannel(binding, adressCardService);

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

        public bool RequestNewCard(string pin)
        {
            bool Result = false;
            try
            {
                Result = cardServiceFactory.RequestNewCard(pin);
            }
            catch (FaultException ex)
            {

                Console.WriteLine("Error: {0}", ex.Message);
            }
            return Result;
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
