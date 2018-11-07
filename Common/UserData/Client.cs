using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace Common.UserData
{
    [DataContract]
    [Serializable]
    public class Client : IClient
    {
        public Client()
        {
            
        }

        public Client(string name, IAccount account, string pin)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Account = account ?? throw new ArgumentNullException(nameof(account));
            Pin = GetPinHash(pin ?? throw new ArgumentNullException(nameof(account)));
        }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public string Pin { get; private set; }

        [DataMember]
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
