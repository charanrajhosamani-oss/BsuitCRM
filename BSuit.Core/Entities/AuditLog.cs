
#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSuit.Core.Entities
{
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        public long Id { get; set; }

        public string TableName { get; set; }
        public string Action { get; set; } // CREATE, UPDATE, DELETE

        public string? KeyValues { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }

        public string? ChangedColumns { get; set; }

        public string? UserId { get; set; }
        public Guid? TenantId { get; set; }

        public DateTime ChangedOn { get; set; } = DateTime.UtcNow;
        public string? Remarks { get; set; }

    }
}