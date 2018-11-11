using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BankServiceApp.Arbitration;
using Common;
using Common.Transaction;
using Common.UserData;

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
