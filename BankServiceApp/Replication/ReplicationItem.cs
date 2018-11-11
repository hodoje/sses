using System;
using Common.Transaction;
using Common.UserData;
using System.Runtime.Serialization;

namespace BankServiceApp.Replication
{
    [DataContract]
    [Serializable]
    public class ReplicationItem : IReplicationItem
    {
        public ReplicationItem()
        {
            
        }

        public ReplicationItem(ReplicationType type, IClient client, ITransaction transaction)
        {
            Type = type;
            Client = client;
            Transaction = transaction;
        }

        [DataMember]
        public ReplicationType Type { get; private set; }

        [DataMember]
        public IClient Client { get; private set; }

        [DataMember]
        public ITransaction Transaction { get; private set; }
    }
}
