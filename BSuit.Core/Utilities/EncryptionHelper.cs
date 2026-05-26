using System.Security.Cryptography;
using System.Text;


namespace BSuit.Core.Utilities
{
    public static class EncryptionHelper
    {
        private static readonly string key =
            "Our1VeryStrong32CharacterSecretKey!!";

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using Aes aes = Aes.Create();

            aes.Key = Encoding.UTF8.GetBytes(key[..32]);
            aes.GenerateIV();

            var iv = aes.IV;

            using var encryptor = aes.CreateEncryptor();

            var plainBytes = Encoding.UTF8.GetBytes(plainText);

            var encryptedBytes = encryptor
                .TransformFinalBlock(
                    plainBytes,
                    0,
                    plainBytes.Length);

            return Convert.ToBase64String(
                iv.Concat(encryptedBytes).ToArray());
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            var fullCipher = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();

            aes.Key = Encoding.UTF8.GetBytes(key[..32]);

            var iv = fullCipher.Take(16).ToArray();
            var cipher = fullCipher.Skip(16).ToArray();

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();

            var decrypted = decryptor
                .TransformFinalBlock(
                    cipher,
                    0,
                    cipher.Length);

            return Encoding.UTF8.GetString(decrypted);
        }


        public static bool IsEncrypted(string value)
        {
            try
            {
                Convert.FromBase64String(value);
                return true;
            }
            catch
            {
                return false;
            }
        }



    }
}
