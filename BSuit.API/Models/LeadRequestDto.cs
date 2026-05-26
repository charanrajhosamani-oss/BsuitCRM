using System;
using System.Collections.Generic;

namespace BSuit.API.Models
{
    public class LeadRequestDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? CompanyName { get; set; }
        public string? City { get; set; }
        public string? Website { get; set; }
        public string? RequirementDetails { get; set; }

        public string? CountryName { get; set; }
        public string? IndustryName { get; set; }
        public string? LeadSourceName { get; set; }

        public List<LeadServiceDto>? Services { get; set; }
    }

    public class LeadServiceDto
    {
        public string? ServiceName { get; set; }
        public string? SubServiceName { get; set; }
        public string? Notes { get; set; }
    }
}