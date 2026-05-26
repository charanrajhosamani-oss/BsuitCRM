
using BSuit.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;


#nullable disable
namespace BSuit.Identity.Audit
{
    public static class AuditHelper
    {
        public static ApplicationAuditLog CreateAuditLog(EntityEntry entry, string userId, Guid? tenantId)
        {
            var audit = new ApplicationAuditLog
            {
                TableName = entry.Metadata.GetTableName(),
                UserId = userId,
                TenantId = tenantId,
                ChangedOn = DateTime.UtcNow
            };

            var keyValues = new Dictionary<string, object>();
            var oldValues = new Dictionary<string, object>();
            var newValues = new Dictionary<string, object>();
            var changedColumns = new List<string>();

            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;

                if (property.Metadata.IsPrimaryKey())
                {
                    keyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        audit.Action = "CREATE";
                        newValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        audit.Action = "DELETE";
                        oldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            audit.Action = "UPDATE";
                            changedColumns.Add(propertyName);
                            oldValues[propertyName] = property.OriginalValue;
                            newValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }

            audit.KeyValues = JsonSerializer.Serialize(keyValues);
            audit.OldValues = oldValues.Count == 0 ? null : JsonSerializer.Serialize(oldValues);
            audit.NewValues = newValues.Count == 0 ? null : JsonSerializer.Serialize(newValues);
            audit.ChangedColumns = changedColumns.Count == 0 ? null : string.Join(",", changedColumns);

            return audit;
        }
    }
}