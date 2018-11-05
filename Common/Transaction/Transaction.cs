using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Transaction
{
    public class Transaction : ITransaction
    {
        public Transaction(TransactionType type, decimal amount)
        {
            TransactionType = type;
            Delta = amount;
        }

        public TransactionType TransactionType { get; private set; }
        public decimal Delta { get; private set; }
    }
}
