using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Helper
{
    public interface IAesEncryptionService
    {
        public string Encrypt(string plainText);

        public string Decrypt(string cipherText);

        public string EncryptBase64(string plainText);
    }
}
