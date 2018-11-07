using Common.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.EventLogData;
using System.Diagnostics;
using System.Configuration;

namespace BankAuditServiceApp
{
    public class BankAuditService : IBankAuditService
    {
        private string _logName = ConfigurationManager.AppSettings["logName"];

        public void Log(EventLogData eventLogData)
        {
            string sourceName = GetLogSourceName(eventLogData);
            using (EventLog log = new EventLog(_logName))
            {
                log.MachineName = Environment.MachineName;
                log.Source = sourceName;
                log.WriteEntry(eventLogData.LogMessage, eventLogData.EventLogType);
            }
        }

        private string GetLogSourceName(EventLogData eventLogData)
        {
            return $"{eventLogData.BankName}_{eventLogData.AccountName}_{eventLogData.DetectionTime}";
        }
    }
}
