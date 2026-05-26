namespace BSuit.Core.Entities
{
    public class EncryptionKey:_BASE2
    {
        public Guid TenantId { get; set; }
        public int Version { get; set; }
        public string EncryptedKey { get; set; }
    }
}
