using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankServiceApp.Replication;
using BankServiceApp.ServiceHosts;

namespace BankServiceApp.Arbitration
{
    public enum ServiceState
    {
        Hot,
        Standby
    }

    public interface IArbitrationServiceProvider : IDisposable
    {
        void RegisterService(IServiceHost service);

        void UnRegisterService(IServiceHost service);

        void OpenServices();

        void CloseServices();

        ServiceState State { get; }
    }
}
