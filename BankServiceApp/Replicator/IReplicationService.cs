using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BankServiceApp.ServiceHosts;
using Common.Transaction;
using Common.UserData;

namespace BankServiceApp.Replicator
{
    public enum ServiceState
    {
        Hot,
        Standby
    }

    [ServiceContract]
    public interface IReplicationService
    {
        [OperationContract]
        void ReplicateTransaction(ITransaction transaction, string clientName);

        [OperationContract]
        void ReplicateClientData(IClient clientData);

        [OperationContract]
        ServiceState CheckState();

        void RegisterService(IServiceHost service);

        void UnRegisterService(IServiceHost service);


        ServiceState State { get; }
    }
}
