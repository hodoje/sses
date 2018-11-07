using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Transaction
{
    [Serializable]
    [DataContract]
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

        [DataMember]
        public TransactionType TransactionType { get; private set; }

        [DataMember]
        public decimal Amount { get; private set; }
    }
}
