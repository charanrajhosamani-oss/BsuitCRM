
namespace BSuit.Infrastructure.PDF
{
    public interface IPdfService
    {
        byte[] GeneratePdf<T>(IEnumerable<T> data, string title);
    }
}
