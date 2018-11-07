using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BankServiceApp.ServiceHosts;
using Common.Transaction;
using Common.UserData;

namespace BankServiceApp.Replicator
{
    public class ReplicationService : IReplicationService
    {
        private readonly ReaderWriterLockSlim _stateLock = new ReaderWriterLockSlim();
        private readonly List<IServiceHost> _registeredServices = new List<IServiceHost>(10);

        private ServiceState _state = ServiceState.Standby;

        public ReplicationService()
        {

        }

        public void RegisterService(IServiceHost service)
        {
            _registeredServices.Add(service);
        }

        public void UnRegisterService(IServiceHost service)
        {
            _registeredServices.Remove(service);
        }

        public void ReplicateTransaction(ITransaction transaction, string clientName)
        {
            throw new NotImplementedException();
        }

        public void ReplicateClientData(IClient clientData)
        {
            throw new NotImplementedException();
        }

        public ServiceState CheckState()
        {
            return State;
        }

        public ServiceState State
        {
            get
            {
                ServiceState state;
                _stateLock.EnterReadLock();
                {
                    state = _state;
                }
                _stateLock.ExitReadLock();
                return state;
            }

            private set
            {
                _stateLock.EnterWriteLock();
                {
                    _state = value;
                }
                _stateLock.ExitWriteLock();
            }
        }

        private void OpenServices()
        {
            foreach (var registeredService in _registeredServices)
            {
                registeredService.OpenService();
            }
        }

        private void CloseServices()
        {
            foreach (var registeredService in _registeredServices)
            {
                registeredService.CloseService();
            }
        }
    }
}
