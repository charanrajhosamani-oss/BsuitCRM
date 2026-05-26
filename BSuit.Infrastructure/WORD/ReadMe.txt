DocumentFormat.OpenXml

public class ExportController : Controller
{
    private readonly IWordService _word;

    public ExportController(IWordService word)
    {
        _word = word;
    }

    public IActionResult FromHtml()
    {
        var html = "<h1>Hello</h1><p>This is Word export</p>";

        var file = _word.GenerateFromHtml(html);

        return File(file,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "html.docx");
    }

    public IActionResult FromList()
    {
        var data = new List<UserDto>
        {
            new() { Name = "Maxi", Email = "maxi@mail.com" }
        };

        var file = _word.GenerateFromList(data, "User Report");

        return File(file,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "users.docx");
    }
}




If You Need Proper HTML Rendering
dotnet add package OpenXmlPowerTools
var converter = new HtmlConverter(mainPart);
converter.ParseHtml(html);


public class ReportController : Controller
{
    private readonly IWordService _word;

    public ReportController(IWordService word)
    {
        _word = word;
    }

    public async Task<IActionResult> ExportWord()
    {
        var data = new List<UserDto>
        {
            new() { Name = "Maxi", Email = "maxi@mail.com", Age = 30 }
        };

        var file = await _word.GenerateFromViewAsync(
            "~/Views/Shared/_GridWord.cshtml", data);

        return File(file,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "Users.docx");
    }
}