using System.Net.Security;
using System.ServiceModel;
using BankServiceApp.Arbitration;
using Common.Transaction;
using Common.UserData;

namespace BankServiceApp.Replication
{


    [ServiceContract(ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface IReplicator
    {
        [OperationContract]
        void ReplicateData(IReplicationItem replicationData);

        [OperationContract]
        ServiceState CheckState();
    }
}
