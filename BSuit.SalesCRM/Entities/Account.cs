using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Account
{
    public Guid AccountId { get; set; }

    public Guid? LeadId { get; set; }

    public Guid? LeadSourceId { get; set; }

    public string AccountName { get; set; } = null!;

    public string? CompanyName { get; set; }

    public string? CustomerId { get; set; }

    public Guid? IndustryId { get; set; }

    public Guid? GenderId { get; set; }

    public Guid? JobTitleId { get; set; }

    public string? CustomerBackground { get; set; }

    public Guid? CompanySizeId { get; set; }

    public string? CompanyRevenue { get; set; }

    public string? CompanyRanking { get; set; }

    public Guid? CustomerTypeID { get; set; }

    public string? BusinessEmail { get; set; }

    public string? BusinessPhone { get; set; }

    public string? PersonalEmail { get; set; }

    public string? PersonalPhone { get; set; }

    public string? Unsubscribe { get; set; }

    public Guid? AccountCategorizationId { get; set; }

    public Guid? AccountSourceId { get; set; }

    public string? Website { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public Guid? CountryId { get; set; }

    public string? ZipCode { get; set; }

    public Guid OwnerId { get; set; }

    public Guid? TenantId { get; set; }

    public Guid? CustomerCategoryId { get; set; }

    public Guid? RegionId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public bool? HighPotentiaStatus { get; set; }

    public string? CompanyURL { get; set; }

    public string? LinkedInURL { get; set; }

    public string? TwitterURL { get; set; }

    public string? FacebookURL { get; set; }

    public string? SkypeID { get; set; }

    public string? InstantMessengerId { get; set; }

    public string? ReferrerClientid { get; set; }

    public string? ReferrerClientName { get; set; }

    public string? FortuneRanking { get; set; }

    public virtual CompanySize? CompanySize { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual Country? Country { get; set; }

    public virtual CustomerType? CustomerType { get; set; }

    public virtual GenderMaster? Gender { get; set; }

    public virtual Industry? Industry { get; set; }

    public virtual JobTitle? JobTitle { get; set; }

    public virtual ICollection<Referral> Referrals { get; set; } = new List<Referral>();
}
