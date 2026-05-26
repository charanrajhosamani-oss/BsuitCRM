
#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSuit.Core.Entities
{
    public class LogEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        public long Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string Level { get; set; } = null!; // Information, Error, Warning

        public string? Message { get; set; }

        public string? MessageTemplate { get; set; }

        public string? Exception { get; set; }

        public string? Properties { get; set; } // JSON (UserId, TenantId, etc.)

        public string? LogEvent { get; set; } // Full raw JSON

        public string? SourceContext { get; set; } // Class/Namespace

        public string? MachineName { get; set; }

        public string? RequestPath { get; set; }

        public string? ActionName { get; set; }

        public string? UserId { get; set; }

        public Guid? TenantId { get; set; }

        public string? CorrelationId { get; set; }
        public string? Remarks { get; set; }
    }
}
