using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BSuit.Infrastructure.Common
{
    public class StringService : IStringService
    {
        public string ToTitleCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        public string ToSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            input = input.ToLowerInvariant();
            input = Regex.Replace(input, @"[^a-z0-9\s-]", "");
            input = Regex.Replace(input, @"\s+", "-").Trim('-');

            return input;
        }

        public bool IsNullOrEmpty(string input)
            => string.IsNullOrEmpty(input);

        public bool IsNullOrWhiteSpace(string input)
            => string.IsNullOrWhiteSpace(input);

        public string Truncate(string input, int length)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return input.Length <= length ? input : input.Substring(0, length) + "...";
        }

        public string RemoveHtml(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        public string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return email;

            var parts = email.Split('@');
            if (parts.Length != 2) return email;

            var name = parts[0];
            var domain = parts[1];

            var masked = name.Length <= 2
                ? name[0] + "*"
                : name.Substring(0, 2) + new string('*', name.Length - 2);

            return $"{masked}@{domain}";
        }

        public string MaskMobile(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile) || mobile.Length < 4)
                return mobile;

            return new string('*', mobile.Length - 4) + mobile[^4..];
        }

        public string GenerateRandom(int length, bool includeNumbers = true)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            const string nums = "0123456789";

            var pool = includeNumbers ? chars + nums : chars;

            var rnd = new Random();

            return new string(Enumerable.Range(0, length)
                .Select(_ => pool[rnd.Next(pool.Length)]).ToArray());
        }

        public string Base64Encode(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            var bytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }

        public string Base64Decode(string base64)
        {
            if (string.IsNullOrEmpty(base64)) return base64;

            var bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);
        }

        public bool IsNumeric(string input)
        {
            return double.TryParse(input, out _);
        }

        public string Left(string input, int length)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return input.Length <= length ? input : input.Substring(0, length);
        }

        public string Right(string input, int length)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return input.Length <= length ? input : input.Substring(input.Length - length);
        }

        public string Clean(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            input = input.Trim();
            input = Regex.Replace(input, @"\s+", " ");

            return input;
        }

    }
}
