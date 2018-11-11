using System;
using System.Runtime.Serialization;
using Common.Transaction;
using Common.UserData;

namespace BankServiceApp.Replication
{
    [DataContract]
    [Serializable]
    public enum ReplicationType
    {
        Transaction,
        ClientData
    }

    public interface IReplicationItem
    {
        [DataMember]
        ReplicationType Type { get; }

        [DataMember]
        IClient Client { get; }

        [DataMember]
        ITransaction Transaction { get; }
    }
}