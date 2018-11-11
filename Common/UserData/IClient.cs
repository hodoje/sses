using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.UserData
{
    public interface IClient
    {
        [DataMember]
        string Name { get; set; }
        
        [DataMember]
        IAccount Account { get; set; }

        [DataMember]
        string Pin { get; set; }

        void ResetPin(string oldPin, string newPin);

        bool CheckPin(string pin);
    }
}
