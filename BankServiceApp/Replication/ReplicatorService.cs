using System.Security.Permissions;
using System.ServiceModel;
using BankServiceApp.Arbitration;
using Common;

namespace BankServiceApp.Replication
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    public class ReplicatorService : IReplicator
    {
        private readonly IArbitrationServiceProvider _arbitrationServiceProvider;

        public ReplicatorService()
        {
            _arbitrationServiceProvider = ServiceLocator.GetInstance<IArbitrationServiceProvider>();
        }

        #region IReplicator

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "Replicator")]
        public void ReplicateData(IReplicationItem replicationData)
        {
            switch (replicationData.Type)
            {
                case ReplicationType.ClientData:
                    break;
                case ReplicationType.Transaction:
                    break;
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "Replicator")]
        public ServiceState CheckState()
        {
            return _arbitrationServiceProvider.State;
        }

        #endregion
    }
}
