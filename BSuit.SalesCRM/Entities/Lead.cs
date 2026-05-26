using System;
using System.Collections.Generic;

namespace BSuit.SalesCRM.Entities;

public partial class Lead
{
    public Guid LeadId { get; set; }

    public Guid? TenantId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? EnquiryId { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? CompanyName { get; set; }

    public string? City { get; set; }

    public string? ZipCode { get; set; }

    public string? Website { get; set; }

    public Guid? CountryId { get; set; }

    public string? PersonalEmail1 { get; set; }

    public string? PersonalEmail2 { get; set; }

    public string? Address { get; set; }

    public string? RequirementDetails { get; set; }

    public string? SkpeId { get; set; }

    public string? TwitterURL { get; set; }

    public string? FacebookURL { get; set; }

    public string? LinkedinURL { get; set; }

    public string? BDComments { get; set; }

    public string? PreferredModeofCommunication { get; set; }

    public string? CustomerBackground { get; set; }

    public Guid? CustomerTypeId { get; set; }

    public Guid? CompanySizeId { get; set; }

    public string? CompanyRevenue { get; set; }

    public string? CompanyRanking { get; set; }

    public Guid? RatingId { get; set; }

    public Guid? LeadPriorityId { get; set; }

    public Guid? GenderId { get; set; }

    public Guid? JobTitleId { get; set; }

    public Guid? IndustryId { get; set; }

    public Guid? LeadTypeId { get; set; }

    public Guid? LeadSourceId { get; set; }

    public Guid? LeadStageId { get; set; }

    public Guid? OwnerId { get; set; }

    public bool? IsActive { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual CompanySize? CompanySize { get; set; }

    public virtual Country? Country { get; set; }

    public virtual GenderMaster? Gender { get; set; }

    public virtual Industry? Industry { get; set; }

    public virtual JobTitle? JobTitle { get; set; }

    public virtual ICollection<LeadAssignment> LeadAssignments { get; set; } = new List<LeadAssignment>();

    public virtual ICollection<LeadAttachment> LeadAttachments { get; set; } = new List<LeadAttachment>();

    public virtual ICollection<LeadNote> LeadNotes { get; set; } = new List<LeadNote>();

    public virtual Priority? LeadPriority { get; set; }

    public virtual ICollection<LeadServiceMapping> LeadServiceMappings { get; set; } = new List<LeadServiceMapping>();

    public virtual LeadSource? LeadSource { get; set; }

    public virtual LeadStage? LeadStage { get; set; }

    public virtual LeadType? LeadType { get; set; }

    public virtual Rating? Rating { get; set; }
}
