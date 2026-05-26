namespace BSuit.Core.Entities
{
    public class Menu : _BASE
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public int? ParentId { get; set; }

        public int SortOrder { get; set; }
    }
}