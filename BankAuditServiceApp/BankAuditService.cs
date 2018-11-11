using Common.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.EventLogData;
using System.Diagnostics;
using System.Configuration;
using System.ServiceModel;
using System.Threading;

namespace BankAuditServiceApp
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    public class BankAuditService : IBankAuditService
    {
        private readonly string _logName = BankAuditServiceConfig.LogName;

        public void Log(EventLogData eventLogData)
        {
            if (!EventLog.SourceExists(eventLogData.BankName))
            {
                EventLog.CreateEventSource(eventLogData.BankName, _logName);

                // Giving OS the time to register the source
                Thread.Sleep(50);
            }
            using (EventLog log = new EventLog(_logName))
            {
                log.MachineName = Environment.MachineName;
                log.Source = eventLogData.BankName;
                log.WriteEntry($"{eventLogData.AccountName}: {eventLogData.LogMessage}", eventLogData.EventLogType);
            }
        }
    }
}
