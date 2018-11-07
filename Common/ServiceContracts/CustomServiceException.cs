using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ServiceContracts
{

    public class CustomServiceException : Exception
    {
        private string strMessage;

        public CustomServiceException()
        {
            strMessage = "An unknown exception occurred";
        }

        public CustomServiceException(string Message)
        {
            strMessage = Message;
        }

        public override string Message
        {
            get => strMessage;
        }
    }
}
