using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Common.UserData
{
    [DataContract]
    [Serializable]
    public class Client : IClient
    {
        public Client()
        {
            
        }

        public Client(string name, IAccount account, string pin = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Account = account ?? throw new ArgumentNullException(nameof(account));
            Pin = pin != null ? GetPinHash(pin) : null;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Pin { get; set; }

        [DataMember]
        [XmlIgnore]
        public IAccount Account { get; set; }

        /// <summary>
        /// Used for serialization purposes
        /// </summary>
        [IgnoreDataMember]
        public decimal Balance
        {
            get => Account.Balance;
            set => Account = new Account(value);
        }

        public virtual void ResetPin(string oldPin, string newPin)
        {
            if (oldPin == null)
            {
                Pin = GetPinHash(newPin);
            }
            else
            {
                var oldPinHash = GetPinHash(oldPin);

                if (Pin.Equals(oldPinHash, StringComparison.OrdinalIgnoreCase))
                {
                    Pin = GetPinHash(newPin);
                }
            }
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
