using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Common.Infrastructure
{
    public class CustomKeyStoreProvider : SqlColumnEncryptionKeyStoreProvider
    {
        private string masterKey = "";
        public CustomKeyStoreProvider(DBCustomEncryption dBCustomEncryption)
        {
            Dictionary<string, SqlColumnEncryptionKeyStoreProvider> providers = new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>();
            providers.Add(dBCustomEncryption.StoreProvider, this);
            masterKey = dBCustomEncryption.MasterKey;
            SqlConnection.RegisterColumnEncryptionKeyStoreProviders(providers);
        }
        //This will constantly get used
        public override byte[] DecryptColumnEncryptionKey(string masterKeyPath, string encryptionAlgorithm, byte[] encryptedColumnEncryptionKey)
        {
            return Cryptography.Decrypt(encryptedColumnEncryptionKey, masterKey);
        }

        //This will never get used by the app, I've used it just to encrypt the column key
        public override byte[] EncryptColumnEncryptionKey(string masterKeyPath, string encryptionAlgorithm, byte[] columnEncryptionKey)
        {
            return Cryptography.Encrypt(columnEncryptionKey, masterKey);
        }
    }
}
