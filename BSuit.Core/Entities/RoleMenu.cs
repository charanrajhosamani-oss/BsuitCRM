namespace BSuit.Core.Entities
{
    public class RoleMenu : _BASE2
    {
        public string RoleId { get; set; }

        public int MenuId { get; set; }

        public Menu Menu { get; set; }
    }
}