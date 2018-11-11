using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BankServiceApp.Arbitration;
using BankServiceApp.ServiceHosts;
using Common.Transaction;
using Common.UserData;

namespace BankServiceApp.Replication
{
    

    [ServiceContract(ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface IReplicator
    {
        [OperationContract]
        void ReplicateTransaction(ITransaction transaction, string clientName);

        [OperationContract]
        void ReplicateClientData(IClient clientData);

        [OperationContract]
        ServiceState CheckState();
    }
}
