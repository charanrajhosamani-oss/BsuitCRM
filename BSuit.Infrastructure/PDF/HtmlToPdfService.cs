using DinkToPdf;
using DinkToPdf.Contracts;


namespace BSuit.Infrastructure.PDF
{
    
    public class HtmlToPdfService : IHtmlToPdfService
    {
        private readonly IConverter _converter;

        public HtmlToPdfService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] GeneratePdf(string html)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Landscape // better for grids
                },
                Objects = {
                new ObjectSettings
                {
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
            };

            return _converter.Convert(doc);
        }
    }
}
