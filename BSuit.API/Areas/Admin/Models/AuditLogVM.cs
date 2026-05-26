using BSuit.Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BSuit.API.Areas.Admin.Models
{
    public class AuditLogVM
    {
        public Guid? SelectedTenantId { get; set; }
        public string SelectedTableName { get; set; }
        public string SelectedAction { get; set; }

        public List<SelectListItem> Tenants { get; set; } = new();
        public List<SelectListItem> Tables { get; set; } = new();
        public List<SelectListItem> Actions { get; set; } = new();

        public List<AuditLog> Logs { get; set; } = new();


        public string UserSearch { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
