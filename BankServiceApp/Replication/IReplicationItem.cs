using System;
using System.Runtime.Serialization;
using Common.Transaction;
using Common.UserData;

namespace BankServiceApp.Replication
{
    public interface IReplicationItem
    {

        [DataMember]
        IClient Client { get; }
    }
}