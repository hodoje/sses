using Common.CertificateManager;
using Common.DataEncapsulation;
using Common.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankServiceApp.BankServices
{
    public class BankMasterCardService : IBankMasterCardService
    {
        X509Certificate2 _issuer;
        public BankMasterCardService(X509Certificate2 issuer)
        {
            _issuer = issuer;
        }
        private string GenerateRandomPin()
        {
            string newPin = "";

            Random randomValues = new Random();

            for (int i = 0; i < 4; ++i)
                newPin += randomValues.Next(0, 10).ToString();

            return newPin;
        }
        public NewCardResults RequestNewCard()
        {
            if(AccountStorage.AccountStorage.Instance.ClientDictionary.ContainsKey(Thread.CurrentPrincipal.Identity.Name))
            {
                throw new FaultException<CustomServiceException>(new CustomServiceException("You already have a card in this bank!"),
                    "You already have a card in this bank!");
            }
            else
            {
                NewCardResults returnInfo = new NewCardResults()
                {
                    PinCode = GenerateRandomPin()
                };

                CertificateManager.Instance.CreateAndStoreNewClientCertificate(Thread.CurrentPrincipal.Identity.Name,
                    returnInfo.PinCode, _issuer);

                // TODO (JOKI) STORE NEW USER TO ACCOUNT STORAGE

                return returnInfo;
            }
        }

        public bool RevokeExistingCard(string pin)
        {
            // TODO (JOKI) Check clients credentials

            /*
                if( AccountStorage.UserExists( usersName ) )
                {
                    if ( AccountStorage.Authorize ( usersName, pin ) )
                    {
                        ADD CARD TO REVOCATION LIST
                    }
                    else
                    {
                        return false or throw exception on Authorization failed ( wrong pin )
                    }
                }
                else
                {
                    return false or throw exception ( wrong username = MasterCard does not exist )
                }
            */
            throw new NotImplementedException();
        }

        public NewCardResults RequestResetPassowrd(string pin)
        {
            // TODO (JOKI) Check clients credentials

            /*
                if( AccountStorage.UserExists( usersName ) )
                {
                    if ( AccountStorage.Authorize ( usersName, pin ) )
                    {
                        CHANGE PASSWORD
                        Generate new Pin

                        CHANGE ENTITY INFO ON ACCOUNT STORAGE

                        return newInfo;
                    }
                    else
                    {
                        return false or throw exception on Authorization failed ( wrong pin )
                    }
                }
                else
                {
                    return false or throw exception ( wrong username = MasterCard does not exist )
                }
            */

            throw new NotImplementedException();
        }
    }
}
