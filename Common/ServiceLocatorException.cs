using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    [Serializable]
    public class ServiceLocatorException : Exception
    {
        public ServiceLocatorException()
        {
        }

        public ServiceLocatorException(string message) : base(message)
        {
        }

        public ServiceLocatorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
