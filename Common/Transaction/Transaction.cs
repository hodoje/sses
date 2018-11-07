using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Transaction
{
    [Serializable]
    public class Transaction : ITransaction
    {
        public Transaction()
        {
            
        }

        public Transaction(TransactionType type, decimal amount)
        {
            TransactionType = type;
            Amount = amount;
        }

        public TransactionType TransactionType { get; private set; }
        public decimal Amount { get; private set; }
    }
}
