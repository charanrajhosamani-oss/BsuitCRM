using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.SalesCRM.VM.Lead
{
    public class LeadEditVM
    {
        // ============================================
        // IDENTIFIERS
        // ============================================

        public Guid LeadId { get; set; }

        public Guid TeanantId { get; set; }

        // ============================================
        // BASIC INFO
        // ============================================

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? EnquiryId { get; set; }

        public Guid? Gender { get; set; }

        public Guid? JobTitle { get; set; }

        // ============================================
        // CONTACT INFO
        // ============================================

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? PersonalEmail1 { get; set; }

        public string? PersonalEmail2 { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? ZipCode { get; set; }

        public Guid? Country { get; set; }

        // ============================================
        // COMPANY INFO
        // ============================================

        public string? CompanyName { get; set; }

        public string? Website { get; set; }

        public Guid? CompanySize { get; set; }

        public string? CompanyRevenue { get; set; }

        public string? CompanyRanking { get; set; }

        public Guid? Industry { get; set; }

        // ============================================
        // LEAD INFO
        // ============================================

        public Guid? LeadType { get; set; }

        public Guid? LeadSource { get; set; }

        public Guid? LeadStage { get; set; }

        public Guid? LeadPriority { get; set; }

        public Guid? Rating { get; set; }

        public Guid? SalesExecutive { get; set; }

        // ============================================
        // ADDITIONAL INFO
        // ============================================

        public string? RequirementDetails { get; set; }

        public string? CustomerBackground { get; set; }

        public string? SkpeId { get; set; }

        public string? TwitterURL { get; set; }

        public string? FacebookURL { get; set; }

        // ============================================
        // NEW MISSING FIELDS
        // ============================================

        public string? LinkedinURL { get; set; }

        public string? BDComments { get; set; }

        public string? PreferredModeofCommunication { get; set; }

        public Guid? CustomerType { get; set; }

        // ============================================
        // AUDIT FIELDS
        // ============================================

        public Guid? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // ============================================
        // STATUS
        // ============================================

        public bool IsActive { get; set; }

        // ============================================
        // DROPDOWN LISTS
        // ============================================

        public List<SelectListItem>
            GenderList
        { get; set; } = new();

        public List<SelectListItem>
            IndustryList
        { get; set; } = new();

        public List<SelectListItem>
            CompanySizeList
        { get; set; } = new();

        public List<SelectListItem>
            LeadTypeList
        { get; set; } = new();

        public List<SelectListItem>
            LeadSourceList
        { get; set; } = new();

        public List<SelectListItem>
            LeadStageList
        { get; set; } = new();

        public List<SelectListItem>
            LeadPriorityList
        { get; set; } = new();

        public List<SelectListItem>
            RatingList
        { get; set; } = new();

        public List<SelectListItem>
            SalesExecutiveList
        { get; set; } = new();

        public List<SelectListItem>
            CountryMaster
        { get; set; } = new();

        // ============================================
        // NEW DROPDOWNS
        // ============================================

        public List<SelectListItem>
            JobTitleList
        { get; set; } = new();

        public List<SelectListItem>
            CustomerTypeList
        { get; set; } = new();

        public List<SelectListItem>
            PreferredCommunicationList
        { get; set; } = new();

        // ============================================
        // SERVICES
        // ============================================

        public List<LeadServiceList>
            SelectedServices
        { get; set; } = new();
    }
}
