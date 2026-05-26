
namespace BSuit.Infrastructure.Common
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);

        string HashSHA256(string input);
        string HashSHA512(string input);

        string GenerateSalt(int size = 16);
        string HashPassword(string password, string salt);
        bool VerifyPassword(string password, string hash, string salt);

        string Base64Encode(string input);
        string Base64Decode(string input);
    }
}
