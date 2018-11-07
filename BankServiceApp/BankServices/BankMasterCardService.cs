using Common.DataEncapsulation;
using Common.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServiceApp.BankServices
{
    public class BankMasterCardService : IBankMasterCardService
    {
      

        public NewCardResults RequestNewCard()
        {
            throw new NotImplementedException();
        }

        public bool RevokeExistingCard(string pin)
        {
            throw new NotImplementedException();
        }
    }
}
