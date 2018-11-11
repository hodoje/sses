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

namespace BankAuditServiceApp
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    public class BankAuditService : IBankAuditService
    {
        private readonly string _logName = BankAuditServiceConfig.LogName;

        public void Log(EventLogData eventLogData)
        {
            using (EventLog log = new EventLog(_logName))
            {
                log.MachineName = Environment.MachineName;
                log.Source = eventLogData.BankName;
                log.WriteEntry($"{eventLogData.AccountName}: {eventLogData.LogMessage}", eventLogData.EventLogType);
            }
        }
    }
}
