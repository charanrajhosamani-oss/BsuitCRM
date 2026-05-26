namespace BSuit.API.Models
{
    public class MenuItemVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public int? ParentId { get; set; }

        public int SortOrder { get; set; }

        public List<MenuItemVM> Children { get; set; } = new();

        // NEW
        public bool IsActive { get; set; }
        public bool HasActiveChild { get; set; }
    }
}
