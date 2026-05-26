using BSuit.Core.Data;
using BSuit.Identity.Data;
using BSuit.SalesCRM.Data;
using BSuit.SalesCRM.Entities;
using BSuit.SalesCRM.Services.ILeadService;
using BSuit.SalesCRM.VM.Lead;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.SalesCRM.Services.LeadService
{
    public class LeadService : ILeadService.ILeadService
    {
        private readonly SalesCRMContext _context;
        private readonly CoreDbContext _coreContext;
        private readonly IdentityDbContext _identityDbContext;


        public LeadService(SalesCRMContext db, CoreDbContext coreDbContext, IdentityDbContext identityDbContext)
        {
            _context = db;
            _coreContext = coreDbContext;
            _identityDbContext = identityDbContext;
        }

        public async Task<IEnumerable<Entities.Lead>> GetAllLeadAsync(Guid tenantId)
        {
            return await _context.Leads.Where(p => p.TenantId == tenantId).ToListAsync();
        }

        public async Task<Entities.Lead?> GetLeadByIdAsync(Guid tenantId, Guid leadId)
        {
            return await _context.Leads.FirstOrDefaultAsync(p => p.TenantId == tenantId && p.LeadId == leadId);
        }

        public async Task<List<LeadInfo>> GetLeadList(Guid tenantId)
        {
            var data = await (
     from l in _context.Leads.AsNoTracking()

     join c in _context.Countries
         on l.CountryId equals c.CountryId into cj
     from c in cj.DefaultIfEmpty()

     join cs in _context.CompanySizes
         on l.CompanySizeId equals cs.LeadCompanySizeId into csj
     from cs in csj.DefaultIfEmpty()

     join r in _context.Ratings
         on l.RatingId equals r.RatingId into rj
     from r in rj.DefaultIfEmpty()

     join lp in _context.Priorities
         on l.LeadPriorityId equals lp.PriorityId into lpj
     from lp in lpj.DefaultIfEmpty()

     join g in _context.GenderMasters
         on l.GenderId equals g.GenderId into gj
     from g in gj.DefaultIfEmpty()

     join jt in _context.JobTitles
         on l.JobTitleId equals jt.JobTitleId into jtj
     from jt in jtj.DefaultIfEmpty()

     join i in _context.Industries
         on l.IndustryId equals i.IndustryId into ij
     from i in ij.DefaultIfEmpty()

     join lt in _context.LeadTypes
         on l.LeadTypeId equals lt.LeadTypeId into ltj
     from lt in ltj.DefaultIfEmpty()

     join ls in _context.LeadSources
         on l.LeadSourceId equals ls.LeadSourceId into lsj
     from ls in lsj.DefaultIfEmpty()

     join st in _context.LeadStages
         on l.LeadStageId equals st.LeadStageId into stj
     from st in stj.DefaultIfEmpty()

         // 🔥 LEAD REJECTION
     join rr in _context.LeadRejections
         on l.LeadId equals rr.LeadId into rrj
     from hmg in rrj.DefaultIfEmpty()

         // 🔥 REJECT REASON MASTER
     join mm in _context.RejectReasonMasters
         on (hmg != null
             ? hmg.RejectReasonId
             : Guid.Empty)
         equals mm.RejectReasonId into mmj
     from hko in mmj.DefaultIfEmpty()

     where l.TenantId == tenantId
           && l.IsActive == true

     orderby l.CreatedOn descending

     select new
     {
         l,

         Country = c != null ? c.CountryName : null,
         CompanySize = cs != null ? cs.SizeName : null,
         Rating = r != null ? r.RatingName : null,
         LeadPriority = lp != null ? lp.PriorityName : null,
         Gender = g != null ? g.GenderName : null,
         JobTitle = jt != null ? jt.JobTitleName : null,
         Industry = i != null ? i.IndustryName : null,
         LeadType = lt != null ? lt.LeadTypeName : null,
         LeadSource = ls != null ? ls.SourceName : null,
         LeadStage = st != null ? st.StageName : null,

         OwnerId = l.OwnerId,

         // 🔥 NEW FIELDS
         Rejection = hko != null
             ? hko.ReasonName
             : null,

         RejectionRemarks = hmg != null
             ? hmg.RejectionRemarks
             : null,
         IsLeadRejectionApproved = hmg != null
             ? hmg.IsApproved
             : null,
         LeadRejectionApprovedDate = hmg != null
             ? hmg.ApprovedDate
             : null,
         LeadRejectionApprovedBy = hmg != null
             ? hmg.ApprovedBy
             : null

     }

 ).ToListAsync();

            // 🔹 Fetch Users
            var userIds = data
                .Where(x => x.OwnerId != null)
                .Select(x => x.OwnerId.ToString())
                .Distinct()
                .ToList();

            var leadIDS = data
             .Select(x => x.l.LeadId)
             .Distinct()
             .AsQueryable();

            var leadServiceMapping = await _context.LeadServiceMappings
      .Where(m => m.TenantId == tenantId && leadIDS.Contains(m.LeadId))
      .Join(_context.Services,
          m => m.ServiceId,
          s => s.ServiceId,
          (m, s) => new LeadServiceList
          {
              LeadId = m.LeadId,
              ServiceId = m.ServiceId,
              ServiceName = s.ServiceName
          })
      .ToListAsync();


            // 🔥 ADD THIS LINE (VERY IMPORTANT)
            userIds = userIds.AsEnumerable().ToList();

            var users = await _identityDbContext.Users
                .Where(u => userIds.Contains(u.UserId.ToString()))
                .ToDictionaryAsync(u => u.UserId.ToString(), u => u.FullName);


            // 🔹 Map to LeadInfo List
            var result = data.Select(x => new LeadInfo
            {
                LeadId = x.l.LeadId,
                TeanantId = x.l.TenantId ?? Guid.Empty,

                FirstName = x.l.FirstName ?? string.Empty,
                LastName = x.l.LastName ?? string.Empty,
                EnquiryId = x.l.EnquiryId ?? string.Empty,
                Gender = x.Gender,
                JobTitle = x.JobTitle,

                Email = x.l.Email ?? string.Empty,
                Phone = x.l.Phone ?? string.Empty,
                PersonalEmail1 = x.l.PersonalEmail1 ?? string.Empty,
                PersonalEmail2 = x.l.PersonalEmail2 ?? string.Empty,
                Address = x.l.Address ?? string.Empty,
                City = x.l.City ?? string.Empty,
                ZipCode = x.l.ZipCode ?? string.Empty,
                Country = x.Country,

                CompanyName = x.l.CompanyName ?? string.Empty,
                Website = x.l.Website ?? string.Empty,
                CompanySize = x.CompanySize,
                CompanyRevenue = x.l.CompanyRevenue ?? string.Empty,
                CompanyRanking = x.l.CompanyRanking ?? string.Empty,
                Industry = x.Industry,

                RequirementDetails = x.l.RequirementDetails ?? string.Empty,
                CustomerBackground = x.l.CustomerBackground ?? string.Empty,
                SkpeId = x.l.SkpeId ?? string.Empty,
                TwitterURL = x.l.TwitterURL ?? string.Empty,
                FacebookURL = x.l.FacebookURL ?? string.Empty,

                Rating = x.Rating,
                LeadPriority = x.LeadPriority,
                LeadType = x.LeadType,
                LeadSource = x.LeadSource,
                LeadStage = x.LeadStage,

                SalesExecutive = (x.OwnerId != null && users.ContainsKey(x.OwnerId.ToString()))
                    ? users[x.OwnerId.ToString()]
                    : null,

                IsActive = x.l.IsActive ?? false,
                RejectionReason = x.Rejection,
                RejectionRemarks = x.RejectionRemarks,
                IsLeadRejectionApproved = x.IsLeadRejectionApproved ?? false,
                LeadRejectionApprovedDate = x.LeadRejectionApprovedDate,
                LeadRejectionApprovedBy = x.LeadRejectionApprovedBy.ToString(),
                LeadCreatedOn = x.l.CreatedOn,
                SelectedServices = leadServiceMapping.Where(s => s.LeadId == x.l.LeadId).ToList(),
                LeadOwnerId = x.OwnerId
    
            }).ToList();

            return result;
        }

        public async Task<LeadInfo> GetLeadDetails(Guid leadId)
        {
            var data = await (
      from l in _context.Leads.AsNoTracking()

      join c in _context.Countries
          on l.CountryId equals c.CountryId into cj
      from c in cj.DefaultIfEmpty()

      join cs in _context.CompanySizes
          on l.CompanySizeId equals cs.LeadCompanySizeId into csj
      from cs in csj.DefaultIfEmpty()

      join r in _context.Ratings
          on l.RatingId equals r.RatingId into rj
      from r in rj.DefaultIfEmpty()

      join lp in _context.Priorities
          on l.LeadPriorityId equals lp.PriorityId into lpj
      from lp in lpj.DefaultIfEmpty()

      join g in _context.GenderMasters
          on l.GenderId equals g.GenderId into gj
      from g in gj.DefaultIfEmpty()

      join jt in _context.JobTitles
          on l.JobTitleId equals jt.JobTitleId into jtj
      from jt in jtj.DefaultIfEmpty()

      join i in _context.Industries
          on l.IndustryId equals i.IndustryId into ij
      from i in ij.DefaultIfEmpty()

      join lt in _context.LeadTypes
          on l.LeadTypeId equals lt.LeadTypeId into ltj
      from lt in ltj.DefaultIfEmpty()

      join ls in _context.LeadSources
          on l.LeadSourceId equals ls.LeadSourceId into lsj
      from ls in lsj.DefaultIfEmpty()

      join st in _context.LeadStages
          on l.LeadStageId equals st.LeadStageId into stj
      from st in stj.DefaultIfEmpty()

      where l.LeadId == leadId
            && l.IsActive == true

      select new
      {
          l,

          Country = c != null ? c.CountryName : null,
          CompanySize = cs != null ? cs.SizeName : null,
          Rating = r != null ? r.RatingName : null,
          LeadPriority = lp != null ? lp.PriorityName : null,
          Gender = g != null ? g.GenderName : null,
          JobTitle = jt != null ? jt.JobTitleName : null,
          Industry = i != null ? i.IndustryName : null,
          LeadType = lt != null ? lt.LeadTypeName : null,
          LeadSource = ls != null ? ls.SourceName : null,
          LeadStage = st != null ? st.StageName : null,

          OwnerId = l.OwnerId
      }

  ).FirstOrDefaultAsync();

            if (data == null)
                return null;

            // 🔹 Sales Executive
            string salesExecutive = null;

            if (data.OwnerId != null)
            {
                salesExecutive = await _identityDbContext.Users
                    .Where(u => u.Id == data.OwnerId.ToString())
                    .Select(u => u.UserName)
                    .FirstOrDefaultAsync();
            }

            // 🔹 Services
            var leadServiceMapping = await _context.LeadServiceMappings
                .Where(m => m.LeadId == leadId)
                .Join(_context.Services,
                    m => m.ServiceId,
                    s => s.ServiceId,
                    (m, s) => new LeadServiceList
                    {
                        LeadId = m.LeadId,
                        ServiceId = m.ServiceId,
                        ServiceName = s.ServiceName
                    })
                .ToListAsync();

            // ============================
            // 🔥 EMAIL COMMUNICATION LOGIC
            // ============================

            var emailMessages = await _context.EmailMessages
                .Where(m => m.ParentEntityId == leadId)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();

            var emailIds = emailMessages.Select(e => e.EmailId).ToList();

            var emailRecipients = await _context.EmailRecipients
                .Where(r => emailIds.Contains(r.EmailId))
                .ToListAsync();


            var leadDocs = _context.LeadAttachments.Where(m => m.LeadId == leadId);

            List<Documents> leadDocsList = new List<Documents>();
            if (leadDocs.Any())
            {
                foreach (var i in leadDocs)
                {
                    leadDocsList.Add(new Documents
                    {
                        FileName = i.FileName,
                        UploadedOn = Convert.ToDateTime(i.CreatedOn),
                        FilePath = i.FilePath
                    });


                }
            }

            // ============================
            // 🔥 OPPORTUNITY LOGIC
            // ============================

            var leadOpportunities = await (
                from o in _context.Opportunities

                join os in _context.OpportunityServices
                    on o.OpportunityId equals os.OpportunityId

                join ser in _context.Services
               on os.ServiceId equals ser.ServiceId

                join st in _context.OpportunityStages
                    on os.OpportunityStageId equals st.StageId into stj
                from st in stj.DefaultIfEmpty()

                where os.LeadId == leadId

                select new LeadOpportunities
                {
                    Id = o.OpportunityId,
                    Name = o.OpportunityName,
                    ServiceId = os.ServiceId ?? Guid.NewGuid(),
                    ServiceName = ser.ServiceName,
                    OpportunityStage = st != null ? st.StageName : "Interim",
                    RequestedOn = o.CreatedOn ?? DateTime.MinValue,
                    SubmittedOn = os.ScoperUpdatedOn,
                    ApprovedOn = null,
                    Scoper = null,
                    WorkFlowId = os.WorkflowId,
                    DeliveryManager = null // update later if you have mapping
                }
            ).OrderByDescending(x => x.RequestedOn)
             .ToListAsync();

            var clientCommunication = emailMessages.Select(email => new ClientCommunication
            {
                EmailId = email.EmailId,
                Subject = email.Subject,

                BodyPreview = !string.IsNullOrEmpty(email.BodyHtml)
                    ? (email.BodyHtml.Length > 100
                        ? email.BodyHtml.Substring(0, 100) + "..."
                        : email.BodyHtml)
                    : "",

                SentDate = email.SentDate,
                Status = email.Status,

                ToEmails = string.Join(", ",
                    emailRecipients
                        .Where(r => r.EmailId == email.EmailId && r.Type == "To")
                        .Select(r => r.Address)),

                CcEmails = string.Join(", ",
                    emailRecipients
                        .Where(r => r.EmailId == email.EmailId && r.Type == "CC")
                        .Select(r => r.Address)),

                BccEmails = string.Join(", ",
                    emailRecipients
                        .Where(r => r.EmailId == email.EmailId && r.Type == "BCC")
                        .Select(r => r.Address))

            }).ToList();

            var reasonDetails = _context.LeadRejections.FirstOrDefault(p => p.LeadId == data.l.LeadId);
            string reasonName = "";
            string reasonRemarks = "";
            if (reasonDetails != null)
            {
                reasonName = _context.RejectReasonMasters.Find(reasonDetails.RejectReasonId)?.ReasonName;
                reasonRemarks = reasonDetails.RejectionRemarks;
            }

            // ============================
            // 🔹 FINAL MAPPING
            // ============================

            var result = new LeadInfo
            {
                LeadId = data.l.LeadId,
                TeanantId = data.l.TenantId ?? Guid.Empty,

                FirstName = data.l.FirstName ?? string.Empty,
                LastName = data.l.LastName ?? string.Empty,
                EnquiryId = data.l.EnquiryId ?? string.Empty,
                Gender = data.Gender,
                JobTitle = data.JobTitle,

                Email = data.l.Email ?? string.Empty,
                Phone = data.l.Phone ?? string.Empty,
                PersonalEmail1 = data.l.PersonalEmail1 ?? string.Empty,
                PersonalEmail2 = data.l.PersonalEmail2 ?? string.Empty,
                Address = data.l.Address ?? string.Empty,
                City = data.l.City ?? string.Empty,
                ZipCode = data.l.ZipCode ?? string.Empty,
                Country = data.Country,

                CompanyName = data.l.CompanyName ?? string.Empty,
                Website = data.l.Website ?? string.Empty,
                CompanySize = data.CompanySize,
                CompanyRevenue = data.l.CompanyRevenue ?? string.Empty,
                CompanyRanking = data.l.CompanyRanking ?? string.Empty,
                Industry = data.Industry,

                RequirementDetails = data.l.RequirementDetails ?? string.Empty,
                CustomerBackground = data.l.CustomerBackground ?? string.Empty,
                SkpeId = data.l.SkpeId ?? string.Empty,
                TwitterURL = data.l.TwitterURL ?? string.Empty,
                FacebookURL = data.l.FacebookURL ?? string.Empty,

                Rating = data.Rating,
                LeadPriority = data.LeadPriority,
                LeadType = data.LeadType,
                LeadSource = data.LeadSource,
                LeadStage = data.LeadStage,

                SalesExecutive = salesExecutive,

                IsActive = data.l.IsActive ?? false,
                LeadCreatedOn = data.l.CreatedOn,
                RejectionReason = reasonName,
                RejectionRemarks = reasonRemarks,

                SelectedServices = leadServiceMapping,
                EmailHistory = clientCommunication,
                LeadOpportunities = leadOpportunities,
                LeadDocuments = leadDocsList
            };

            return result;
        }


        public async Task<LeadEditVM> GetLeadEditData(
    Guid tenantId,
    Guid leadId)
        {
            // ============================================
            // CREATE MODE
            // ============================================

            if (leadId == Guid.Empty)
            {
                return new LeadEditVM
                {
                    LeadId = Guid.Empty,
                    TeanantId = tenantId,

                    // DEFAULT VALUES
                    IsActive = true,

                    // DROPDOWNS
                    GenderList = await GetGenderList(),
                    IndustryList = await GetIndustryList(),
                    CompanySizeList = await GetCompanySizeList(),
                    LeadTypeList = await GetLeadTypeList(),
                    LeadSourceList = await GetLeadSourceList(),
                    LeadStageList = await GetLeadStageList(),
                    LeadPriorityList = await GetLeadPriorityList(),
                    RatingList = await GetRatingList(),
                    SalesExecutiveList = await GetSalesExecutiveList(),
                    CountryMaster = await GetCountryList(),
                    
                    JobTitleList = await GetJobTitlesList(),
                    CustomerTypeList = await GetCustomerTypeList(),
                    // EMPTY SERVICES
                    SelectedServices = new List<LeadServiceList>()
                };
            }

            // ============================================
            // EDIT MODE
            // ============================================

            var lead = await _context.Leads
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.TenantId == tenantId &&
                    x.LeadId == leadId);

            if (lead == null)
                return null;

            var leadServiceMapping =
                await _context.LeadServiceMappings
                    .Where(m => m.LeadId == leadId)
                    .Join(_context.Services,
                        m => m.ServiceId,
                        s => s.ServiceId,
                        (m, s) => new LeadServiceList
                        {
                            LeadId = m.LeadId,
                            ServiceId = m.ServiceId,
                            ServiceName = s.ServiceName
                        })
                    .ToListAsync();

            var vm = new LeadEditVM
            {
                // IDENTIFIERS
                LeadId = lead.LeadId,
                TeanantId = lead.TenantId ?? Guid.Empty,

                // BASIC INFO
                FirstName = lead.FirstName,
                LastName = lead.LastName,
                EnquiryId = lead.EnquiryId,
                Gender = lead.GenderId,
                Country = lead.CountryId,
                BDComments = lead.BDComments,
                CustomerType = lead.CustomerTypeId,
                LinkedinURL = lead.LinkedinURL,

                // CONTACT INFO
                Email = lead.Email,
                Phone = lead.Phone,
                PersonalEmail1 = lead.PersonalEmail1,
                PersonalEmail2 = lead.PersonalEmail2,
                Address = lead.Address,
                City = lead.City,
                ZipCode = lead.ZipCode,
                JobTitle = lead.JobTitleId,
                // COMPANY INFO
                CompanyName = lead.CompanyName,
                Website = lead.Website,
                CompanySize = lead.CompanySizeId,
                CompanyRevenue = lead.CompanyRevenue,
                CompanyRanking = lead.CompanyRanking,
                Industry = lead.IndustryId,

                // LEAD INFO
                LeadType = lead.LeadTypeId,
                LeadSource = lead.LeadSourceId,
                LeadStage = lead.LeadStageId,
                LeadPriority = lead.LeadPriorityId,
                Rating = lead.RatingId,
                SalesExecutive = lead.OwnerId,

                // ADDITIONAL INFO
                RequirementDetails = lead.RequirementDetails,
                CustomerBackground = lead.CustomerBackground,
                SkpeId = lead.SkpeId,
                TwitterURL = lead.TwitterURL,
                FacebookURL = lead.FacebookURL,
                PreferredModeofCommunication = lead.PreferredModeofCommunication,
                

                // STATUS
                IsActive = lead.IsActive ?? false,

                // SERVICES
                SelectedServices = leadServiceMapping,

                // DROPDOWNS
                GenderList = await GetGenderList(),
                IndustryList = await GetIndustryList(),
                CompanySizeList = await GetCompanySizeList(),
                LeadTypeList = await GetLeadTypeList(),
                LeadSourceList = await GetLeadSourceList(),
                LeadStageList = await GetLeadStageList(),
                LeadPriorityList = await GetLeadPriorityList(),
                RatingList = await GetRatingList(),
                SalesExecutiveList = await GetSalesExecutiveList(),
                CountryMaster = await GetCountryList(),
                JobTitleList = await GetJobTitlesList(),
                CustomerTypeList = await GetCustomerTypeList()
            };

            return vm;
        }

        public async Task<LeadConfigurationVM> LeadConfigurationVM(Guid tenantId)
        {
            var regions = await GetRegionList();
            var salesExec = await GetSalesExecutiveList(tenantId);
            var services = await GetServiceList();

            var configs = await _context.LeadConfigurations
                .Include(x => x.Region)
                .Include(x => x.Service)
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync();

            return new LeadConfigurationVM
            {
                Regions = regions,
                SalesExecutives = salesExec,
                Services = services,
                Configurations = configs
            };
        }

        public async Task<List<SelectListItem>> GetRegionList()
        {
            return await _context.Regions
                .Select(x => new SelectListItem
                {
                    Value = x.RegionId.ToString(),
                    Text = x.RegionName
                }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetServiceList()
        {
            return await _context.Services
                .Select(x => new SelectListItem
                {
                    Value = x.ServiceId.ToString(),
                    Text = x.ServiceName
                }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetSalesExecutiveList(Guid tenantId)
        {
            // 🔹 Get Sales Executive Role Id
            var saleExeRoleID = await _identityDbContext.Roles
                .Where(r => r.Name == "Sales Executive")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(saleExeRoleID))
                return new List<SelectListItem>();

            // 🔹 Join Users + UserRoles
            var salesExecutives = await (
                from user in _identityDbContext.Users
                join ur in _identityDbContext.UserRoles
                    on user.Id equals ur.UserId
                where user.TenantId == tenantId
                      && ur.RoleId == saleExeRoleID
                select new SelectListItem
                {
                    Value = user.Id,              // ⚠️ use Id (not UserId unless custom)
                    Text = user.FullName
                }
            ).ToListAsync();

            return salesExecutives;
        }

        public async Task<List<SelectListItem>> GetRatingList()
        {
            return await _context.Ratings
                .Select(x => new SelectListItem
                {
                    Value = x.RatingId.ToString(),
                    Text = x.RatingName
                }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetLeadTypeList()
        {
            return await _context.LeadTypes
                .Select(x => new SelectListItem
                {
                    Value = x.LeadTypeId.ToString(),
                    Text = x.LeadTypeName
                }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetLeadStageList()
        {
            return await _context.LeadStages
                .Select(x => new SelectListItem
                {
                    Value = x.LeadStageId.ToString(),
                    Text = x.StageName
                }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetLeadPriorityList()
        {
            return await _context.Priorities
                .Select(x => new SelectListItem
                {
                    Value = x.PriorityId.ToString(),
                    Text = x.PriorityName
                }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetIndustryList()
        {
            return await _context.Industries
                .Select(x => new SelectListItem
                {
                    Value = x.IndustryId.ToString(),
                    Text = x.IndustryName
                }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCompanySizeList()
        {
            return await _context.CompanySizes
                .Select(x => new SelectListItem
                {
                    Value = x.LeadCompanySizeId.ToString(),
                    Text = x.SizeName
                }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetGenderList()
        {
            return await _context.GenderMasters
                .Select(x => new SelectListItem
                {
                    Value = x.GenderId.ToString(),
                    Text = x.GenderName
                }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetLeadSourceList()
        {
            return await _context.LeadSources
                .Select(x => new SelectListItem
                {
                    Value = x.LeadSourceId.ToString(),
                    Text = x.SourceName
                }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetSalesExecutiveList()
        {
            var roles = new[]
            {
        "Sales Executive",
        "Sales Manager"
    };

            var users = await (
                from user in _identityDbContext.Users
                join userRole in _identityDbContext.UserRoles
                    on user.Id equals userRole.UserId
                join role in _identityDbContext.Roles
                    on userRole.RoleId equals role.Id
                where roles.Contains(role.Name)
                select new SelectListItem
                {
                    Value = user.Id.ToString(),
                    Text = user.UserName
                }
            )
            .Distinct()
            .ToListAsync();

            return users;
        }

        public async Task<List<SelectListItem>> GetJobTitlesList()
        {
            return await _context.JobTitles
                .Select(u => new SelectListItem
                {
                    Value = u.JobTitleId.ToString(),
                    Text = u.JobTitleName
                }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCountryList()
        {
            return await _context.Countries
                .Select(u => new SelectListItem
                {
                    Value = u.CountryId.ToString(),
                    Text = u.CountryName
                }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCustomerTypeList()
        {
            return await _context.CustomerTypes
                .Select(u => new SelectListItem
                {
                    Value = u.CustomerTypeId.ToString(),
                    Text = u.CustomerTypeName
                }).ToListAsync();
        }

        public async Task<List<LeadStage>> GetLeadStages()
        {
            return await _context.LeadStages.ToListAsync();
        }

        public async Task SaveLeadConfigurationAsync(List<LeadConfiguration> data)
        {
            if (data == null || !data.Any())
                return;

            await _context.LeadConfigurations.AddRangeAsync(data);
            await _context.SaveChangesAsync();
        }


        public async Task<List<SelectListItem>> GetLeadRejectionReasonsList()
        {
            return await _context.RejectReasonMasters
                .Where(p => p.IsActive == true)
                .Select(u => new SelectListItem
                {
                    Value = u.RejectReasonId.ToString(),
                    Text = u.ReasonName
                }).ToListAsync();
        }
    }
}
