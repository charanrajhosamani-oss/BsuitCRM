public class ReportController : Controller
{
    private readonly IViewRenderService _viewRender;
    private readonly IHtmlToPdfService _pdf;

    public ReportController(IViewRenderService viewRender, IHtmlToPdfService pdf)
    {
        _viewRender = viewRender;
        _pdf = pdf;
    }

    public async Task<IActionResult> Export()
    {
        var data = GetUsers(); // your DB data

        var html = await _viewRender.RenderToStringAsync(
            "~/Views/Shared/_GridPdf.cshtml", data);

        var pdfBytes = _pdf.GeneratePdf(html);

        return File(pdfBytes, "application/pdf", "GridReport.pdf");
    }
}




Place Native Library in Project
BSuit.API
 └── wkhtmltox
      └── wkhtmltox.dll



👉 Set file property:
Copy to Output Directory = Copy if newer


Create Native Loader (IMPORTANT 🔥)
using System.Runtime.InteropServices;
public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        return LoadUnmanagedDll(absolutePath);
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        return LoadUnmanagedDllFromPath(unmanagedDllName);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        return null;
    }
}



BEFORE BUILD:
var context = new CustomAssemblyLoadContext();

var wkhtmlPath = Path.Combine(Directory.GetCurrentDirectory(),
    "wkhtmltox", "wkhtmltox.dll");

context.LoadUnmanagedLibrary(wkhtmlPath);






