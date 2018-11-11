using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServiceApp.Arbitration
{
    public class ArbitrationException : Exception
    { 
        public ArbitrationException(string message) : base(message)
        {
        }

        public ArbitrationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
