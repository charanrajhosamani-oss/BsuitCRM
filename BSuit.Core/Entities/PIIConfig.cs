
#nullable disable
namespace BSuit.Core.Entities
{
    public class PIIConfig:_BASE2
    {        
        public Guid TenantId { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
    }
}
