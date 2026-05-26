using System.Diagnostics;

namespace BSuit.API.Areas.Admin.Models
{
    public class DashboardVM
    {
        public int TotalUsers { get; set; }
        public int TotalRoles { get; set; }
        public int TotalMenus { get; set; }
        public int TotalTenants { get; set; }

        public List<ChartItemVM> WeeklyUsers { get; set; } = new();


       
        public int ActiveSubscriptions { get; set; }
        public int TotalAssignedModules { get; set; }
        public int TotalUserPermissions { get; set; }
        public int TotalMappedServices { get; set; }

        public List<ActivityVM> RecentActivities { get; set; }
        public List<ChartItemVM> RolesWithUsers { get; set; }
    }

    public class ChartItemVM
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }
    public class ActivityVM
    {       
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Type { get; set; } // Login / Create / Update / Delete
        public string UserName { get; set; }

    }

}
