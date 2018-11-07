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
        /// <param name="pin">Pin for two factor auth.</param>
        /// <returns>
        /// True if new card certificate is successfully created.
        /// </returns>
        [OperationContract]
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
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true, Role = "Client")]
        bool RevokeExistingCard(string pin);
    }
}
