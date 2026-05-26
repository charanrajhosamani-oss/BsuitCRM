
namespace BSuit.Core.Entities
{
    public class BusinessMenuMaster:_BASE
    {
        public Guid TenantId { get; set; }
        public int ModuleId { get; set; }
        public int? ParentMenuId { get; set; }

        public string MenuName { get; set; }       

        public string Url { get; set; }

        public string Icon { get; set; }

        public int SortOrder { get; set; }
        public string Remarks { get; set; }

        public virtual ModuleMaster Module { get; set; }
        public virtual BusinessMenuMaster ParentMenu { get; set; }

        public virtual ICollection<BusinessMenuMaster> ChildMenus { get; set; }
    }
}
