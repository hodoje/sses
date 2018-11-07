using Common.DataEncapsulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.ServiceContracts
{
    [ServiceContract]
    public interface IBankMasterCardService
    {
        /// <summary>
        /// Request new card certificate creation.
        /// </summary>
        /// <returns>
        /// NewCardResults that contains all information that is relevant to client
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(CustomServiceException))]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "Client")]
        NewCardResults RequestNewCard();

        /// <summary>
        /// Revoke existing certificate given to client.
        /// </summary>
        /// <param name="pin">Pin which authorizes revocation.</param>
        /// <returns>
        /// True if existing card is successfully revoked.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(CustomServiceException))]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "Client")]
        bool RevokeExistingCard(string pin);

        /// <summary>
        /// Request password reset.
        /// </summary>
        /// <returns>
        /// NewCardResults that contains all information that is relevant to client
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(CustomServiceException))]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "Client")]
        NewCardResults RequestResetPin(string pin);

    }
}