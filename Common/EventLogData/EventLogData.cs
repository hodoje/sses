using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Common.EventLogData
{
    /// <summary>
    /// Represents an item for EventLog
    /// </summary>
    [Serializable]
    [DataContract]
    public class EventLogData
    {
        [DataMember]
        public string BankName { get; set; }
        [DataMember]
        public string AccountName { get; set; }
        [DataMember]
        public DateTime DetectionTime { get; set; }
        [DataMember]
        public string LogMessage { get; set; }
        [DataMember]
        public EventLogEntryType EventLogType { get; set; }

        public EventLogData() { }

        public EventLogData(string bankName, string accountName, DateTime detectionTime, string logMessage)
        {
            BankName = !String.IsNullOrWhiteSpace(bankName) ? bankName : throw new ArgumentNullException(nameof(bankName));
            AccountName = !String.IsNullOrWhiteSpace(accountName) ? accountName : throw new ArgumentNullException(nameof(accountName));
            DetectionTime = !(DateTime.MinValue == detectionTime) ? detectionTime : throw new ArgumentOutOfRangeException(nameof(detectionTime));
            LogMessage = !String.IsNullOrWhiteSpace(logMessage) ? logMessage : throw new ArgumentNullException(nameof(logMessage));
        }
    }
}
