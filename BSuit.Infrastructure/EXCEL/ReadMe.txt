dotnet add package ClosedXML

🔥 Advanced Improvements (Recommended)
✅ 1. Custom Column Names
[Display(Name = "User Name")]
public string Name { get; set; }
Use:
var name = prop.GetCustomAttribute<DisplayAttribute>()?.Name ?? prop.Name;

✅ 2. Ignore Columns
[ExcelIgnore]
public string InternalId { get; set; }

✅ 3. Formatting (Date / Currency)
if (value is DateTime dt)
    worksheet.Cell(row, col + 1).Value = dt.ToString("dd-MM-yyyy");

✅ 4. Add Filters
worksheet.RangeUsed().SetAutoFilter();

✅ 5. Freeze Header Row
worksheet.SheetView.FreezeRows(1);


public IActionResult ExportUsers()
{
    var users = new List<UserDto>
    {
        new() { Name = "Maxi", Email = "maxi@mail.com", Age = 30 },
        new() { Name = "John", Email = "john@mail.com", Age = 28 }
    };

    var file = _excelService.ExportFromList(users, "Users");

    return File(file,
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "Users.xlsx");
}




public IActionResult ExportDataTable()
{
    var table = new DataTable();
    table.Columns.Add("Name");
    table.Columns.Add("Email");

    table.Rows.Add("Maxi", "maxi@mail.com");
    table.Rows.Add("John", "john@mail.com");

    var file = _excelService.ExportFromDataTable(table, "Data");

    return File(file,
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "Data.xlsx");
}






services.AddScoped<IExcelTemplateService, ExcelTemplateService>();
Add Logo Dynamically (Optional)
var imagePath = Path.Combine("wwwroot", "logo.png");
worksheet.AddPicture(imagePath)
    .MoveTo(worksheet.Cell("A1"))
    .Scale(0.5);

Apply Styling Programmatically
var headerRange = worksheet.Range("A4:C4");
headerRange.Style.Font.Bold = true;
headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

public IActionResult ExportUsers()
{
    var users = new List<UserDto>
    {
        new() { Name = "Maxi", Email = "maxi@mail.com", Age = 30 },
        new() { Name = "John", Email = "john@mail.com", Age = 28 }
    };

    var templatePath = Path.Combine(
        Directory.GetCurrentDirectory(),
        "Templates",
        "Reports",
        "UserTemplate.xlsx");

    var file = _excelTemplateService.ExportFromTemplate(users, templatePath);

    return File(file,
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "Users.xlsx");
}