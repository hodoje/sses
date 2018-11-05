using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Common.CertificateManager
{
    public interface ICertificateManager
    {
        /// <summary>
        /// Get certificate from windows storage.
        /// </summary>
        /// <param name="storeLocation">Storage location.</param>
        /// <param name="storeName">Storage name.</param>
        /// <param name="subjectName">Certificate subject CN.</param>
        /// <returns>
        /// X509Certificate instance or null if invalid subject name.
        /// </returns>
        X509Certificate2 GetCertificateFromStore(StoreLocation storeLocation, StoreName storeName, string subjectName);

        /// <summary>
        /// Get public certificate from certificate file.
        /// </summary>
        /// <param name="filePath">Path to .cer file.</param>
        /// <exception cref="CryptographicException">Throws if <see cref="X509Certificate2"/> can't be created.</exception>
        /// <exception cref="CryptographicException">Throws if password is invalid.</exception>
        /// <returns>
        /// X509Certificate instance or null if invalid path.
        /// </returns>
        X509Certificate2 GetPublicCertificateFromFile(string filePath);
        /// <summary>
        /// Get full information from certificate file.
        /// </summary>
        /// <param name="filePath">Path to .pfx file.</param>
        /// <param name="password">Private key password.</param>
        /// <exception cref="CryptographicException">Throws if <see cref="X509Certificate2"/> can't be created.</exception>
        /// <exception cref="ArgumentNullException">Throws if any of arguments is null.</exception>
        /// <returns>
        /// X509Certificate instance or null if invalid path.
        /// </returns>
        X509Certificate2 GetPrivateCertificateFromFile(string filePath, string password);

        /// <summary>
        /// Creates new certificate, stores it in personal storage and returns path to .pfx file to distribute to client.
        /// </summary>
        /// <param name="subjectName">Client name.</param>
        /// <param name="pvkPass">Password for client private key.</param>
        /// <param name="signingCertificatePath">Path to signing certificate .pfx file.</param>
        /// <returns>
        /// String containing path to .pfx file.
        /// </returns>
        string CreateAndStoreNewClientCertificate(string subjectName, string pvkPass, X509Certificate2 signingCertificate);
    }
}
