
namespace BSuit.Infrastructure.Common
{
    public interface IAppSettingsCryptoService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
