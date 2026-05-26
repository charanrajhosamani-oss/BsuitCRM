using BSuit.API.Areas.Admin.Models.Base;
using BSuit.Core.Entities;

#nullable disable
public class MenuVM: GridRequest
{
    public List<Menu> Items { get; set; }
}