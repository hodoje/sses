using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.UserData
{
    public class Client : IClient
    {
        public Client(string name, IAccount account, string pin)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Account = account ?? throw new ArgumentNullException(nameof(account));
            Pin = GetPinHash(pin ?? throw new ArgumentNullException(nameof(account)));
        }

        public string Name { get; private set; }
        public string Pin { get; private set; }
        public IAccount Account { get; private set; }

        public virtual void ResetPin(string oldPin, string newPin)
        {
            var oldPinHash = GetPinHash(oldPin);

            if (Pin.Equals(oldPinHash, StringComparison.OrdinalIgnoreCase)) Pin = GetPinHash(newPin);
        }

        public bool CheckPin(string pin)
        {
            return Pin.Equals(GetPinHash(pin));
        }

        private string GetPinHash(string pin)
        {
            string pinHash;

            using (var crypto = new SHA512Cng())
            {
                pinHash = Encoding.ASCII.GetString(crypto.ComputeHash(Encoding.ASCII.GetBytes(pin)));
            }

            return pinHash;
        }
    }
}
