using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace BSuit.Infrastructure.Common
{
    public class EncryptionService : IEncryptionService
    {
        private readonly string _key;

        public EncryptionService(IConfiguration config)
        {
            _key = config["Encryption:Key"]; // MUST be 32 chars (256-bit)
        }

        // ✅ AES Encryption
        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();

            aes.Key = Encoding.UTF8.GetBytes(_key);
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();

            aes.Key = Encoding.UTF8.GetBytes(_key);

            var iv = new byte[16];
            Array.Copy(fullCipher, iv, iv.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }

        // ✅ Hashing
        public string HashSHA256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }

        public string HashSHA512(string input)
        {
            using var sha = SHA512.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }

        // ✅ Salt
        public string GenerateSalt(int size = 16)
        {
            var buffer = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buffer);

            return Convert.ToBase64String(buffer);
        }

        // ✅ Password Hash (PBKDF2)
        public string HashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                10000,
                HashAlgorithmName.SHA256);

            return Convert.ToBase64String(pbkdf2.GetBytes(32));
        }

        public bool VerifyPassword(string password, string hash, string salt)
        {
            var newHash = HashPassword(password, salt);
            return newHash == hash;
        }

        // ✅ Base64
        public string Base64Encode(string input)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public string Base64Decode(string input)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(input));
        }
    }
}
