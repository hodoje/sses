using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.UserData
{
    public interface IClient
    {
        string Name { get; }
        
        IAccount Account { get; }

        void ResetPin(string oldPin, string newPin);

        bool CheckPin(string pin);
    }
}
