using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common.Transaction;

namespace Common.ServiceContracts
{
    [ServiceContract]
    public interface IBankTransactionService
    {
        /// <summary>
        /// Execute requested transaction.
        /// </summary>
        /// <param name="transaction">Client created transaction.</param>
        /// <returns>
        /// Remaining amount of money on the given account
        /// </returns>
        [OperationContract]
        bool ExecuteTransaction(byte[] signiture,ITransaction transaction);
        [OperationContract]
        decimal CheckBalance(byte [] signiture,ITransaction transaction);
    }
}
