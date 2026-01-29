using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmyloxFirstUI.Helpers
{
    public class SecureTokenStorage
    {
        public void saveToken(string key, string token)
        {
            byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
            byte[] encryptedBytes = ProtectedData.Protect(
                tokenBytes,
                null,
                DataProtectionScope.CurrentUser
                );

            string encryptedBase64 = Convert.ToBase64String(encryptedBytes);

            Properties.Settings.Default[key] = encryptedBase64;
            Properties.Settings.Default.Save();
        }

        public string ? retrieveToken(string key)
        {
            string? encryptedBase64 = Properties.Settings.Default[key] as string;
            if (string.IsNullOrEmpty(encryptedBase64))
            {
                return null;
            }
            byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);
            byte[] decryptedBytes = ProtectedData.Unprotect(
                encryptedBytes,
                null,
                DataProtectionScope.CurrentUser
                );
            string token = Encoding.UTF8.GetString(decryptedBytes);
            return token;
        }

        public void clearTokens()
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
        }
    }
}
