
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Reflection;

namespace BSuit.Infrastructure.PDF
{
    public class PdfService : IPdfService
    {
        public byte[] GeneratePdf<T>(IEnumerable<T> data, string title)
        {
            var items = data?.ToList() ?? new List<T>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);

                    // Header
                    page.Header()
                        .Text(title)
                        .FontSize(18)
                        .Bold()
                        .AlignCenter();

                    // Content
                    page.Content().Table(table =>
                    {
                        // Dynamic columns
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (var _ in properties)
                                columns.RelativeColumn();
                        });

                        // Header row
                        table.Header(header =>
                        {
                            foreach (var prop in properties)
                            {
                                header.Cell()
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(5)
                                    .Text(prop.Name)
                                    .Bold();
                            }
                        });

                        // Data rows
                        foreach (var item in items)
                        {
                            foreach (var prop in properties)
                            {
                                var value = prop.GetValue(item);

                                table.Cell()
                                    .Padding(5)
                                    .Text(value?.ToString() ?? "");
                            }
                        }
                    });

                    // Footer
                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated on {DateTime.Now:dd-MM-yyyy HH:mm}");
                });
            }).GeneratePdf();
        }
    }
}