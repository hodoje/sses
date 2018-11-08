using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.UserData;

namespace BankServiceApp.AccountStorage
{
    public class AccountStorage : IAccountStorage
    {
        private Dictionary<string, IClient> clientDictionary;

        private static IAccountStorage _instance;
        private static object _sync = new object();

        public AccountStorage()
        {
            
        }

        public static IAccountStorage Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock(_sync)
                    {
                        if (_instance == null)
                            _instance = new AccountStorage();
                    }
                }

                return _instance;
            }
        }

        public Dictionary<string, IClient> ClientDictionary
        {
            get => clientDictionary;
            private set => clientDictionary = value;
        }

        public List<IClient> LoadClients(string filePath)
        {
            throw new NotImplementedException();
        }

        public void StoreClients(List<IClient> clients)
        {
            throw new NotImplementedException();
        }
    }
}
