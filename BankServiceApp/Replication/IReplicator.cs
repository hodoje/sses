﻿using System.ServiceModel;
using Common;
using Common.Transaction;
using Common.UserData;

namespace BankServiceApp.Replication
{


    [ServiceContract]
    [ServiceKnownType(typeof(ReplicationItem))]
    [ServiceKnownType(typeof(Transaction))]
    [ServiceKnownType(typeof(Client))]
    [ServiceKnownType(typeof(Account))]
    public interface IReplicator
    {
        [OperationContract]
        void ReplicateData(IReplicationItem replicationData);

        [OperationContract]
        ServiceState CheckState();
    }
}
