using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class AccountReportTbl
{
    public string? AccountId { get; set; }

    public Guid? LeadId { get; set; }

    public Guid? TenantId { get; set; }

    public Guid? LeadSourceId { get; set; }

    public Guid? OwnerId { get; set; }

    public string? AccountOwner { get; set; }

    public string? CustomerId { get; set; }

    public string? CompanyName { get; set; }

    public Guid? JobTitleId { get; set; }

    public string? JobTitle { get; set; }

    public string? AccountName { get; set; }

    public string? BusinessEmail { get; set; }

    public string? BusinessPhone { get; set; }

    public string? PersonalEmail { get; set; }

    public string? PersonalPhone { get; set; }

    public string? Unsubscribe { get; set; }

    public string? CustomerType { get; set; }

    public Guid? CustomerTypeId { get; set; }

    public string? CustomerBackground { get; set; }

    public Guid? AccountCategorizationId { get; set; }

    public string? AccountCategorization { get; set; }

    public Guid? CustomerCategoryId { get; set; }

    public string? CustomerCategory { get; set; }

    public string? CompanyIndustry { get; set; }

    public Guid? CompanySizeId { get; set; }

    public Guid? RegionId { get; set; }

    public string? Region { get; set; }

    public Guid? IndustryId { get; set; }

    public string? CompanySize { get; set; }

    public string? CompanyRevenue { get; set; }

    public bool? HighPotentiaStatus { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public Guid? CountryId { get; set; }

    public string? Country { get; set; }

    public string? ZipCode { get; set; }

    public string? Website { get; set; }

    public string? CompanyRanking { get; set; }

    public string? CompanyURL { get; set; }

    public string? LinkedInURL { get; set; }

    public string? Twitter_URL { get; set; }

    public string? FacebookURL { get; set; }

    public string? SkypeID { get; set; }

    public string? InstantMessengerId { get; set; }

    public string? ReferrerClientid { get; set; }

    public string? ReferrerClientName { get; set; }

    public string? FortuneRanking { get; set; }

    public Guid? CreatedBy { get; set; }

    public string? Created_By { get; set; }

    public string? CreatedOn { get; set; }
}
