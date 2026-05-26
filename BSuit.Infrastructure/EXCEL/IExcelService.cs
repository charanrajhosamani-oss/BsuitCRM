
using System.Data;

namespace BSuit.Infrastructure.EXCEL
{
    public interface IExcelService
    {
        byte[] ExportFromList<T>(IEnumerable<T> data, string sheetName = "Sheet1");
        byte[] ExportFromDataTable(DataTable table, string sheetName = "Sheet1");
    }
}
