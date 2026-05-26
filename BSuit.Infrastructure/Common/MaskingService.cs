using System.Text.RegularExpressions;

namespace BSuit.Infrastructure.Common
{
    public class MaskingService : IMaskingService
    {
        public string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return email;

            var parts = email.Split('@');
            if (parts.Length != 2) return email;

            var name = parts[0];
            var domain = parts[1];

            if (name.Length <= 2)
                return $"{name[0]}*@{domain}";

            var masked = name.Substring(0, 2) + new string('*', name.Length - 2);
            return $"{masked}@{domain}";
        }

        public string MaskMobile(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return mobile;

            mobile = Regex.Replace(mobile, @"\D", ""); // digits only

            if (mobile.Length < 4) return mobile;

            return new string('*', mobile.Length - 4) + mobile[^4..];
        }

        public string MaskAadhaar(string aadhaar)
        {
            if (string.IsNullOrWhiteSpace(aadhaar)) return aadhaar;

            aadhaar = Regex.Replace(aadhaar, @"\D", "");

            if (aadhaar.Length != 12) return aadhaar;

            return "XXXX XXXX " + aadhaar[^4..];
        }

        public string MaskPAN(string pan)
        {
            if (string.IsNullOrWhiteSpace(pan)) return pan;

            pan = pan.ToUpper();

            if (pan.Length != 10) return pan;

            return pan.Substring(0, 3) + "XXXX" + pan.Substring(7);
        }

        public string MaskCreditCard(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber)) return cardNumber;

            cardNumber = Regex.Replace(cardNumber, @"\D", "");

            if (cardNumber.Length < 4) return cardNumber;

            return new string('*', cardNumber.Length - 4) + cardNumber[^4..];
        }

        // 🔥 Generic Masking
        public string MaskCustom(string input, int visibleStart, int visibleEnd, char maskChar = '*')
        {
            if (string.IsNullOrEmpty(input)) return input;

            if (input.Length <= visibleStart + visibleEnd)
                return input;

            var start = input.Substring(0, visibleStart);
            var end = input.Substring(input.Length - visibleEnd);

            var masked = new string(maskChar, input.Length - (visibleStart + visibleEnd));

            return start + masked + end;
        }

        // ⚠️ Controlled unmask (requires original value)
        public string UnmaskPartial(string maskedValue, string originalValue)
        {
            // NEVER reconstruct from masked alone
            return originalValue;
        }
    }
}
