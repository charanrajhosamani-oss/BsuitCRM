
namespace BSuit.Identity
{
    public static class AppRoles
    {
        public const string SUPERADMIN = "SUPERADMIN";
        public const string ADMIN = "ADMIN";
        public const string TENANT = "TENANT";
        
        public static List<string> All => new()
        {
            SUPERADMIN, ADMIN, TENANT
        };        
    }
}
