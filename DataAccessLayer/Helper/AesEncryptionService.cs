using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KycBizWebApi.Helper
{
    public class AesEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;
        private readonly ILogger _logger;


        public AesEncryptionService(string keyBase64, string ivBase64)
        {
            _key = Convert.FromBase64String(keyBase64);
            _iv = Convert.FromBase64String(ivBase64);
        }


        public static string GenerateKey(int keySize)
        {
            byte[] key = new byte[keySize / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            return Convert.ToBase64String(key);
        }

        public static string GenerateIV(int blockSize)
        {
            byte[] iv = new byte[blockSize / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv);
            }
            return Convert.ToBase64String(iv);
        }
        public string Encrypt(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    byte[] encryptedBytes = msEncrypt.ToArray();
                    //_logger.LogInformation("AES Encryption Process completed.");
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var msDecrypt = new MemoryStream(cipherBytes))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    //_logger.LogInformation("AES Decryption Process completed.");
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        public string EncryptBase64(string plainText)
        {
            byte[] plainTextBytes = Convert.FromBase64String(plainText);
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainTextBytes);
                    }

                    byte[] encryptedBytes = msEncrypt.ToArray();
                    //_logger.LogInformation("EncryptBase64 Process completed.");
                    return Convert.ToBase64String(encryptedBytes);



                    //using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    //using (var swEncrypt = new StreamWriter(csEncrypt))
                    //{
                    //    swEncrypt.Write(plainTextBytes);
                    //}

                    //byte[] encryptedBytes = msEncrypt.ToArray();
                    //return Convert.ToBase64String(encryptedBytes);
                }
            }
        }
    }
}
