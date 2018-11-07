using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Transaction;

namespace Common.UserData
{
    [Serializable]
    [DataContract]
    public class Account : IAccount
    {
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private decimal _balance;

        public Account()
        {
            
        }

        public Account(decimal initialBalance = 0m)
        {
            Balance = initialBalance;
        }

        [DataMember]
        public decimal Balance
        {
            get
            {
                decimal balance;
                _rwLock.EnterReadLock();
                {
                    balance = _balance;
                }
                _rwLock.ExitReadLock();

                return balance;
            }
            private set
            {
                _rwLock.EnterWriteLock();
                {
                    _balance = value;

                }
                _rwLock.ExitWriteLock();
            }
        }

        public void Deposit(decimal amount)
        {
            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if(amount > Balance)
                throw new NotEnoughResourcesException("User tried to withdraw more resources than available.");
            Balance -= amount;
        }
    }
}
