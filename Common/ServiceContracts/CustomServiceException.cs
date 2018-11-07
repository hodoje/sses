using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ServiceContracts
{

    public class CustomServiceException : Exception
    {
        public CustomServiceException() : base("An unknown exception occurred")
        {

        }

        public CustomServiceException(string Message) : base(Message)
        {
        }
    }
}
