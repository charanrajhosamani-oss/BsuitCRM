using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.SalesCRM.VM.Lead
{
    public class LeadDashboardVM
    {
        // ============================================
        // LABEL + VALUE
        // ============================================

        public List<(string Label, int Value)>
            LeadSources
        { get; set; } = new();

        public List<(string Label, int Value)>
            MonthlyTrend
        { get; set; } = new();

        public List<(string Label, int Value)>
            LeadsByOwner
        { get; set; } = new();

        public List<(string Label, int Value)>
            CustomerTypes
        { get; set; } = new();

        public List<(string Label, int Value)>
            CountryWiseLeads
        { get; set; } = new();

        public List<(string Label, int Value)>
            ConversionRates
        { get; set; } = new();
    }
}
