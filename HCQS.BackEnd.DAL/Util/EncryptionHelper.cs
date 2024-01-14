using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace HCQS.BackEnd.DAL.Util
{
    public class EncryptionHelper
    {
        private static string SecretKey = "03122002"; // Thay thế bằng secret key thực tế của bạn
        private readonly IConfiguration _configuration;

        public EncryptionHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GenerateKey(SecretKey, aesAlg.KeySize);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GenerateKey(SecretKey, aesAlg.KeySize);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        private static byte[] GenerateKey(string secretKey, int keySize)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
                return sha256.ComputeHash(keyBytes).Take(keySize / 8).ToArray();
            }
        }
    }
}