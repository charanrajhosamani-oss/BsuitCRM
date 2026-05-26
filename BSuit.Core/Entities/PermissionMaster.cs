
#nullable disable
namespace BSuit.Core.Entities
{
    public class PermissionMaster : _BASE
    {
        public int ModuleId { get; set; }   //FK for Module Master
        public string Name { get; set; }
        public int Action { get; set; }           //ENUM STORED AS INT

        public string DisplayName { get; set; }
        public string? Remarks { get; set; }


        // Navigation
        public ModuleMaster Module { get; set; }
    }
}
