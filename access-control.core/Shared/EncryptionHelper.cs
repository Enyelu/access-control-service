using System.Security.Cryptography;
using System.Text;

namespace access_control.core.Shared
{
    public class EncryptionHelper
    {
        public static string Decrypt(string encryptedText, string ngCipherKeyIvPhrase)
        {
            string plainText = string.Empty;
            var cipherTextArray = ngCipherKeyIvPhrase.Split("|");
            string cipherPhrase = cipherTextArray[0];
            string salt = cipherTextArray[1];

            byte[] cipherText = Convert.FromBase64String(encryptedText);
            // Create an AesManaged object  
            // with the specified key and IV.  
            using var aesAlg = new AesManaged();
            //Settings  
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.FeedbackSize = 128;

            aesAlg.Key = Encoding.UTF8.GetBytes(cipherPhrase);
            aesAlg.IV = Encoding.UTF8.GetBytes(salt);
            // Create a decryptor to perform the stream transform.  
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.  
            using (var msDecrypt = new MemoryStream(cipherText))
            {
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

                using var srDecrypt = new StreamReader(csDecrypt);
                // Read the decrypted bytes from the decrypting stream  
                // and place them in a string.  
                plainText = srDecrypt.ReadToEnd();
            }
            return plainText;
        }

        public static string Encrypt(string plainText, string ngCipherKeyIvPhrase)
        {
            var cipherTextArray = ngCipherKeyIvPhrase.Split("|");
            string cipherPhrase = cipherTextArray[0];
            string salt = cipherTextArray[1];

            byte[] encrypted;
            // Create an AesManaged object  
            // with the specified key and IV.  
            using (var aesAlg = new AesManaged())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.FeedbackSize = 128;

                aesAlg.Key = Encoding.UTF8.GetBytes(cipherPhrase);
                aesAlg.IV = Encoding.UTF8.GetBytes(salt);

                // Create an encryptor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes as a base64 string.
            return Convert.ToBase64String(encrypted);
        }
    }
}
