using Microsoft.AspNetCore.Mvc.Rendering;

namespace BSuit.API.Areas.Admin.Models
{
    public class AnalyticsVM
    {
        public Guid? SelectedTenantId { get; set; }
        public string SelectedTableName { get; set; }

        public List<SelectListItem> Tenants { get; set; } = new();
        public List<SelectListItem> Tables { get; set; } = new();

        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
        public int DuplicateCount { get; set; }

        public int TodayCount { get; set; }
        public int Last7DaysCount { get; set; }
        public int Last1MonthCount { get; set; }
        public int Last1YearCount { get; set; }


        // Chart Data
        public List<string> ChartLabels { get; set; } = new();
        public List<int> ChartValues { get; set; } = new();
    }
}
