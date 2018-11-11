using Common.CertificateManager;
using Common.DataEncapsulation;
using Common.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankServiceApp.BankServices
{
    public class BankMasterCardService : IBankMasterCardService
    {
        private X509Certificate2 _CACertificate;

        public BankMasterCardService()
        {
            _CACertificate = CertificateManager.Instance.GetCACertificate();
            if (_CACertificate == null)
            {
                throw new Exception("Certificate manager returned null for CA certificate.");
            }
        }

        #region IBankMasterCardService Methods

        public NewCardResults RequestNewCard(string password)
        {
            var client = Thread.CurrentPrincipal;
            if (!client.IsInRole("Clients"))
            {

            }
            try
            {
                //if (AccountStorage.AccountStorage.Instance.CheckIfClientExists(clientName))
                //{
                //    throw new FaultException<CustomServiceException>(new CustomServiceException("You already have a card in this bank!"),
                //        "You already have a card in this bank!");
                //}
                //else
                //{
                //    // Information that is going to be sent to client 
                //    NewCardResults returnInfo = new NewCardResults()
                //    {
                //        PinCode = GenerateRandomPin()
                //    };

                //    // Create new certificate ( MasterCard ) for client and store it
                //    CertificateManager.Instance.CreateAndStoreNewClientCertificate(clientName,
                //        returnInfo.PinCode, _CACertificate);

                //    // Add client to the "database"
                //    AccountStorage.AccountStorage.Instance.AddNewClient(clientName, returnInfo.PinCode);

                //    return returnInfo;
                //}
            }
            catch(ArgumentNullException ane)
            {
                throw new FaultException<CustomServiceException>(new CustomServiceException(ane.Message + "was null!"),
                    $"{ane.Message} was null!");
            }

            return null;
        }

        public bool RevokeExistingCard(string pin)
        {
            string clientName = Thread.CurrentPrincipal.Identity.Name;
            try
            {
                // Check if client exists
                if (AccountStorage.AccountStorage.Instance.CheckIfClientExists(clientName))
                {
                    // if he exists in the system, authorize him
                    if (AccountStorage.AccountStorage.Instance.ValidateClientPin(clientName, pin))
                    {
                        // TODO CARD REVOCATION

                        return true;
                    }
                    // if the authorization fails, throw CustomServiceException
                    else
                    {
                        throw new FaultException<CustomServiceException>(new CustomServiceException("Pin is not valid!"),
                            "Pin is not valid!");
                    }
                }
                // if the client does not exist in the system
                else
                {
                    throw new FaultException<CustomServiceException>(new CustomServiceException("You are not authenticated!"),
                        "You are not authenticated!");
                }
            }
            catch(ArgumentNullException ane)
            {
                throw new FaultException<CustomServiceException>(new CustomServiceException(ane.Message + "was null!"),
                    $"{ane.Message} was null!");
            }
        }

        public NewCardResults RequestResetPin(string pin)
        {
            string clientName = Thread.CurrentPrincipal.Identity.Name;
            NewCardResults results = null;
            try
            {
                if (AccountStorage.AccountStorage.Instance.CheckIfClientExists(clientName))
                {
                    // if he exists in the system, authorize him
                    if (AccountStorage.AccountStorage.Instance.ValidateClientPin(clientName, pin))
                    {
                        results = new NewCardResults() { PinCode = GenerateRandomPin() };
                        AccountStorage.AccountStorage.Instance.ChangePinCode(clientName, pin, results.PinCode);

                        return results;
                    }
                    // if the authorization fails, throw CustomServiceException
                    else
                    {
                        throw new FaultException<CustomServiceException>(new CustomServiceException("Pin is not valid!"),
                            "Pin is not valid!");
                    }
                }
                // if the client does not exist in the system
                else
                {
                    throw new FaultException<CustomServiceException>(new CustomServiceException("You are not authenticated!"),
                        "You are not authenticated!");
                }
            }
            catch(ArgumentNullException ane)
            {
                throw new FaultException<CustomServiceException>(new CustomServiceException(ane.Message + "was null!"),
                    $"{ane.Message} was null!");
            }
        }

        public void Login()
        {
            var principal = Thread.CurrentPrincipal;
            if (!principal.Identity.IsAuthenticated || !principal.IsInRole("Clients"))
            {
                // Audit failed login
                throw new SecurityAccessDeniedException("User is not authenticated or isn't in Clients role.");
            }
            else
            {
                // Audit success login
            }
        }

        #endregion

        private string GenerateRandomPin()
        {
            string newPin = "";

            Random randomValues = new Random();

            for (int i = 0; i < 4; ++i)
                newPin += randomValues.Next(0, 10).ToString();

            return newPin;
        }
    }
}
