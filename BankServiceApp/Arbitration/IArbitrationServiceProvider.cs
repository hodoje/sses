using System;
using BankServiceApp.ServiceHosts;
using Common;

namespace BankServiceApp.Arbitration
{
    public interface IArbitrationServiceProvider : IDisposable
    {
        void RegisterService(IServiceHost service);

        void UnRegisterService(IServiceHost service);

        void OpenServices();

        void CloseServices();

        ServiceState State { get; }
    }
}
