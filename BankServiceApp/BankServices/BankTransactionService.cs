using Common.ServiceContracts;
using Common.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BankServiceApp.BankServices
{
    public class BankTransactionService : IBankTransactionService
    {
        public decimal CheckBalance(byte[] signiture, ITransaction transaction)
        {
            return 0m;
        }

        public bool ExecuteTransaction(byte[] signiture, ITransaction transaction)
        {
            // TODO (JOKI) WAITING ON ACCOUNT STORAGE

            /*
                if( AccountStorage.UserExists( usersName ) )
                {
                    if ( AccountStorage.Authorize ( usersName, pin ) )
                    {
                        UserPublicKey publicKey = GetUsersPublicKey(usersName)
                        TransactionType transactionType = getTransactionFromByteArray(transaction)
                        byte[] digitalSignedPart = getDigitalSignature(transaction)
                        Hash(transactionType, publicKey) == digitalSignedPart ? 
                            execute 
                            :
                            throw exception ( integrity violated )
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
