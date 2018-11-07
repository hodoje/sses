using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Transaction
{
    public enum TransactionType
    {
        Deposit,
        Withdrawal
    }
    
    public interface ITransaction
    {
        /// <summary>
        /// Type of transaction.
        /// </summary>
        TransactionType TransactionType { get; }

        /// <summary>
        /// Delta to be processed.
        /// </summary>
        decimal Amount { get; }
    }
}
