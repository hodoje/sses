using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Common.UserData;

namespace BankServiceApp.AccountStorage
{
    public class BankCache : ICache
    {
        private Dictionary<string, Client> _clients = new Dictionary<string, Client>(100);

        private bool _disposed = false;

        public BankCache()
        {
            LoadData();
        }

        public void LoadData()
        {
            var storagePath = BankAppConfig.BankCachePath;
            var userNames = new List<string>(100);

            // Load all usernames belonging to Clients group
            using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
            {
                using (GroupPrincipal group = GroupPrincipal.FindByIdentity(context, "Clients"))
                {
                    userNames.AddRange(group.Members.Select(x => x.Name));
                }
            }

            // For all users set data to null
            foreach (var userName in userNames)
            {
                _clients[userName] = null;
            }

            // Try to populate client data from persistent storage
            var cacheLocation = $"{BankAppConfig.BankCachePath}{BankAppConfig.BankName}.xml";
            if (File.Exists(cacheLocation))
            {
                using (var stream = new FileStream(cacheLocation, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Client>));
                    var _loadedClients = serializer.Deserialize(stream) as List<Client>;
                    foreach (var loadedClient in _loadedClients)
                    {
                        if (_clients.ContainsKey(loadedClient.Name))
                        {
                            _clients[loadedClient.Name] = loadedClient;
                        }
                    }
                }
            }

            // For each client that isn't in persistent storage populate new data.
            var clientsCpy = _clients.ToList();

            foreach (var client in clientsCpy)
            {
                if (client.Value == null)
                {
                    IAccount account = new Account();
                    _clients[client.Key] = new Client(client.Key, account);
                }
            }
        }

        public void StoreData()
        {
            var storagePath = BankAppConfig.BankCachePath;

            using (var stream = new FileStream($"{BankAppConfig.BankCachePath}{BankAppConfig.BankName}.xml", FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Client>));
                serializer.Serialize(stream, _clients.Select(x => x.Value).ToList());
            }
        }

        public bool TryGetClient(string clientName, out IClient client)
        {
            client = default(IClient);

            var retVal = _clients.TryGetValue(clientName, out Client clientValue);
            if (retVal)
            {
                client = clientValue;
            }

            return retVal;
        }

        public bool TryGetAccount(string clientName, out IAccount account)
        {
            account = default(IAccount);

            var retVal = _clients.TryGetValue(clientName, out Client client);
            if (retVal)
            {
                account = client.Account;
            }

            return retVal;
        }

        public static IClient GetClientFromCache(ICache cache, string clientName)
        {
            IClient client;
            if (!cache.TryGetClient(clientName, out client))
            {
                cache.StoreData();
                cache.LoadData();
                if (cache.TryGetClient(clientName, out client))
                {
                    cache.StoreData();
                }
            }

            return client;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                StoreData();

                _clients.Clear();
                _clients = null;
            }
        }
    }
}
