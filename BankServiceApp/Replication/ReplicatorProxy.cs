using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BankServiceApp.Arbitration;
using Common.Transaction;
using Common.UserData;

namespace BankServiceApp.Replication
{
    public class ReplicatorProxy : IReplicator
    {
        private ChannelFactory<IReplicator> _replicatorProxyFactory;
        private IReplicator _replicatorProxy;

        private ConcurrentQueue<IReplicationItem> _replicationQueue = new ConcurrentQueue<IReplicationItem>();

        public void ReplicateTransaction(IReplicationItem replicationItem)
        {
            _replicationQueue.Enqueue(replicationItem);
        }

        public void ReplicateClientData(IReplicationItem replicationItem)
        {
            _replicationQueue.Enqueue(replicationItem);
        }

        public ServiceState CheckState()
        {
            return _replicatorProxy.CheckState();
        }
    }
}
