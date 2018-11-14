using System;
using System.Security.Permissions;
using System.ServiceModel;
using System.Threading;
using BankServiceApp.AccountStorage;
using BankServiceApp.Arbitration;
using Common;

namespace BankServiceApp.Replication
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    public class ReplicatorService : IReplicator
    {
        private readonly IArbitrationServiceProvider _arbitrationServiceProvider;
        private readonly ICache _bankCache;

        public ReplicatorService()
        {
            _arbitrationServiceProvider = ServiceLocator.GetInstance<IArbitrationServiceProvider>();
            _bankCache = ServiceLocator.GetInstance<ICache>();
        }

        #region IReplicator

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "BankServices")]
        public void ReplicateData(IReplicationItem replicationData)
        {
            var principal = Thread.CurrentPrincipal;
            Console.WriteLine($"Replicating cache data from {principal.Identity.Name}");
            BankCache.GetClientFromCache(_bankCache, replicationData.Client.Name);
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "BankServices")]
        public ServiceState CheckState()
        {
            return _arbitrationServiceProvider.State;
        }

        #endregion
    }
}
