using Common.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankServiceApp.AccountStorage
{
    public interface ICache : IDisposable
    {
        /// <summary>
        /// Get client from storage.
        /// </summary>
        /// <param name="clientName">The client name.</param>
        /// <returns>
        /// Client interface.
        /// </returns>
        bool TryGetClient(string clientName, out IClient client);

        /// <summary>
        /// Get account from storage.
        /// </summary>
        /// <param name="clientName">The client name.</param>
        /// <returns>
        /// Account interface.
        /// </returns>
        bool TryGetAccount(string clientName, out IAccount account);
    }
}
