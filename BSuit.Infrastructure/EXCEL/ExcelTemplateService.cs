using ClosedXML.Excel;
using System.Reflection;

namespace BSuit.Infrastructure.EXCEL
{
    public class ExcelTemplateService : IExcelTemplateService
    {
        public byte[] ExportFromTemplate<T>(IEnumerable<T> data, string templatePath)
        {
            using var workbook = new XLWorkbook(templatePath);
            var worksheet = workbook.Worksheet(1);

            var properties = typeof(T).GetProperties();

            int startRow = 5; // where data starts

            int row = startRow;

            foreach (var item in data)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var value = properties[col].GetValue(item);
                    worksheet.Cell(row, col + 1).Value = (XLCellValue)value;
                }
                row++;
            }

            // Auto adjust
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}
