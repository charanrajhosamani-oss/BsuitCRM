using BSuit.Infrastructure.PDF;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlToOpenXml;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BSuit.Infrastructure.WORD
{
   
    public class WordService : IWordService
    {
        private readonly IViewRenderService _viewRender;
        public WordService(IViewRenderService viewRender)
        {
            _viewRender = viewRender;
        }



        // ✅ METHOD 1: HTML → Word
        public byte[] GenerateFromHtml(string html)
        {
            using var stream = new MemoryStream();

            using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());

                var body = mainPart.Document.Body;

                // VERY BASIC HTML parsing (for simple content)
                html = Regex.Replace(html, "<.*?>", string.Empty);

                body.Append(new Paragraph(new Run(new Text(html))));
            }

            return stream.ToArray();
        }

        // ✅ METHOD 2: Generic List → Table
        public byte[] GenerateFromList<T>(IEnumerable<T> data, string title)
        {
            using var stream = new MemoryStream();

            using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());

                var body = mainPart.Document.Body;

                // Title
                body.Append(new Paragraph(new Run(new Text(title)))
                {
                    ParagraphProperties = new ParagraphProperties(
                        new Justification { Val = JustificationValues.Center })
                });

                var table = new Table();

                var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // Header Row
                var headerRow = new TableRow();
                foreach (var prop in props)
                {
                    headerRow.Append(CreateCell(prop.Name, true));
                }
                table.Append(headerRow);

                // Data Rows
                foreach (var item in data)
                {
                    var row = new TableRow();

                    foreach (var prop in props)
                    {
                        var value = prop.GetValue(item)?.ToString() ?? "";
                        row.Append(CreateCell(value));
                    }

                    table.Append(row);
                }

                body.Append(table);
            }

            return stream.ToArray();
        }

        private TableCell CreateCell(string text, bool isHeader = false)
        {
            var run = new Run(new Text(text));

            if (isHeader)
                run.RunProperties = new RunProperties(new Bold());

            return new TableCell(new Paragraph(run));
        }




        public async Task<byte[]> GenerateFromViewAsync(string viewPath, object model)
        {
            // 1. Render Razor → HTML
            var html = await _viewRender.RenderToStringAsync(viewPath, model);

            using var stream = new MemoryStream();

            using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());

                // 2. Convert HTML → Word
                var converter = new HtmlConverter(mainPart);
                converter.ParseHtml(html);
            }

            return stream.ToArray();
        }
    }



}
