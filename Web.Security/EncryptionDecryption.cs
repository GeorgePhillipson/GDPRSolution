using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Web.Security
{
    public static class EncryptionDecryption
    {
        private const string Iv = "eav4aeR5slSbxRwF";

        private const int Keysize = 256;

        public static string EncryptString(string plainText, string passPhrase)
        {
            if (!string.IsNullOrEmpty(plainText))
            {
                byte[] initVectorBytes                  = Encoding.UTF8.GetBytes(Iv);
                byte[] plainTextBytes                   = Encoding.UTF8.GetBytes(plainText);
                Rfc2898DeriveBytes rfc2898DeriveBytes   = new Rfc2898DeriveBytes(passPhrase, initVectorBytes);
                byte[] keyBytes                         = rfc2898DeriveBytes.GetBytes(Keysize / 8);

                RijndaelManaged rijndaelManaged         = new RijndaelManaged
                {
                    Mode = CipherMode.CBC
                };
                ICryptoTransform encryptor              = rijndaelManaged.CreateEncryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream               = new MemoryStream();

                CryptoStream cryptoStream               = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] cipherTextBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();

                return Convert.ToBase64String(cipherTextBytes);
            }
            return "No Data Supplied";
        }

        public static string DecryptString(string cipherText, string passPhrase)
        {
            if (!string.IsNullOrEmpty(cipherText))
            {
                byte[] initVectorBytes                  = Encoding.UTF8.GetBytes(Iv);
                byte[] cipherTextBytes                  = Convert.FromBase64String(cipherText);
                Rfc2898DeriveBytes rfc2898DeriveBytes   = new Rfc2898DeriveBytes(passPhrase, initVectorBytes);
                byte[] keyBytes                         = rfc2898DeriveBytes.GetBytes(Keysize / 8);

                RijndaelManaged rijndaelManaged = new RijndaelManaged
                {
                    Mode = CipherMode.CBC
                };
                ICryptoTransform decryptor  = rijndaelManaged.CreateDecryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream   = new MemoryStream(cipherTextBytes);

                CryptoStream cryptoStream   = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes       = new byte[cipherTextBytes.Length];
                int decryptedByteCount      = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();

                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            }
            //Or throw an error
            return "No Data Supplied";

        }

        public static string EncryptWithNoPassPhrase(string plainText)
        {
            if (!string.IsNullOrEmpty(plainText))
            {
                byte[] data = Encoding.Unicode.GetBytes(plainText);

                CspParameters cspParameters         = new CspParameters { KeyContainerName = Iv };
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048, cspParameters))
                {
                    byte[] reBytes = rsa.Encrypt(data, false);

                    return Convert.ToBase64String(reBytes);
                }
            }
            return "No Data Supplied";
        }

    public static string DecryptWithNoPassPhrase(string cipherText)
        {
            if (!string.IsNullOrEmpty(cipherText))
            {
                byte[] data                 = Convert.FromBase64String(cipherText);
                CspParameters cspParameters = new CspParameters { KeyContainerName = Iv };

                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048, cspParameters))
                {
                    byte[] desDecrypt = rsa.Decrypt(data, false);
                    return Encoding.Unicode.GetString(desDecrypt);
                }
            }
            return "No Data Supplied";
        }

        public static string WindowsEncrypted(string text)
        {
            return Convert.ToBase64String(ProtectedData.Protect(Encoding.Unicode.GetBytes(text), null, DataProtectionScope.LocalMachine));
        }

        public static string WindowsDecrypted(string text)
        {
            return Encoding.Unicode.GetString(ProtectedData.Unprotect(Convert.FromBase64String(text), null, DataProtectionScope.LocalMachine));
        }
    }
}
