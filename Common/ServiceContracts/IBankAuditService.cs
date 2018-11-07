using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Security.Permissions;

namespace Common.ServiceContracts
{
    [ServiceContract]
    public interface IBankAuditService
    {
        /// <summary>
        /// Logs specific event data to a dedicated Windows event log
        /// </summary>
        /// <param name="eventLogData">Object that holds required event log data.</param>
        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "BankService")]
        void Log(EventLogData.EventLogData eventLogData);
    }
}
