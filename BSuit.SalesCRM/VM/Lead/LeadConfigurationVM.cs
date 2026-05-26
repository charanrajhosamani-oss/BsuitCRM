using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.SalesCRM.VM.Lead
{
    public class LeadConfigurationVM
    {
        public List<SelectListItem> Services { get; set; } = new();
        public List<SelectListItem> Regions { get; set; } = new();
        public List<SelectListItem> SalesExecutives { get; set; } = new();

        public List<Entities.LeadConfiguration> Configurations { get; set; } = new();
    }
}
