using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BankServiceApp.Arbitration;
using Common.Transaction;
using Common.UserData;

namespace BankServiceApp.Replication
{
    public class ReplicatorProxy : IReplicator, IDisposable
    {
        private ChannelFactory<IReplicator> _replicatorProxyFactory;
        private IReplicator _replicatorProxy;

        private ConcurrentQueue<IReplicationItem> _replicationQueue = new ConcurrentQueue<IReplicationItem>();

        private Thread _replicationThread;
        private CancellationTokenSource _replicationTokenSource;
        private bool _disposed = false;

        public ReplicatorProxy()
        {
            if (BankAppConfig.InstanceNo > 1)
            {
                _replicationThread = new Thread(ReplicationWorker);
                _replicationThread.Start(_replicationTokenSource.Token);
            }
        }

        private void ReplicationWorker(object param)
        {
            var cancelationToken = (CancellationToken)param;

            while (!cancelationToken.IsCancellationRequested)
            {
                try
                {
                    if (_replicatorProxy == null)
                    {
                        if (_replicatorProxyFactory.State != CommunicationState.Opened)
                        {

                        }
                    }
                }
                catch (SecurityAccessDeniedException secEx)
                {
                    Console.WriteLine(
                        $"({nameof(BankServiceApp)}) [{nameof(ReplicatorProxy)}] Security exception while trying to replicate: {secEx.Message}");
                    _replicatorProxy = null;

                    // Break on sec exception since replicator probably doesn't have necessary privileges.
                    break;
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"({nameof(BankServiceApp)}) [{nameof(ReplicatorProxy)}] Error: {ex.Message}");
                    _replicatorProxy = null;
                    Thread.Sleep(1000);
                }
            }
        }

        private void CreateReplicatorFactory()
        {
            var binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            var address = BankAppConfig.Endpoints.FirstOrDefault(x => !x.Equals(BankAppConfig.MyAddress));
            var factory = new ChannelFactory<IReplicator>(binding, $"{address}/{BankAppConfig.ReplicatorName}");
        }

        #region IReplicator Methods

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

        #endregion

        public void Dispose()
        {
            if (!_disposed)
            {
                _replicationTokenSource.Cancel();
            }
        }
    }
}
