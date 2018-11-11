using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BankServiceApp.Replication;
using BankServiceApp.ServiceHosts;
using Common.Transaction;
using Common.UserData;

namespace BankServiceApp.Arbitration
{
    public class ArbitrationServiceProvider : IArbitrationServiceProvider
    {
        private ReaderWriterLockSlim _stateLock = new ReaderWriterLockSlim();
        private List<IServiceHost> _registeredServices = new List<IServiceHost>(10);

        private static ServiceState _state = ServiceState.Standby;

        private readonly string _myEndpoint = null;
        private ServiceHost _replicatorHost = null;

        private IReplicator _replicatorProxy;
        private ChannelFactory<IReplicator> _replicatorProxyFactory;

        private bool _disposed = false;

        public ArbitrationServiceProvider()
        {
            if(BankAppConfig.InstanceNo < 1 || BankAppConfig.InstanceNo > 2)
            {
                throw new ArbitrationException($"Invalid instance number. Range [1,2], is {BankAppConfig.InstanceNo}");
            }

            var endpoints = BankAppConfig.Endpoints;
            var replicator = BankAppConfig.ReplicatorName;

            var binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;

            if (BankAppConfig.InstanceNo > 1)
            {
                List<string> successList = new List<string>(BankAppConfig.InstanceNo);
                foreach (var endpoint in endpoints)
                {
                    try
                    {
                        var factory = new ChannelFactory<IReplicator>(binding, $"{endpoint}/{replicator}");
                        factory.Credentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;

                        var channel = factory.CreateChannel();
                        if (channel.CheckState() == ServiceState.Hot)
                        {
                            successList.Add(endpoint);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Failed to establish connection to other service on {endpoint}.");
                        BankAppConfig.MyAddress = _myEndpoint = _myEndpoint ?? endpoint;
                    }
                }

                if (successList.Count == 0)
                {
                    State = ServiceState.Hot;
                    Console.WriteLine($"No {nameof(BankServiceApp)} is HOT => {nameof(BankServiceApp)}_{Process.GetCurrentProcess().Id} will assert.");
                }
            }
            else
            {
                BankAppConfig.MyAddress = _myEndpoint = BankAppConfig.Endpoints[0];
                State = ServiceState.Hot;
            }

            _replicatorHost = new ServiceHost(typeof(ReplicatorService));
            _replicatorHost.AddServiceEndpoint(typeof(IReplicator), binding, $"{_myEndpoint}/{replicator}");
            _replicatorHost.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.UseWindowsGroups;
            _replicatorHost.Authorization.ImpersonateCallerForAllOperations = true;

            _replicatorHost.Open();
            Console.WriteLine($"Replication service open on {_myEndpoint}/{replicator}");
        }

        #region IArbitrationServiceProvider
    
        public void RegisterService(IServiceHost service)
        {
            _registeredServices.Add(service);
        }

        public void UnRegisterService(IServiceHost service)
        {
            _registeredServices.Remove(service);
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

        public void OpenServices()
        {
            if (State == ServiceState.Hot)
            {
                foreach (var registeredService in _registeredServices)
                {
                    registeredService.OpenService();
                }
            }
            else
            {
                Console.WriteLine($"Services startup aborted since server is in STANDBY mode.");
            }
        }

        public void CloseServices()
        {
            foreach (var registeredService in _registeredServices)
            {
                registeredService.CloseService();
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            try
            {
                if (!_disposed)
                {
                    _disposed = true;

                    State = ServiceState.Standby;
                    CloseServices();
                    _registeredServices.Clear();
                    _registeredServices = null;

                    _stateLock.Dispose();
                    _stateLock = null;

                    (_replicatorHost as IDisposable).Dispose();
                    _replicatorHost = null;
                }
            }
            catch
            {
                if (_disposed)
                {
                    Console.WriteLine("(BankServiceApp) [ReplicationService] Dispose called on disposing object.");
                }
            }
        }
        
        #endregion
    }
}
