using BSuit.Infrastructure.Common;
using BSuit.Infrastructure.CSV;
using BSuit.Infrastructure.Email;
using BSuit.Infrastructure.EXCEL;
using BSuit.Infrastructure.ExternalAPIs;
using BSuit.Infrastructure.Files;
using BSuit.Infrastructure.Graphics;
using BSuit.Infrastructure.PDF;
using BSuit.Infrastructure.WORD;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace BSuit.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration config)
        {
            //services.AddHttpContextAccessor();

            services.AddScoped<SmtpEmailService>();
            services.AddScoped<GraphEmailService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IFileService, FileService>();
          
            services.AddScoped<IRequestInfoService, RequestInfoService>();

            services.AddScoped<ICsvService, CsvService>();

            services.AddScoped<IPdfService, PdfService>();//For Lists
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));//for razor pages export

            services.AddScoped<IHtmlToPdfService, HtmlToPdfService>();
            services.AddScoped<IViewRenderService, ViewRenderService>();


            services.AddScoped<IWordService, WordService>();

            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IExcelTemplateService, ExcelTemplateService>();


            services.AddScoped<IStringService, StringService>();
            services.AddScoped<IDateTimeService, DateTimeService>();

            services.AddScoped<IEncryptionService, EncryptionService>();

            services.AddScoped<IMaskingService, MaskingService>();

            services.AddScoped<IAppSettingsCryptoService, AppSettingsCryptoService>();

            services.AddScoped<IFileTransferService, FileTransferService>();

            services.AddScoped<IFilePathService, FilePathService>();

            services.AddScoped<IImageService, ImageService>();


            services.AddScoped<IApiClientService, ApiClientService>();

            services.AddHttpClient<IApiAuthClient, ApiAuthClient>();

            return services;
        }
    }
}
