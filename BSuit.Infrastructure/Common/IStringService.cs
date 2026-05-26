
namespace BSuit.Infrastructure.Common
{
    public interface IStringService
    {
        string ToTitleCase(string input);
        string ToSlug(string input);

        bool IsNullOrEmpty(string input);
        bool IsNullOrWhiteSpace(string input);

        string Truncate(string input, int length);
        string RemoveHtml(string input);

        string MaskEmail(string email);
        string MaskMobile(string mobile);

        string GenerateRandom(int length, bool includeNumbers = true);

        string Base64Encode(string plainText);
        string Base64Decode(string base64);

        bool IsValidEmail(string email);
        bool IsNumeric(string input);

        string Left(string input, int length);
        string Right(string input, int length);

        string Clean(string input);
    }
}
