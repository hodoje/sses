using Common.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServiceApp.AccountStorage
{
    public interface IAccountStorage
    {
        /// <summary>
        /// Caching client data
        /// </summary>
        Dictionary<string, IClient> ClientDictionary { get; }

        /// <summary>
        /// Serialize all clients to XML file
        /// </summary>
        /// <param name="cleints">Clients list.</param>
        void StoreClients(List<IClient> clients);

        /// <summary>
        /// Loads clients from given XML file
        /// </summary>
        /// <param name="filePath">Path to the XML file.</param>
        /// <exception cref="ArgumentNullException">Throws if the filePath is not valid.</exception>
        /// <returns>
        /// List of clients
        /// </returns>
        List<IClient> LoadClients(string filePath);
    }
}
