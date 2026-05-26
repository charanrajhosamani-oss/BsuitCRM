#nullable disable
namespace BSuit.API.Areas.Admin.Models.Base
{
    public class GridRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }

        public string? Search { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; } = "asc";
    }
}
