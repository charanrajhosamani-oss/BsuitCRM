#nullable disable
namespace BSuit.Core.Entities
{
    public class ColumnPermission : _BASE2
    {
        public string RoleId { get; set; }
        public string? UserId { get; set; }
        public int PermissionId { get; set; } // Optional (link to screen)

        public string EntityName { get; set; }   // Employee
        public string ColumnName { get; set; }   // Salary

        public Guid TenantId { get; set; }

        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public string Remarks { get; set; }   // Salary


    }
}
