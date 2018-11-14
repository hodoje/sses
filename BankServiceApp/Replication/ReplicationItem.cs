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

        public ReplicationItem(IClient client)
        {
            Client = client;
        }

        [DataMember]
        public IClient Client { get; private set; }
    }
}
