
namespace BSuit.Infrastructure.Common
{
    public interface IMaskingService
    {
        string MaskEmail(string email);
        string MaskMobile(string mobile);
        string MaskAadhaar(string aadhaar);
        string MaskPAN(string pan);
        string MaskCreditCard(string cardNumber);

        string MaskCustom(string input, int visibleStart, int visibleEnd, char maskChar = '*');

        string UnmaskPartial(string maskedValue, string originalValue); // controlled reveal
    }
}
