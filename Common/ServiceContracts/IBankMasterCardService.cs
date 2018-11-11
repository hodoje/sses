﻿using Common.DataEncapsulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Security;
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
        NewCardResults RequestNewCard(string password);

        /// <summary>
        /// Revoke existing certificate given to client.
        /// </summary>
        /// <param name="pin">Pin which authorizes revocation.</param>
        /// <returns>
        /// True if existing card is successfully revoked.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(CustomServiceException))]
        bool RevokeExistingCard(string pin);

        /// <summary>
        /// Request password reset.
        /// </summary>
        /// <exception cref="CustomServiceException"></exception>
        /// <returns>
        /// NewCardResults that contains all information that is relevant to client
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(CustomServiceException))]
        NewCardResults RequestResetPin(string oldPin);

        /// <summary>
        /// Checks user credentials arrived trough windows api.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(SecurityAccessDeniedException))]
        void Login();
    }
}