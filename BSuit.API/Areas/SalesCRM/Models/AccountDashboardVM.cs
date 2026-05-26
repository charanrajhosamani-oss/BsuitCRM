namespace BSuit.API.Areas.SalesCRM.Models
{
    public class AccountDashboardVM
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
