using BSuit.SalesCRM.Entities;
using BSuit.SalesCRM.VM.Lead;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Graph.Models;
using System.ComponentModel.DataAnnotations;

namespace BSuit.API.Areas.SalesCRM.Models
{

    public class AccountVM
    {
        public Guid AccountId { get; set; }
        public Guid LeadId { get; set; }


        public Guid LeadSourceId { get; set; }


        public Guid TenantId { get; set; }


        public string AccountName { get; set; }
        public string CompanyName { get; set; }

        public string CustomerId { get; set; }
        public Guid? IndustryId { get; set; }
        public Guid? GenderId { get; set; }
        public Guid? JobTitleId { get; set; }
        public string CustomerBackground { get; set; }
        // Dropdown property
        public Guid? CompanySizeId { get; set; }


        public string? CompanyRevenue { get; set; }
        public string? CompanyRanking { get; set; }
        public Guid? CustomerTypeID { get; set; }
        public string? BusinessEmail { get; set; }
        public string? BusinessPhone { get; set; }
        public string? PersonalEmail { get; set; }
        public string? PersonalPhone { get; set; }
        public string Website { get; set; }

        public string LinkedInURL { get; set; }
        public string TwitterURL { get; set; }
        public string FacebookURL { get; set; }
        public string SkypeID { get; set; }
        public string InstantMessengerId { get; set; }

        public Guid? CountryId { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }



        public string ZipCode { get; set; }
        public Guid? OwnerId { get; set; }

        public string OwnerName { get; set; }
        public string TenantName { get; set; }
        public string GenderName { get; set; }
        public string CountryName { get; set; }
        public string IndustryName { get; set; }
        public string JobTitleName { get; set; }
        public string SizeName { get; set; }
        public string CustomerTypeName { get; set; }

        public Guid? AccountCategorizationId { get; set; }
        public string AccountCategorization { get; set; }



        public string SourceName { get; set; }
        public Guid? AccountSourceId { get; set; }
        public string AccountSourceName { get; set; }


        public Guid? CustomerCategoryId { get; set; }
        public string CustomerCategory { get; set; }

        public Guid? RegionId { get; set; }
        public string RegionName { get; set; }

        public string UserName { get; set; }




        public string IsActive { get; set; }

        public string HighPotentialStatus { get; set; }
        // Referrals Details
        public string? ReferredName { get; set; }
        public string? ReferredPhone { get; set; }
        public string? ReferredEmail { get; set; }
        public string? IsReferredActive { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public Guid? ServiceId { get; set; }
        public string? OpportunityName { get; set; }



        // Dropdown lists

        public List<SelectListItem> ServicesInfo { get; set; } = new();
        public List<SelectListItem> Industries { get; set; } = new();
        public List<SelectListItem> Genders { get; set; } = new();
        public List<SelectListItem> JobTitles { get; set; } = new();
        public List<SelectListItem> Countries { get; set; } = new();
        public List<SelectListItem> CompanySizes { get; set; } = new();
        public List<SelectListItem> CustomerTypes { get; set; } = new();
        public List<SelectListItem> AccountCategory { get; set; } = new();
        public List<SelectListItem> LeadSources { get; set; } = new();

        public List<SelectListItem> AccountSource { get; set; } = new();

        public List<SelectListItem> CustomerCategories { get; set; } = new();
        public List<SelectListItem> Regions { get; set; } = new();
        public List<AccountDto> accountDtos { get; set; } = new();
        public List<ContactInfo> contactInfo { get; set; } = new();
        public List<ScopeOpportunity> opportunities { get; set; } = new();
        public List<ProjectViewModel> projectViews { get; set; } = new();
        public List<ReferralVM> referralInfo { get; set; } = new();
        public List<ServicesViewModel> servicesVM { get; set; } = new();
        public List<DocumentsVM> documentsVM { get; set; } = new();
        public List<ClientCommunication> EmailHistory { get; set; } = new();



    }

    public class AccountListVM
    {
        // 🔹 Sections (Sub-classes)
        public List<AccountDto> AccountList { get; set; } = new();
        public Dictionary<string, int> StageCounts { get; set; } = new();

        // 🔥 Pagination (for listing screen)
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string SelectedStage { get; set; }
    }



    public class AccountDto
    {



        public Guid AccountId { get; set; }
        public Guid LeadId { get; set; }
        public Guid? LeadSourceId { get; set; }
        public Guid TenantId { get; set; }

        public string AccountName { get; set; }
        public string CompanyName { get; set; }

        public string CustomerId { get; set; }
        public Guid? IndustryId { get; set; }
        public Guid? GenderId { get; set; }
        public Guid? JobTitleId { get; set; }
        public string CustomerBackground { get; set; }
        // Dropdown property
        public Guid? CompanySizeId { get; set; }


        public string? CompanyRevenue { get; set; }
        public string? CompanyRanking { get; set; }
        public Guid? CustomerTypeID { get; set; }
        public string? BusinessEmail { get; set; }
        public string? BusinessPhone { get; set; }
        public string? PersonalEmail { get; set; }
        public string? PersonalPhone { get; set; }
        public string Website { get; set; }
        public string LinkedInURL { get; set; }
        public string TwitterURL { get; set; }
        public string FacebookURL { get; set; }
        public string SkypeID { get; set; }
        public string InstantMessengerId { get; set; }

        public Guid? CountryId { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }



        public string ZipCode { get; set; }
        public Guid? OwnerId { get; set; }

        public string OwnerName { get; set; }
        public string TenantName { get; set; }
        public string GenderName { get; set; }
        public string CountryName { get; set; }
        public string IndustryName { get; set; }
        public string JobTitleName { get; set; }
        public string SizeName { get; set; }
        public string CustomerTypeName { get; set; }

        public Guid? AccountCategorizationId { get; set; }
        public string AccountCategorization { get; set; }



        public string SourceName { get; set; }
        public Guid? AccountSourceId { get; set; }
        public string AccountSourceName { get; set; }


        public Guid? CustomerCategoryId { get; set; }
        public string CustomerCategory { get; set; }

        public Guid? RegionId { get; set; }
        public string RegionName { get; set; }


        public string UserName { get; set; }


        public string IsActive { get; set; }

        public string HighPotentialStatus { get; set; }
        // Referrals Details
        public string? ReferredName { get; set; }
        public string? ReferredPhone { get; set; }
        public string? ReferredEmail { get; set; }
        public string? IsReferredActive { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;


    }

    public class ContactInfo
    {
        public Guid ContactId { get; set; }

        public string FullName { get; set; }

        public string Department { get; set; }
        public string Designation { get; set; }// ✅ FIXED

        public string Salutation { get; set; }

        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string StatusId { get; set; }



    }
    public class ScopeOpportunity
    {
        public Guid OpportunityId { get; set; }

        public Guid ServiceId { get; set; }
        public Guid TenantId { get; set; }
        public string Service { get; set; }   // ✅
                                              //
                                              // serviceIdFIXED
        public string OpportunityStatus { get; set; }
        public string OpportunityName { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string DeliveryManager { get; set; }

        public string? Scoper { get; set; }
        public DateTime? RequestedOn { get; set; }





        public string? OpportunityStage { get; set; }





        public DateTime? SubmittedOn { get; set; }




    }

    public class ProjectViewModel
    {
        public Guid? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectHours { get; set; }
        public string BalanceHours { get; set; }

        public string TransferHours { get; set; }

        public string Notes { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

    }

    public class ServicesViewModel
    {
        public Guid? ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }


        public DateTime CreatedOn { get; set; }


    }

    public class ReferralVM
    {
        public Guid ReferralId { get; set; }
        public Guid ReferrerAccountId { get; set; }
        public Guid ReferredLeadId { get; set; }
        public string ReferredName { get; set; }
        public string ReferredPhone { get; set; }
        public string ReferredEmail { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StatusId { get; set; }




    }

    public class DocumentsVM
    {
        public string? FileName { get; set; }
        public DateTime UploadedOn { get; set; }
        public string? FilePath { get; set; }

        public string? FullName { get; set; }


    }


    public class NDASignatureVM
    {
        public Guid NDAID { get; set; }

        public Guid? LeadId { get; set; }

        public Guid? AccountId { get; set; }

        public string? NDAVersionNumber { get; set; }

        public string NDANumber { get; set; }

        public bool? AcceptNonDisclosureAgreement { get; set; }

        public bool? AcceptScannedDocumentsNDA { get; set; }

        public string NDAFormat { get; set; }

        public string NDASpecialClauseForOrg { get; set; }

        public string NDASpecialClauseForInternal { get; set; }

        public DateOnly? NDAStartDate { get; set; }

        public DateOnly? NDAEndDate { get; set; }

        public DateOnly? NDARenewalDate { get; set; }

        public string NDACustomerName { get; set; }

        public string? NDALink { get; set; }

        public DateTime? ExecutedDate { get; set; }
        public string NDAStatus { get; set; }
    }


}
