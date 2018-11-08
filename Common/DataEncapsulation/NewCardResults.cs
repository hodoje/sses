using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataEncapsulation
{
    /// <summary>
    /// Is used as a return value for Clients request for services that include changing or adding information about the card
    /// </summary>
    public class NewCardResults
    {
        /// <summary>
        /// Pincode for the new card that is requested
        /// </summary>
        public string PinCode { get; set; }
    }
}
