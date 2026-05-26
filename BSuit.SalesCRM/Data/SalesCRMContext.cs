using System;
using System.Collections.Generic;
using BSuit.SalesCRM.Entities;
using Microsoft.EntityFrameworkCore;

namespace BSuit.SalesCRM.Data;

public partial class SalesCRMContext : DbContext
{
    public SalesCRMContext(DbContextOptions<SalesCRMContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountCategorization> AccountCategorizations { get; set; }

    public virtual DbSet<AccountReportTbl> AccountReportTbls { get; set; }

    public virtual DbSet<AccountReport_13_May_2026> AccountReport_13_May_2026s { get; set; }

    public virtual DbSet<AccountReport_22_May_2026> AccountReport_22_May_2026s { get; set; }

    public virtual DbSet<AccountSource> AccountSources { get; set; }

    public virtual DbSet<ActivityType> ActivityTypes { get; set; }

    public virtual DbSet<ApprovalTransaction> ApprovalTransactions { get; set; }

    public virtual DbSet<ApprovalWorkflow> ApprovalWorkflows { get; set; }

    public virtual DbSet<ApprovalWorkflowLevel> ApprovalWorkflowLevels { get; set; }

    public virtual DbSet<ApprovalWorkflowLevelUser> ApprovalWorkflowLevelUsers { get; set; }

    public virtual DbSet<ApprovalWorkflowMaster> ApprovalWorkflowMasters { get; set; }

    public virtual DbSet<ApprovalWorkflowRole> ApprovalWorkflowRoles { get; set; }

    public virtual DbSet<ApprovalWorkflowService> ApprovalWorkflowServices { get; set; }

    public virtual DbSet<CompanySize> CompanySizes { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<CustomerCategory> CustomerCategories { get; set; }

    public virtual DbSet<CustomerType> CustomerTypes { get; set; }

    public virtual DbSet<DealType> DealTypes { get; set; }

    public virtual DbSet<DocumentRepository> DocumentRepositories { get; set; }

    public virtual DbSet<EmailAttachment> EmailAttachments { get; set; }

    public virtual DbSet<EmailMessage> EmailMessages { get; set; }

    public virtual DbSet<EmailRecipient> EmailRecipients { get; set; }

    public virtual DbSet<EngagementModel> EngagementModels { get; set; }

    public virtual DbSet<Field> Fields { get; set; }

    public virtual DbSet<FieldRoleAccess> FieldRoleAccesses { get; set; }

    public virtual DbSet<Form> Forms { get; set; }

    public virtual DbSet<GenderMaster> GenderMasters { get; set; }

    public virtual DbSet<Industry> Industries { get; set; }

    public virtual DbSet<JobTitle> JobTitles { get; set; }

    public virtual DbSet<Lead> Leads { get; set; }

    public virtual DbSet<LeadActivity> LeadActivities { get; set; }

    public virtual DbSet<LeadAssignment> LeadAssignments { get; set; }

    public virtual DbSet<LeadAttachment> LeadAttachments { get; set; }

    public virtual DbSet<LeadConfiguration> LeadConfigurations { get; set; }

    public virtual DbSet<LeadNote> LeadNotes { get; set; }

    public virtual DbSet<LeadRejection> LeadRejections { get; set; }

    public virtual DbSet<LeadServiceMapping> LeadServiceMappings { get; set; }

    public virtual DbSet<LeadSource> LeadSources { get; set; }

    public virtual DbSet<LeadStage> LeadStages { get; set; }

    public virtual DbSet<LeadType> LeadTypes { get; set; }

    public virtual DbSet<Lead_Report_Sample> Lead_Report_Samples { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<NDASignature> NDASignatures { get; set; }

    public virtual DbSet<Opportunity> Opportunities { get; set; }

    public virtual DbSet<OpportunityModule> OpportunityModules { get; set; }

    public virtual DbSet<OpportunityService> OpportunityServices { get; set; }

    public virtual DbSet<OpportunityStage> OpportunityStages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMode> PaymentModes { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<PaymentType> PaymentTypes { get; set; }

    public virtual DbSet<Priority> Priorities { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectModule> ProjectModules { get; set; }

    public virtual DbSet<ProjectMoudleEmployeemap> ProjectMoudleEmployeemaps { get; set; }

    public virtual DbSet<ProjectStatus> ProjectStatuses { get; set; }

    public virtual DbSet<ProjectTask> ProjectTasks { get; set; }

    public virtual DbSet<ProjectTaskPriority> ProjectTaskPriorities { get; set; }

    public virtual DbSet<ProjectTaskStatus> ProjectTaskStatuses { get; set; }

    public virtual DbSet<ProjectTimeLog> ProjectTimeLogs { get; set; }

    public virtual DbSet<Proposal> Proposals { get; set; }

    public virtual DbSet<ProposalActivity> ProposalActivities { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Referral> Referrals { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<RejectReasonMaster> RejectReasonMasters { get; set; }

    public virtual DbSet<SalutationMaster> SalutationMasters { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServicePricing> ServicePricings { get; set; }

    public virtual DbSet<ServiceUserRoleMap> ServiceUserRoleMaps { get; set; }

    public virtual DbSet<WinLossReason> WinLossReasons { get; set; }

    public virtual DbSet<WorkOrder> WorkOrders { get; set; }

    public virtual DbSet<WorkOrderActivity> WorkOrderActivities { get; set; }

    public virtual DbSet<WorkflowStep> WorkflowSteps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_CI_AI");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA5A64E94F4C9");

            entity.ToTable("Account", "CRM");

            entity.HasIndex(e => e.OwnerId, "IX_Account");

            entity.Property(e => e.AccountId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AccountName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.BusinessEmail).HasMaxLength(500);
            entity.Property(e => e.BusinessPhone).HasMaxLength(500);
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.CompanyRanking).HasMaxLength(200);
            entity.Property(e => e.CompanyRevenue).HasMaxLength(500);
            entity.Property(e => e.CompanyURL)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerBackground).HasMaxLength(500);
            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FacebookURL)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FortuneRanking)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.InstantMessengerId)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(false);
            entity.Property(e => e.LinkedInURL)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.PersonalEmail).HasMaxLength(500);
            entity.Property(e => e.PersonalPhone).HasMaxLength(500);
            entity.Property(e => e.ReferrerClientName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ReferrerClientid)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.SkypeID)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TwitterURL)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Unsubscribe)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Website)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ZipCode).HasMaxLength(100);

            entity.HasOne(d => d.CompanySize).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.CompanySizeId)
                .HasConstraintName("FK_Account_CompanySize");

            entity.HasOne(d => d.Country).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("FK_Account_Countries");

            entity.HasOne(d => d.CustomerType).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.CustomerTypeID)
                .HasConstraintName("FK_Account_CustomerTypes");

            entity.HasOne(d => d.Gender).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("FK_Account_GenderMaster");

            entity.HasOne(d => d.Industry).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.IndustryId)
                .HasConstraintName("FK_Account_Industries");

            entity.HasOne(d => d.JobTitle).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.JobTitleId)
                .HasConstraintName("FK_Account_JobTitles");
        });

        modelBuilder.Entity<AccountCategorization>(entity =>
        {
            entity.HasKey(e => e.AccountCategorizationId).HasName("PK_CustomerCategory");

            entity.ToTable("AccountCategorization", "CRM");

            entity.Property(e => e.AccountCategorizationId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AccountCategorization1)
                .HasMaxLength(200)
                .HasColumnName("AccountCategorization");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(300);
        });

        modelBuilder.Entity<AccountReportTbl>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AccountReportTbl", "CRM");

            entity.Property(e => e.AccountCategorization).IsUnicode(false);
            entity.Property(e => e.AccountId).IsUnicode(false);
            entity.Property(e => e.AccountName).IsUnicode(false);
            entity.Property(e => e.AccountOwner).IsUnicode(false);
            entity.Property(e => e.Address).IsUnicode(false);
            entity.Property(e => e.BusinessEmail).IsUnicode(false);
            entity.Property(e => e.BusinessPhone).IsUnicode(false);
            entity.Property(e => e.City).IsUnicode(false);
            entity.Property(e => e.CompanyIndustry).IsUnicode(false);
            entity.Property(e => e.CompanyName).IsUnicode(false);
            entity.Property(e => e.CompanyRanking).IsUnicode(false);
            entity.Property(e => e.CompanyRevenue).IsUnicode(false);
            entity.Property(e => e.CompanySize).IsUnicode(false);
            entity.Property(e => e.CompanyURL).IsUnicode(false);
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.CreatedOn).IsUnicode(false);
            entity.Property(e => e.Created_By).IsUnicode(false);
            entity.Property(e => e.CustomerBackground).IsUnicode(false);
            entity.Property(e => e.CustomerCategory).IsUnicode(false);
            entity.Property(e => e.CustomerId).IsUnicode(false);
            entity.Property(e => e.CustomerType).IsUnicode(false);
            entity.Property(e => e.FacebookURL).IsUnicode(false);
            entity.Property(e => e.FortuneRanking).IsUnicode(false);
            entity.Property(e => e.InstantMessengerId).IsUnicode(false);
            entity.Property(e => e.JobTitle).IsUnicode(false);
            entity.Property(e => e.LinkedInURL).IsUnicode(false);
            entity.Property(e => e.PersonalEmail).IsUnicode(false);
            entity.Property(e => e.PersonalPhone).IsUnicode(false);
            entity.Property(e => e.ReferrerClientName).IsUnicode(false);
            entity.Property(e => e.ReferrerClientid).IsUnicode(false);
            entity.Property(e => e.Region).IsUnicode(false);
            entity.Property(e => e.SkypeID).IsUnicode(false);
            entity.Property(e => e.State).IsUnicode(false);
            entity.Property(e => e.Twitter_URL).IsUnicode(false);
            entity.Property(e => e.Unsubscribe).IsUnicode(false);
            entity.Property(e => e.Website).IsUnicode(false);
            entity.Property(e => e.ZipCode).IsUnicode(false);
        });

        modelBuilder.Entity<AccountReport_13_May_2026>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AccountReport_13_May_2026", "CRM");

            entity.Property(e => e.AccountCategorization).IsUnicode(false);
            entity.Property(e => e.AccountCategorizationId).IsUnicode(false);
            entity.Property(e => e.AccountName).IsUnicode(false);
            entity.Property(e => e.AccountOwner).IsUnicode(false);
            entity.Property(e => e.Address).IsUnicode(false);
            entity.Property(e => e.BusinessEmail).IsUnicode(false);
            entity.Property(e => e.BusinessPhone).IsUnicode(false);
            entity.Property(e => e.City).IsUnicode(false);
            entity.Property(e => e.CompanyIndustry).IsUnicode(false);
            entity.Property(e => e.CompanyName).IsUnicode(false);
            entity.Property(e => e.CompanyRanking).IsUnicode(false);
            entity.Property(e => e.CompanyRevenue).IsUnicode(false);
            entity.Property(e => e.CompanySize).IsUnicode(false);
            entity.Property(e => e.CompanySizeId).IsUnicode(false);
            entity.Property(e => e.CompanyURL).IsUnicode(false);
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.CountryId).IsUnicode(false);
            entity.Property(e => e.CreatedOn).IsUnicode(false);
            entity.Property(e => e.Created_By).IsUnicode(false);
            entity.Property(e => e.CustomerBackground).IsUnicode(false);
            entity.Property(e => e.CustomerCategory).IsUnicode(false);
            entity.Property(e => e.CustomerCategoryId).IsUnicode(false);
            entity.Property(e => e.CustomerId).IsUnicode(false);
            entity.Property(e => e.CustomerType).IsUnicode(false);
            entity.Property(e => e.CustomerTypeId).IsUnicode(false);
            entity.Property(e => e.FacebookURL).IsUnicode(false);
            entity.Property(e => e.FortuneRanking).IsUnicode(false);
            entity.Property(e => e.IndustryId).IsUnicode(false);
            entity.Property(e => e.InstantMessengerId).IsUnicode(false);
            entity.Property(e => e.JobTitle).IsUnicode(false);
            entity.Property(e => e.JobTitleId).IsUnicode(false);
            entity.Property(e => e.LeadID)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.LeadSourceId)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.LinkedInURL).IsUnicode(false);
            entity.Property(e => e.OwnerId)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.PersonalEmail).IsUnicode(false);
            entity.Property(e => e.PersonalPhone).IsUnicode(false);
            entity.Property(e => e.ReferrerClientName).IsUnicode(false);
            entity.Property(e => e.ReferrerClientid).IsUnicode(false);
            entity.Property(e => e.Region).IsUnicode(false);
            entity.Property(e => e.RegionId).IsUnicode(false);
            entity.Property(e => e.SkypeID).IsUnicode(false);
            entity.Property(e => e.State).IsUnicode(false);
            entity.Property(e => e.Twitter_URL).IsUnicode(false);
            entity.Property(e => e.Unsubscribe).IsUnicode(false);
            entity.Property(e => e.Website).IsUnicode(false);
            entity.Property(e => e.ZipCode).IsUnicode(false);
        });

        modelBuilder.Entity<AccountReport_22_May_2026>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AccountReport_22_May_2026", "CRM");

            entity.Property(e => e.AcceptScannedDocumentsNDA).IsUnicode(false);
            entity.Property(e => e.AccountCategorization).IsUnicode(false);
            entity.Property(e => e.AccountCategorizationId).IsUnicode(false);
            entity.Property(e => e.AccountId).IsUnicode(false);
            entity.Property(e => e.AccountName).IsUnicode(false);
            entity.Property(e => e.AccountOwner).IsUnicode(false);
            entity.Property(e => e.AccountSourceId).IsUnicode(false);
            entity.Property(e => e.AccountType).IsUnicode(false);
            entity.Property(e => e.Address).IsUnicode(false);
            entity.Property(e => e.BusinessEmail).IsUnicode(false);
            entity.Property(e => e.BusinessPhone).IsUnicode(false);
            entity.Property(e => e.City).IsUnicode(false);
            entity.Property(e => e.CompanyIndustry).IsUnicode(false);
            entity.Property(e => e.CompanyName).IsUnicode(false);
            entity.Property(e => e.CompanyRanking).IsUnicode(false);
            entity.Property(e => e.CompanyRevenue).IsUnicode(false);
            entity.Property(e => e.CompanyURL).IsUnicode(false);
            entity.Property(e => e.Continent).IsUnicode(false);
            entity.Property(e => e.Country).IsUnicode(false);
            entity.Property(e => e.CountryId).IsUnicode(false);
            entity.Property(e => e.CreatedOn).IsUnicode(false);
            entity.Property(e => e.CustomerBackground).IsUnicode(false);
            entity.Property(e => e.CustomerBlogURL).IsUnicode(false);
            entity.Property(e => e.CustomerCategory).IsUnicode(false);
            entity.Property(e => e.CustomerCategoryId).IsUnicode(false);
            entity.Property(e => e.CustomerId).IsUnicode(false);
            entity.Property(e => e.CustomerType).IsUnicode(false);
            entity.Property(e => e.CustomerTypeID).IsUnicode(false);
            entity.Property(e => e.FacebookURL).IsUnicode(false);
            entity.Property(e => e.FortuneRanking).IsUnicode(false);
            entity.Property(e => e.Gender).IsUnicode(false);
            entity.Property(e => e.GenderId).IsUnicode(false);
            entity.Property(e => e.HighPotentiaStatus).IsUnicode(false);
            entity.Property(e => e.HowdidyouhearaboutBrickwork).IsUnicode(false);
            entity.Property(e => e.IndustryId).IsUnicode(false);
            entity.Property(e => e.InstantMessengerId).IsUnicode(false);
            entity.Property(e => e.JobTitle).IsUnicode(false);
            entity.Property(e => e.JobTitleId).IsUnicode(false);
            entity.Property(e => e.LeadId).IsUnicode(false);
            entity.Property(e => e.LeadSourceId).IsUnicode(false);
            entity.Property(e => e.LinkedInURL).IsUnicode(false);
            entity.Property(e => e.NDAEndDate).IsUnicode(false);
            entity.Property(e => e.NDAExecuted).IsUnicode(false);
            entity.Property(e => e.NDAExecutedDate).IsUnicode(false);
            entity.Property(e => e.NDAFormat).IsUnicode(false);
            entity.Property(e => e.NDALink).IsUnicode(false);
            entity.Property(e => e.NDANumber).IsUnicode(false);
            entity.Property(e => e.NDARenewalDate).IsUnicode(false);
            entity.Property(e => e.NDASpecialClauseForInternal).IsUnicode(false);
            entity.Property(e => e.NDASpecialClauseForOrg).IsUnicode(false);
            entity.Property(e => e.NDAStartDate).IsUnicode(false);
            entity.Property(e => e.NDAVersionNumber).IsUnicode(false);
            entity.Property(e => e.OtherURLIfAny).IsUnicode(false);
            entity.Property(e => e.OwnerId).IsUnicode(false);
            entity.Property(e => e.PaymentDate).IsUnicode(false);
            entity.Property(e => e.PersonAccountSkypeID).IsUnicode(false);
            entity.Property(e => e.PersonalEmail).IsUnicode(false);
            entity.Property(e => e.PersonalPhone).IsUnicode(false);
            entity.Property(e => e.ReferrerClientName).IsUnicode(false);
            entity.Property(e => e.ReferrerClientid).IsUnicode(false);
            entity.Property(e => e.RegionId).IsUnicode(false);
            entity.Property(e => e.SkypeID).IsUnicode(false);
            entity.Property(e => e.State).IsUnicode(false);
            entity.Property(e => e.TaskPurpose).IsUnicode(false);
            entity.Property(e => e.TenantId).IsUnicode(false);
            entity.Property(e => e.TwitterURL).IsUnicode(false);
            entity.Property(e => e.Unsubscribe).IsUnicode(false);
            entity.Property(e => e.WOExecuted).IsUnicode(false);
            entity.Property(e => e.WOExecutedDate).IsUnicode(false);
            entity.Property(e => e.WONumber).IsUnicode(false);
            entity.Property(e => e.WOVersionNumber).IsUnicode(false);
            entity.Property(e => e.Website).IsUnicode(false);
        });

        modelBuilder.Entity<AccountSource>(entity =>
        {
            entity.ToTable("AccountSources", "CRM");

            entity.Property(e => e.AccountSourceId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AccountSourceName).HasMaxLength(200);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<ActivityType>(entity =>
        {
            entity.ToTable("ActivityTypes", "CRM");

            entity.Property(e => e.ActivityTypeId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ActivityCode).HasMaxLength(50);
            entity.Property(e => e.ActivityTypeName).HasMaxLength(100);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<ApprovalTransaction>(entity =>
        {
            entity.HasKey(e => e.ApprovalId).HasName("PK__Approval__328477F415886805");

            entity.ToTable("ApprovalTransactions", "CRM");

            entity.Property(e => e.ApprovalId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ActionDate).HasColumnType("datetime");
            entity.Property(e => e.ApprovalStatus).HasMaxLength(50);
            entity.Property(e => e.Remarks).HasMaxLength(500);

            entity.HasOne(d => d.Step).WithMany(p => p.ApprovalTransactions)
                .HasForeignKey(d => d.StepId)
                .HasConstraintName("FK_Approval_Step");

            entity.HasOne(d => d.Workflow).WithMany(p => p.ApprovalTransactions)
                .HasForeignKey(d => d.WorkflowId)
                .HasConstraintName("FK_Approval_Workflow");
        });

        modelBuilder.Entity<ApprovalWorkflow>(entity =>
        {
            entity.HasKey(e => e.WorkflowId).HasName("PK__Approval__5704A66A433F5129");

            entity.ToTable("ApprovalWorkflows", "CRM");

            entity.Property(e => e.WorkflowId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModuleName).HasMaxLength(100);
            entity.Property(e => e.WorkflowName).HasMaxLength(200);
        });

        modelBuilder.Entity<ApprovalWorkflowLevel>(entity =>
        {
            entity.HasKey(e => e.WorkflowLevelId).HasName("PK__Approval__4FC4220E5E25D8AA");

            entity.ToTable("ApprovalWorkflowLevels", "CRM");

            entity.Property(e => e.WorkflowLevelId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.Workflow).WithMany(p => p.ApprovalWorkflowLevels)
                .HasForeignKey(d => d.WorkflowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApprovalWorkflowLevels_Workflow");
        });

        modelBuilder.Entity<ApprovalWorkflowLevelUser>(entity =>
        {
            entity.HasKey(e => e.WorkflowLevelUserId).HasName("PK__Approval__63C3CCE1B1DDED8C");

            entity.ToTable("ApprovalWorkflowLevelUsers", "CRM");

            entity.Property(e => e.WorkflowLevelUserId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.WorkflowLevel).WithMany(p => p.ApprovalWorkflowLevelUsers)
                .HasForeignKey(d => d.WorkflowLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApprovalWorkflowLevelUsers_Level");
        });

        modelBuilder.Entity<ApprovalWorkflowMaster>(entity =>
        {
            entity.HasKey(e => e.WorkflowId).HasName("PK__Approval__5704A66AAD48D714");

            entity.ToTable("ApprovalWorkflowMaster", "CRM");

            entity.Property(e => e.WorkflowId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModuleName).HasMaxLength(150);
            entity.Property(e => e.WorkflowName).HasMaxLength(200);
        });

        modelBuilder.Entity<ApprovalWorkflowRole>(entity =>
        {
            entity.HasKey(e => e.WorkflowRoleId).HasName("PK__Approval__FB18613F714E50EE");

            entity.ToTable("ApprovalWorkflowRoles", "CRM");

            entity.Property(e => e.WorkflowRoleId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleId).HasMaxLength(450);

            entity.HasOne(d => d.Workflow).WithMany(p => p.ApprovalWorkflowRoles)
                .HasForeignKey(d => d.WorkflowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApprovalWorkflowRoles_Workflow");
        });

        modelBuilder.Entity<ApprovalWorkflowService>(entity =>
        {
            entity.HasKey(e => e.WorkflowServiceId).HasName("PK__Approval__DD89306273131E74");

            entity.ToTable("ApprovalWorkflowServices", "CRM");

            entity.Property(e => e.WorkflowServiceId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Workflow).WithMany(p => p.ApprovalWorkflowServices)
                .HasForeignKey(d => d.WorkflowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApprovalWorkflowServices_Workflow");
        });

        modelBuilder.Entity<CompanySize>(entity =>
        {
            entity.HasKey(e => e.LeadCompanySizeId).HasName("PK__CompanyS__4F8C0AE48D8149D6");

            entity.ToTable("CompanySize", "CRM");

            entity.Property(e => e.LeadCompanySizeId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SizeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__Contact__3214EC07930084A3");

            entity.ToTable("Contact", "CRM");

            entity.Property(e => e.ContactId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Designation)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.FacebookURL).HasMaxLength(200);
            entity.Property(e => e.FullName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(false);
            entity.Property(e => e.MobilePhone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.PersonalEmail1).HasMaxLength(500);
            entity.Property(e => e.PersonalEmail2).HasMaxLength(500);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SkpeId).HasMaxLength(200);
            entity.Property(e => e.TwitterURL).HasMaxLength(200);

            entity.HasOne(d => d.Account).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Contact_Account");

            entity.HasOne(d => d.Salutation).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.SalutationId)
                .HasConstraintName("FK_Contact_SalutationMaster");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__Countrie__10D1609F7656702E");

            entity.ToTable("Countries", "CRM");

            entity.Property(e => e.CountryId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CountryCode).HasMaxLength(10);
            entity.Property(e => e.CountryName).HasMaxLength(150);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrencyCode).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PhoneCode).HasMaxLength(10);

            entity.HasOne(d => d.Region).WithMany(p => p.Countries)
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_Countries_Regions");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.CurrencyId).HasName("PK__Currenci__14470AF03448538D");

            entity.ToTable("Currencies", "CRM");

            entity.HasIndex(e => e.CurrencyCode, "UQ__Currenci__408426BFF33DC61F").IsUnique();

            entity.Property(e => e.CurrencyId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrencyCode).HasMaxLength(10);
            entity.Property(e => e.CurrencyName).HasMaxLength(100);
            entity.Property(e => e.CurrencySymbol).HasMaxLength(10);
            entity.Property(e => e.ExchangeRate)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(18, 6)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsBaseCurrency).HasDefaultValue(false);
        });

        modelBuilder.Entity<CustomerCategory>(entity =>
        {
            entity.ToTable("CustomerCategories", "CRM");

            entity.Property(e => e.CustomerCategoryId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.CustomerCategory1)
                .HasMaxLength(200)
                .HasColumnName("CustomerCategory");
            entity.Property(e => e.Description).HasMaxLength(300);
        });

        modelBuilder.Entity<CustomerType>(entity =>
        {
            entity.HasKey(e => e.CustomerTypeId).HasName("PK__Customer__958B61AC94B1E1DB");

            entity.ToTable("CustomerTypes", "CRM");

            entity.Property(e => e.CustomerTypeId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerTypeName).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<DealType>(entity =>
        {
            entity.HasKey(e => e.DealTypeId).HasName("PK__DealType__1C7E2016DA103175");

            entity.ToTable("DealTypes", "CRM");

            entity.Property(e => e.DealTypeId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DealTypeName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<DocumentRepository>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("PK__Document__442C64BEA9BEA4D9");

            entity.ToTable("DocumentRepository", "CRM");

            entity.Property(e => e.AttachmentId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DocumentType).HasMaxLength(100);
            entity.Property(e => e.FileExtension).HasMaxLength(20);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OriginalFileName).HasMaxLength(255);
            entity.Property(e => e.ReferenceType).HasMaxLength(100);
            entity.Property(e => e.Remarks).HasMaxLength(500);
        });

        modelBuilder.Entity<EmailAttachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("PK__EmailAtt__442C64BE10EC99BC");

            entity.ToTable("EmailAttachment", "CRM");

            entity.Property(e => e.AttachmentId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.MimeType).HasMaxLength(100);

            entity.HasOne(d => d.Email).WithMany(p => p.EmailAttachments)
                .HasForeignKey(d => d.EmailId)
                .HasConstraintName("FK_EmailAttachment_Email");
        });

        modelBuilder.Entity<EmailMessage>(entity =>
        {
            entity.HasKey(e => e.EmailId).HasName("PK__EmailMes__7ED91ACFE7E31C17");

            entity.ToTable("EmailMessage", "CRM");

            entity.Property(e => e.EmailId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ParentEntityType).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Subject).HasMaxLength(255);
        });

        modelBuilder.Entity<EmailRecipient>(entity =>
        {
            entity.HasKey(e => e.RecipientId).HasName("PK__EmailRec__F0A6024D6E451739");

            entity.ToTable("EmailRecipient", "CRM");

            entity.Property(e => e.RecipientId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(10);

            entity.HasOne(d => d.Email).WithMany(p => p.EmailRecipients)
                .HasForeignKey(d => d.EmailId)
                .HasConstraintName("FK_EmailRecipient_Email");
        });

        modelBuilder.Entity<EngagementModel>(entity =>
        {
            entity.HasKey(e => e.EngagementModelId).HasName("PK__Engageme__70C7F786253A2C2F");

            entity.ToTable("EngagementModels", "CRM");

            entity.Property(e => e.EngagementModelId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.EngagementModelName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Field>(entity =>
        {
            entity.HasKey(e => e.FieldId).HasName("PK__Fields__C8B6FF07CEFE6803");

            entity.ToTable("Fields", "CRM");

            entity.Property(e => e.FieldId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DataType).HasMaxLength(50);
            entity.Property(e => e.FieldLabel).HasMaxLength(100);
            entity.Property(e => e.FieldName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Form).WithMany(p => p.Fields)
                .HasForeignKey(d => d.FormId)
                .HasConstraintName("FK_Fields_Forms");
        });

        modelBuilder.Entity<FieldRoleAccess>(entity =>
        {
            entity.HasKey(e => e.AccessId).HasName("PK__FieldRol__4130D05FC5DC4435");

            entity.ToTable("FieldRoleAccess", "CRM");

            entity.Property(e => e.AccessId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CanEdit).HasDefaultValue(false);
            entity.Property(e => e.CanView).HasDefaultValue(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsHidden).HasDefaultValue(false);
            entity.Property(e => e.RoleId).HasMaxLength(450);

            entity.HasOne(d => d.Field).WithMany(p => p.FieldRoleAccesses)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK_FieldRoleAccess_Fields");

            entity.HasOne(d => d.Form).WithMany(p => p.FieldRoleAccesses)
                .HasForeignKey(d => d.FormId)
                .HasConstraintName("FK_FieldRoleAccess_Forms");

            entity.HasOne(d => d.Module).WithMany(p => p.FieldRoleAccesses)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_FieldRoleAccess_Modules");
        });

        modelBuilder.Entity<Form>(entity =>
        {
            entity.HasKey(e => e.FormId).HasName("PK__Forms__FB05B7DDDAE0FD66");

            entity.ToTable("Forms", "CRM");

            entity.Property(e => e.FormId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FormName).HasMaxLength(100);

            entity.HasOne(d => d.Module).WithMany(p => p.Forms)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_Forms_Modules");
        });

        modelBuilder.Entity<GenderMaster>(entity =>
        {
            entity.HasKey(e => e.GenderId).HasName("PK__GenderMa__4E24E9F7FCDD12A3");

            entity.ToTable("GenderMaster", "CRM");

            entity.Property(e => e.GenderId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.GenderName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Industry>(entity =>
        {
            entity.HasKey(e => e.IndustryId).HasName("PK__Industri__808DEDCC31B9800A");

            entity.ToTable("Industries", "CRM");

            entity.Property(e => e.IndustryId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.IndustryName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.ParentIndustry).WithMany(p => p.InverseParentIndustry)
                .HasForeignKey(d => d.ParentIndustryId)
                .HasConstraintName("FK_CompanyIndustry_Parent");
        });

        modelBuilder.Entity<JobTitle>(entity =>
        {
            entity.HasKey(e => e.JobTitleId).HasName("PK__JobTitle__35382FE990BEFD2E");

            entity.ToTable("JobTitles", "CRM");

            entity.Property(e => e.JobTitleId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.JobTitleName).HasMaxLength(150);
        });

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.HasKey(e => e.LeadId).HasName("PK__Leads__73EF78FABA121700");

            entity.ToTable("Leads", "CRM");

            entity.Property(e => e.LeadId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.BDComments).HasMaxLength(1000);
            entity.Property(e => e.City).HasMaxLength(200);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.CompanyRanking).HasMaxLength(200);
            entity.Property(e => e.CompanyRevenue).HasMaxLength(500);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerBackground).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(500);
            entity.Property(e => e.EnquiryId).HasMaxLength(100);
            entity.Property(e => e.FacebookURL).HasMaxLength(200);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.LinkedinURL).HasMaxLength(200);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.PersonalEmail1).HasMaxLength(500);
            entity.Property(e => e.PersonalEmail2).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(300);
            entity.Property(e => e.PreferredModeofCommunication).HasMaxLength(200);
            entity.Property(e => e.SkpeId).HasMaxLength(200);
            entity.Property(e => e.TwitterURL).HasMaxLength(200);
            entity.Property(e => e.Website).HasMaxLength(200);
            entity.Property(e => e.ZipCode).HasMaxLength(100);

            entity.HasOne(d => d.CompanySize).WithMany(p => p.Leads)
                .HasForeignKey(d => d.CompanySizeId)
                .HasConstraintName("FK_Leads_CompanySize");

            entity.HasOne(d => d.Country).WithMany(p => p.Leads)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("FK_Leads_Countries");

            entity.HasOne(d => d.Gender).WithMany(p => p.Leads)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("FK_Leads_GenderMaster");

            entity.HasOne(d => d.Industry).WithMany(p => p.Leads)
                .HasForeignKey(d => d.IndustryId)
                .HasConstraintName("FK_Leads_Industries");

            entity.HasOne(d => d.JobTitle).WithMany(p => p.Leads)
                .HasForeignKey(d => d.JobTitleId)
                .HasConstraintName("FK_Leads_JobTitles");

            entity.HasOne(d => d.LeadPriority).WithMany(p => p.Leads)
                .HasForeignKey(d => d.LeadPriorityId)
                .HasConstraintName("FK_Leads_LeadPriority");

            entity.HasOne(d => d.LeadSource).WithMany(p => p.Leads)
                .HasForeignKey(d => d.LeadSourceId)
                .HasConstraintName("FK_Leads_LeadSources");

            entity.HasOne(d => d.LeadStage).WithMany(p => p.Leads)
                .HasForeignKey(d => d.LeadStageId)
                .HasConstraintName("FK_Leads_LeadStage");

            entity.HasOne(d => d.LeadType).WithMany(p => p.Leads)
                .HasForeignKey(d => d.LeadTypeId)
                .HasConstraintName("FK_Leads_LeadTypes");

            entity.HasOne(d => d.Rating).WithMany(p => p.Leads)
                .HasForeignKey(d => d.RatingId)
                .HasConstraintName("FK_Leads_Ratings");
        });

        modelBuilder.Entity<LeadActivity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK__LeadActi__45F4A791A17486D0");

            entity.ToTable("LeadActivities", "CRM");

            entity.Property(e => e.ActivityId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.ActivityDate).HasColumnType("datetime");
            entity.Property(e => e.ActivityType).HasMaxLength(100);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<LeadAssignment>(entity =>
        {
            entity.HasKey(e => e.LeadAssignmentId).HasName("PK__LeadAssi__3214EC07FC789139");

            entity.ToTable("LeadAssignments", "CRM");

            entity.Property(e => e.LeadAssignmentId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AssignedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Lead).WithMany(p => p.LeadAssignments)
                .HasForeignKey(d => d.LeadId)
                .HasConstraintName("FK_LeadAssignments_Lead");
        });

        modelBuilder.Entity<LeadAttachment>(entity =>
        {
            entity.HasKey(e => e.LeadAttachmentId).HasName("PK__LeadAtta__3214EC0708E381DC");

            entity.ToTable("LeadAttachments", "CRM");

            entity.Property(e => e.LeadAttachmentId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(200);
            entity.Property(e => e.FilePath).HasMaxLength(500);

            entity.HasOne(d => d.Lead).WithMany(p => p.LeadAttachments)
                .HasForeignKey(d => d.LeadId)
                .HasConstraintName("FK_LeadAttachments_Leads");
        });

        modelBuilder.Entity<LeadConfiguration>(entity =>
        {
            entity.HasKey(e => e.LeadConfigId).HasName("PK__LeadConf__58353794ACC5745B");

            entity.ToTable("LeadConfiguration", "CRM");

            entity.Property(e => e.LeadConfigId).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.EffectiveFrom).HasColumnType("datetime");
            entity.Property(e => e.Remarks).HasMaxLength(1000);
            entity.Property(e => e.RuleId).HasDefaultValueSql("(newsequentialid())");

            entity.HasOne(d => d.Region).WithMany(p => p.LeadConfigurations)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeadConfiguration_Regions");

            entity.HasOne(d => d.Service).WithMany(p => p.LeadConfigurations)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeadConfiguration_Services");
        });

        modelBuilder.Entity<LeadNote>(entity =>
        {
            entity.HasKey(e => e.NoteId).HasName("PK__LeadNote__EACE355F6748A243");

            entity.ToTable("LeadNotes", "CRM");

            entity.Property(e => e.NoteId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Lead).WithMany(p => p.LeadNotes)
                .HasForeignKey(d => d.LeadId)
                .HasConstraintName("FK_LeadNotes_Leads");
        });

        modelBuilder.Entity<LeadRejection>(entity =>
        {
            entity.HasKey(e => e.LeadRejectionId).HasName("PK__LeadReje__B666923F1E16D71A");

            entity.ToTable("LeadRejection", "CRM");

            entity.Property(e => e.LeadRejectionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsApproved).HasDefaultValue(false);
            entity.Property(e => e.RejectedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<LeadServiceMapping>(entity =>
        {
            entity.HasKey(e => e.LeadServiceId).HasName("PK__LeadServ__FABECAE59F8CEA92");

            entity.ToTable("LeadServiceMapping", "CRM");

            entity.Property(e => e.LeadServiceId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(d => d.Lead).WithMany(p => p.LeadServiceMappings)
                .HasForeignKey(d => d.LeadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeadServiceMapping_Leads");

            entity.HasOne(d => d.Service).WithMany(p => p.LeadServiceMappingServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeadServiceMapping_Services");

            entity.HasOne(d => d.SubService).WithMany(p => p.LeadServiceMappingSubServices)
                .HasForeignKey(d => d.SubServiceId)
                .HasConstraintName("FK_LeadServiceMapping_Services1");
        });

        modelBuilder.Entity<LeadSource>(entity =>
        {
            entity.HasKey(e => e.LeadSourceId).HasName("PK__LeadSour__9FB37DD30BEB0A73");

            entity.ToTable("LeadSources", "CRM");

            entity.Property(e => e.LeadSourceId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.SourceName).HasMaxLength(100);
        });

        modelBuilder.Entity<LeadStage>(entity =>
        {
            entity.HasKey(e => e.LeadStageId).HasName("PK__LeadStat__C8EE2063C3C8A2B3");

            entity.ToTable("LeadStage", "CRM");

            entity.Property(e => e.LeadStageId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.StageName).HasMaxLength(100);
        });

        modelBuilder.Entity<LeadType>(entity =>
        {
            entity.HasKey(e => e.LeadTypeId).HasName("PK__LeadType__023608481C444E54");

            entity.ToTable("LeadTypes", "CRM");

            entity.Property(e => e.LeadTypeId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LeadTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Lead_Report_Sample>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Lead_Report_Sample", "CRM");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.ModuleId).HasName("PK__Modules__2B7477A7F51A4CE6");

            entity.ToTable("Modules", "CRM");

            entity.Property(e => e.ModuleId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModuleName).HasMaxLength(100);
        });

        modelBuilder.Entity<NDASignature>(entity =>
        {
            entity.HasKey(e => e.NDAID).HasName("PK__NDASigna__9A952C4403AFBE49");

            entity.ToTable("NDASignature", "CRM");

            entity.Property(e => e.NDAID).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AcceptNonDisclosureAgreement).HasDefaultValue(false);
            entity.Property(e => e.AcceptScannedDocumentsNDA).HasDefaultValue(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExecutedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.NDACustomerName).HasMaxLength(250);
            entity.Property(e => e.NDAFormat).HasMaxLength(100);
            entity.Property(e => e.NDALink).HasMaxLength(500);
            entity.Property(e => e.NDANumber).HasMaxLength(100);
            entity.Property(e => e.NDAVersionNumber).HasMaxLength(50);
        });

        modelBuilder.Entity<Opportunity>(entity =>
        {
            entity.HasKey(e => e.OpportunityId).HasName("PK__Opportun__0034ED91515B92B4");

            entity.ToTable("Opportunities", "CRM");

            entity.Property(e => e.OpportunityId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.OpportunityName).HasMaxLength(200);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<OpportunityModule>(entity =>
        {
            entity.HasKey(e => e.OpportunityModuleId).HasName("PK__Opportunity__D357F809550EDAC7");

            entity.ToTable("OpportunityModules", "CRM");

            entity.Property(e => e.OpportunityModuleId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EstimatedHours).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ModuleName).HasMaxLength(200);
            entity.Property(e => e.QCHours).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Opportunity).WithMany(p => p.OpportunityModules)
                .HasForeignKey(d => d.OpportunityId)
                .HasConstraintName("FK_OpportunityModules_Opportunities");
        });

        modelBuilder.Entity<OpportunityService>(entity =>
        {
            entity.HasKey(e => e.OpportunityServiceId).HasName("PK__Opportun__1CA49E93D7D935CA");

            entity.ToTable("OpportunityServices", "CRM");

            entity.Property(e => e.OpportunityServiceId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpectedOn).HasColumnType("datetime");
            entity.Property(e => e.ScoperStatus).HasMaxLength(50);
            entity.Property(e => e.ScoperUpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.ServiceName).HasMaxLength(200);
            entity.Property(e => e.SupportTime)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<OpportunityStage>(entity =>
        {
            entity.HasKey(e => e.StageId).HasName("PK__Opportun__03EB7AD8E6DE7FEE");

            entity.ToTable("OpportunityStages", "CRM");

            entity.Property(e => e.StageId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.RoleId).HasMaxLength(450);
            entity.Property(e => e.StageName).HasMaxLength(100);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A381356DA0A");

            entity.ToTable("Payments", "CRM");

            entity.Property(e => e.PaymentId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AmountRecieved).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BDRemarks).HasMaxLength(500);
            entity.Property(e => e.FinanceApprovedOn).HasColumnType("datetime");
            entity.Property(e => e.FinanceRemarks).HasMaxLength(500);
            entity.Property(e => e.FinanceRequestSentOn).HasColumnType("datetime");
            entity.Property(e => e.InvoiceAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceEndDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceStartDate).HasColumnType("datetime");
            entity.Property(e => e.MilestoneCreatedOn).HasColumnType("datetime");
            entity.Property(e => e.PaymentDueDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentLink).HasMaxLength(500);
            entity.Property(e => e.PaymentMilestone).HasMaxLength(150);
            entity.Property(e => e.PaymentReceivedOn).HasColumnType("datetime");
            entity.Property(e => e.TransactionNumber).HasMaxLength(150);

            entity.HasOne(d => d.Currency).WithMany(p => p.Payments)
                .HasForeignKey(d => d.CurrencyId)
                .HasConstraintName("FK_Payments_Currencies");

            entity.HasOne(d => d.PaymentStatus).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentStatusId)
                .HasConstraintName("FK_Payments_PaymentStatus");

            entity.HasOne(d => d.PaymentType).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentTypeId)
                .HasConstraintName("FK_Payments_PaymentTypes");

            entity.HasOne(d => d.Project).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_Payments_Projects");

            entity.HasOne(d => d.WorkOrder).WithMany(p => p.Payments)
                .HasForeignKey(d => d.WorkOrderId)
                .HasConstraintName("FK_Payments_WorkOrders");
        });

        modelBuilder.Entity<PaymentMode>(entity =>
        {
            entity.HasKey(e => e.ModeOfPaymentId).HasName("PK_ModeOfPayment");

            entity.ToTable("PaymentMode", "CRM");

            entity.HasIndex(e => e.PaymentModeName, "UQ_ModeOfPayment_Name").IsUnique();

            entity.Property(e => e.ModeOfPaymentId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.PaymentModeCode).HasMaxLength(50);
            entity.Property(e => e.PaymentModeName).HasMaxLength(100);
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(e => e.PaymentStatusId).HasName("PK__PaymentS__34F8AC3FD67F3FF9");

            entity.ToTable("PaymentStatus", "CRM");

            entity.Property(e => e.PaymentStatusId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<PaymentType>(entity =>
        {
            entity.HasKey(e => e.PaymentTypeId).HasName("PK__PaymentT__BA430B3538BDDE74");

            entity.ToTable("PaymentTypes", "CRM");

            entity.Property(e => e.PaymentTypeId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PaymentTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Priority>(entity =>
        {
            entity.HasKey(e => e.PriorityId).HasName("PK__Priority__D0A3D0BE15633632");

            entity.ToTable("Priority", "CRM");

            entity.Property(e => e.PriorityId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.ColorCode).HasMaxLength(20);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PriorityName).HasMaxLength(100);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABEF0641ECD9D");

            entity.ToTable("Projects", "CRM");

            entity.Property(e => e.ProjectId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Attachment).HasMaxLength(300);
            entity.Property(e => e.BalanceHours).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerID)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ProjectCode).HasMaxLength(50);
            entity.Property(e => e.ProjectHours).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProjectName).HasMaxLength(200);
            entity.Property(e => e.TransferHours).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Priority).WithMany(p => p.Projects)
                .HasForeignKey(d => d.PriorityId)
                .HasConstraintName("FK_Projects_Priority");

            entity.HasOne(d => d.ProjectStatus).WithMany(p => p.Projects)
                .HasForeignKey(d => d.ProjectStatusId)
                .HasConstraintName("FK_Projects_ProjectStatus");
        });

        modelBuilder.Entity<ProjectModule>(entity =>
        {
            entity.HasKey(e => e.ProjectModuleId).HasName("PK__ProjectM__D357F809550EDAC7");

            entity.ToTable("ProjectModules", "CRM");

            entity.Property(e => e.ProjectModuleId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EstimatedHours).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ModuleName).HasMaxLength(200);

            entity.HasOne(d => d.Opportunity).WithMany(p => p.ProjectModules)
                .HasForeignKey(d => d.OpportunityId)
                .HasConstraintName("FK_ProjectModules_Opportunities");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectModules)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_ProjectModules_Projects");
        });

        modelBuilder.Entity<ProjectMoudleEmployeemap>(entity =>
        {
            entity.ToTable("ProjectMoudleEmployeemap", "CRM");

            entity.Property(e => e.ProjectMoudleEmployeemapId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.ProjectModule).WithMany(p => p.ProjectMoudleEmployeemaps)
                .HasForeignKey(d => d.ProjectModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectMoudleEmployeemap_ProjectModules");
        });

        modelBuilder.Entity<ProjectStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__ProjectS__C8EE2063C6C44680");

            entity.ToTable("ProjectStatus", "CRM");

            entity.Property(e => e.StatusId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StatusName).HasMaxLength(100);
        });

        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(e => e.ProjectTaskId).HasName("PK__ProjectT__71C01D04BE4CAD00");

            entity.ToTable("ProjectTasks", "CRM");

            entity.Property(e => e.ProjectTaskId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.EstimatedHours).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.TaskName).HasMaxLength(200);

            entity.HasOne(d => d.Priority).WithMany(p => p.ProjectTasks)
                .HasForeignKey(d => d.PriorityId)
                .HasConstraintName("FK_ProjectTasks_ProjectTaskPriorities");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectTasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectTasks_Projects");

            entity.HasOne(d => d.TaskStatus).WithMany(p => p.ProjectTasks)
                .HasForeignKey(d => d.TaskStatusId)
                .HasConstraintName("FK_ProjectTasks_ProjectTaskStatus");
        });

        modelBuilder.Entity<ProjectTaskPriority>(entity =>
        {
            entity.HasKey(e => e.PriorityId).HasName("PK__ProjectT__D0A3D0BE5B509F00");

            entity.ToTable("ProjectTaskPriorities", "CRM");

            entity.Property(e => e.PriorityId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PriorityName).HasMaxLength(100);
        });

        modelBuilder.Entity<ProjectTaskStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__ProjectT__C8EE20631135BDBF");

            entity.ToTable("ProjectTaskStatus", "CRM");

            entity.Property(e => e.StatusId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsClosed).HasDefaultValue(false);
            entity.Property(e => e.StatusName).HasMaxLength(100);
        });

        modelBuilder.Entity<ProjectTimeLog>(entity =>
        {
            entity.HasKey(e => e.TimeLogId).HasName("PK__ProjectT__26E43757BD4577A5");

            entity.ToTable("ProjectTimeLogs", "CRM");

            entity.Property(e => e.TimeLogId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.ProjectTask).WithMany(p => p.ProjectTimeLogs)
                .HasForeignKey(d => d.ProjectTaskId)
                .HasConstraintName("FK_ProjectTimeLogs_ProjectTasks");
        });

        modelBuilder.Entity<Proposal>(entity =>
        {
            entity.HasKey(e => e.ProposalId).HasName("PK__Proposal__6F39E12069CDE566");

            entity.ToTable("Proposals", "CRM");

            entity.Property(e => e.ProposalId).ValueGeneratedNever();
            entity.Property(e => e.ApprovalToken).HasMaxLength(200);
            entity.Property(e => e.ClientApprovedOn).HasColumnType("datetime");
            entity.Property(e => e.ClientRejectedReason).HasMaxLength(1000);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.EstimatedHours).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProposalNumber).HasMaxLength(50);
            entity.Property(e => e.ProposalStatus).HasMaxLength(50);
            entity.Property(e => e.ProposalTitle).HasMaxLength(200);
            entity.Property(e => e.ProposalURL).HasMaxLength(500);
            entity.Property(e => e.ProposedAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Rate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SentOn).HasColumnType("datetime");
            entity.Property(e => e.VersionNo).HasDefaultValue(1);
        });

        modelBuilder.Entity<ProposalActivity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK__Proposal__45F4A7911DB22EAD");

            entity.ToTable("ProposalActivities", "CRM");

            entity.Property(e => e.ActivityId).ValueGeneratedNever();
            entity.Property(e => e.ActionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ActivityType).HasMaxLength(100);
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Ratings__FCCDF87C6DDB834B");

            entity.ToTable("Ratings", "CRM");

            entity.Property(e => e.RatingId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RatingName).HasMaxLength(100);
        });

        modelBuilder.Entity<Referral>(entity =>
        {
            entity.HasKey(e => e.ReferralId).HasName("PK__Referral__A2C4A96636EA409A");

            entity.ToTable("Referrals", "CRM");

            entity.Property(e => e.ReferralId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReferredEmail).HasMaxLength(200);
            entity.Property(e => e.ReferredName).HasMaxLength(200);
            entity.Property(e => e.ReferredPhone).HasMaxLength(20);
            entity.Property(e => e.Status).HasDefaultValueSql("('New')");

            entity.HasOne(d => d.ReferrerAccount).WithMany(p => p.Referrals)
                .HasForeignKey(d => d.ReferrerAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Referral_Account");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("PK__Regions__ACD844A3FBE2727E");

            entity.ToTable("Regions", "CRM");

            entity.Property(e => e.RegionId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RegionCode).HasMaxLength(20);
            entity.Property(e => e.RegionName).HasMaxLength(150);
        });

        modelBuilder.Entity<RejectReasonMaster>(entity =>
        {
            entity.HasKey(e => e.RejectReasonId).HasName("PK__RejectRe__1D8829121973BDFE");

            entity.ToTable("RejectReasonMaster", "CRM");

            entity.Property(e => e.RejectReasonId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DisplayOrder).HasDefaultValue(1);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModuleType).HasMaxLength(50);
            entity.Property(e => e.ReasonCode).HasMaxLength(50);
            entity.Property(e => e.ReasonName).HasMaxLength(200);
            entity.Property(e => e.StageType).HasMaxLength(50);
        });

        modelBuilder.Entity<SalutationMaster>(entity =>
        {
            entity.HasKey(e => e.SalutationId).HasName("PK__Salutati__562AE14F4F6E81EA");

            entity.ToTable("SalutationMaster", "CRM");

            entity.Property(e => e.SalutationId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.SalutationName).HasMaxLength(50);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB00A5224803D");

            entity.ToTable("Services", "CRM");

            entity.Property(e => e.ServiceId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ServiceName).HasMaxLength(200);
        });

        modelBuilder.Entity<ServicePricing>(entity =>
        {
            entity.HasKey(e => e.ServicePricingId).HasName("PK__ServiceP__506451BEB2A78A83");

            entity.ToTable("ServicePricing", "CRM");

            entity.Property(e => e.ServicePricingId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.HighPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LowPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Currency).WithMany(p => p.ServicePricings)
                .HasForeignKey(d => d.CurrencyId)
                .HasConstraintName("FK_ServicePricing_Currencies");

            entity.HasOne(d => d.Service).WithMany(p => p.ServicePricings)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServicePricing_Services");
        });

        modelBuilder.Entity<ServiceUserRoleMap>(entity =>
        {
            entity.ToTable("ServiceUserRoleMap", "CRM");

            entity.Property(e => e.AssignedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<WinLossReason>(entity =>
        {
            entity.HasKey(e => e.WinLossReasonId).HasName("PK__WinLossR__1FBCDDF676413C61");

            entity.ToTable("WinLossReasons", "CRM");

            entity.Property(e => e.WinLossReasonId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ReasonName).HasMaxLength(200);
            entity.Property(e => e.ReasonType).HasMaxLength(50);
        });

        modelBuilder.Entity<WorkOrder>(entity =>
        {
            entity.HasKey(e => e.WorkOrderId).HasName("PK__WorkOrde__AE75511552976680");

            entity.ToTable("WorkOrders", "CRM");

            entity.Property(e => e.WorkOrderId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.ActualHours).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ApprovalToken).HasMaxLength(200);
            entity.Property(e => e.ClientEmail).HasMaxLength(200);
            entity.Property(e => e.ClientName).HasMaxLength(200);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EstimatedHours).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FinanceApproved).HasDefaultValue(false);
            entity.Property(e => e.FinanceApprovedOn).HasColumnType("datetime");
            entity.Property(e => e.FinanceRemarks).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.PaymentLinkSent).HasDefaultValue(false);
            entity.Property(e => e.PaymentLinkSentOn).HasColumnType("datetime");
            entity.Property(e => e.PaymentLinkURL).HasMaxLength(200);
            entity.Property(e => e.PaymentReceivedOn).HasColumnType("datetime");
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.ProjectCreated).HasDefaultValue(false);
            entity.Property(e => e.ProjectCreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ProjectEndDate).HasColumnType("datetime");
            entity.Property(e => e.ProjectStartDate).HasColumnType("datetime");
            entity.Property(e => e.ReasonForWONonAccepence).HasMaxLength(500);
            entity.Property(e => e.WOAcceptedOn).HasColumnType("datetime");
            entity.Property(e => e.WORejectedOn).HasColumnType("datetime");
            entity.Property(e => e.WOSentOn).HasColumnType("datetime");
            entity.Property(e => e.WorkOrderNumber).HasMaxLength(50);
            entity.Property(e => e.WorkOrderStatus).HasMaxLength(50);
            entity.Property(e => e.WorkOrderURL).HasMaxLength(200);
        });

        modelBuilder.Entity<WorkOrderActivity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK__WorkOrde__45F4A791AD774D42");

            entity.ToTable("WorkOrderActivities", "CRM");

            entity.Property(e => e.ActivityId).ValueGeneratedNever();
            entity.Property(e => e.ActionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ActivityByRole).HasMaxLength(100);
            entity.Property(e => e.ActivityType).HasMaxLength(100);
            entity.Property(e => e.NewStatus).HasMaxLength(50);
            entity.Property(e => e.OldStatus).HasMaxLength(50);

            entity.HasOne(d => d.WorkOrder).WithMany(p => p.WorkOrderActivities)
                .HasForeignKey(d => d.WorkOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkOrder__WorkO__75AD65ED");
        });

        modelBuilder.Entity<WorkflowStep>(entity =>
        {
            entity.HasKey(e => e.StepId).HasName("PK__Workflow__2434335787E1DD9F");

            entity.ToTable("WorkflowSteps", "CRM");

            entity.Property(e => e.StepId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IsMandatory).HasDefaultValue(true);
            entity.Property(e => e.RoleId).HasMaxLength(450);

            entity.HasOne(d => d.Workflow).WithMany(p => p.WorkflowSteps)
                .HasForeignKey(d => d.WorkflowId)
                .HasConstraintName("FK_WorkflowSteps_Workflow");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
