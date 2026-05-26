using BSuit.Contracts.Services;
using BSuit.Identity.Data;
using BSuit.SalesCRM.Data;
using BSuit.SalesCRM.Entities;
using BSuit.SalesCRM.Services.LeadService;
using BSuit.SalesCRM.VM._Opportunity_;
using BSuit.SalesCRM.VM.Lead;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Stripe;
using System.Globalization;
using System.Linq;

namespace BSuit.API.Areas.SalesCRM.Controllers
{
    [Area("SalesCRM")]

    public class OpportunityController : Controller
    {
        private readonly SalesCRMContext _context;
        private readonly IdentityDbContext _identityDbContext;
        private readonly IUserContext _IUserContext;

        public OpportunityController(SalesCRMContext context, IdentityDbContext identityDbContext, IUserContext iUserContext)
        {
            _context = context;
            _identityDbContext = identityDbContext;
            _IUserContext = iUserContext;
        }

        public async Task<IActionResult> OpportunityScoperView(
    int pageNumber = 1,
    int pageSize = 5,
    string startDate = null,
    string endDate = null,
    string searchText = null,
    string[] serviceIds = null,
    string stageId = null
)
        {
            var tenantId = Guid.Parse("6200E0AE-F4C7-4509-B618-DC3490EE88D1");

            Guid scoperRoleId = Guid.Parse("ce8bb0b8-842b-45b6-85f0-5db039ac2f51");
            Guid salesRoleId = Guid.Parse("7bd843ef-9865-47ac-b6c3-9de8447a38d3");

            // =========================
            // USERS
            // =========================
            var users = await _identityDbContext.Users
                .Select(u => new
                {
                    u.Id,
                    u.UserId,
                    u.FullName
                })
                .ToListAsync();

            // =========================
            // ROLE MAPS
            // =========================
            var roleMaps = await _context.ServiceUserRoleMaps
                .Where(x => x.RoleId == scoperRoleId || x.RoleId == salesRoleId)
                .ToListAsync();

            var scoperMapDict = roleMaps
                .Where(x => x.RoleId == scoperRoleId)
                .GroupBy(x => x.ServiceId)
                .ToDictionary(x => x.Key, x => x.FirstOrDefault());

            var salesMapDict = roleMaps
                .Where(x => x.RoleId == salesRoleId)
                .GroupBy(x => x.ServiceId)
                .ToDictionary(x => x.Key, x => x.FirstOrDefault());


            // =========================
            // BASE QUERY
            // =========================
            var query =
    from o in _context.Opportunities

    join s in _context.OpportunityServices
        on o.OpportunityId equals s.OpportunityId

    join sr in _context.Services
        on s.ServiceId equals sr.ServiceId

    join l in _context.Leads
        on s.LeadId equals l.LeadId

    join os in _context.OpportunityStages
        on s.OpportunityStageId equals os.StageId

    where o.TenantId == tenantId

    select new
    {
        o,
        s,
        sr,
        os,
        l,

        // LAST APPROVED TRANSACTION
        LastApproved =
            _context.ApprovalTransactions
                .Where(a =>
                    a.RecordId == s.OpportunityServiceId 
                    //&&
                    //a.ApprovalStatus == "Approved"
                    )
                .OrderByDescending(a => a.ActionDate)
                .FirstOrDefault(),

        Module = _context.OpportunityModules
            .Where(m => m.ServiceId == s.OpportunityServiceId)
            .OrderByDescending(m => m.CreatedOn)
            .FirstOrDefault()
    };
            // =========================
            // ROLE FILTER
            // =========================

            var currentRoleIds = _IUserContext.RoleIds;
            var identityUsers = await _identityDbContext.Users
    .Select(u => new
    {
        u.Id,
        u.FullName
    })
    .ToListAsync();
            if (currentRoleIds == null || !currentRoleIds.Any())
            {
                query = query.Where(x => false);
            }
            else
            {
                query = query.Where(x =>
                    _context.ServiceUserRoleMaps.Any(r =>
                        r.ServiceId == x.s.ServiceId &&
                        currentRoleIds.Contains(r.RoleId)));
            }
            var roles = await _identityDbContext.Roles
    .Select(r => new
    {
        r.Id,
        r.Name
    })
    .ToListAsync();
            var raw = await query.ToListAsync();


            // =========================
            // TENANT FILTER
            // =========================
            query = query.Where(x => x.o.TenantId == tenantId);


            // =========================
            var data = raw.Select(x =>
            {
                Guid serviceId = x.s?.ServiceId ?? Guid.Empty;

                scoperMapDict.TryGetValue(serviceId, out var scoperMap);

                salesMapDict.TryGetValue(serviceId, out var salesMap);

                var scoperUser = users.FirstOrDefault(u =>
                    x.s.ScoperId != null &&
                    u.UserId == x.s.ScoperId);

                var salesUser = users.FirstOrDefault(u =>
                    salesMap != null &&
                    u.Id == salesMap.UserId.ToString());

                // =========================
                // APPROVED ROLE
                // =========================
                string lastApprovedRole = "-";

                // =========================
                // APPROVED BY USER
                // =========================
                string approvedByName = "-";

                if (x.LastApproved != null)
                {
                    // ROLE
                    var workflowStep = _context.WorkflowSteps
                        .FirstOrDefault(w =>
                            w.StepId == x.LastApproved.StepId);

                    if (workflowStep != null)
                    {
                        lastApprovedRole = roles
                            .FirstOrDefault(r =>
                                r.Id.ToString() == workflowStep.RoleId)
                            ?.Name ?? "-";
                    }

                    // USER
                    if (x.LastApproved.ApprovedBy != null)
                    {
                        approvedByName = identityUsers
                            .FirstOrDefault(u =>
                                u.Id == x.LastApproved.ApprovedBy.ToString())
                            ?.FullName ?? "-";
                    }
                }

                return new OpportunityVM
                {
                    OpportunityId = x.o.OpportunityId,

                    OpportunityName = x.o.OpportunityName,

                    CreatedOn = x.o.CreatedOn,

                    OpportunityStage = x.os?.StageName ?? "-",

                    OpportunityStageId =
                        x.s?.OpportunityStageId ?? Guid.Empty,

                    EnquiryId =
                        x.l?.EnquiryId.ToString() ?? "-",

                    CustomerId =
                        x.l?.EnquiryId.ToString() ?? "-",

                    EstimatedOn = x.Module?.CreatedOn,

                    ServiceId = serviceId,

                    ServiceName = x.sr?.ServiceName ?? "-",

                    LeadName =
                        $"{x.l?.FirstName} {x.l?.LastName}",

                    ScoperStatus =
                        x.s?.ScoperStatus ?? "Draft",

                    ApprovalStatus =
                        x.LastApproved?.ApprovalStatus ?? "-",

                    ApprovedRole = lastApprovedRole,

                    ApprovedByName = approvedByName,

                    Scoper = scoperUser?.FullName ?? "-",

                    SlaesExecutive = salesUser?.FullName ?? "-"
                };

            }).ToList();
            // =========================
            // FILTERS
            // =========================

            if (serviceIds?.Length > 0)
            {
                var ids = serviceIds
                    .Where(x => Guid.TryParse(x, out _))
                    .Select(Guid.Parse)
                    .ToList();

                data = data.Where(x => x.ServiceId.HasValue && ids.Contains(x.ServiceId.Value)).ToList();
            }

            if (!string.IsNullOrEmpty(stageId) &&
                Guid.TryParse(stageId, out var stageGuid))
            {
                data = data.Where(x => x.OpportunityStageId == stageGuid).ToList();
            }

            if (!string.IsNullOrEmpty(startDate) &&
                DateTime.TryParseExact(startDate, "dd-MMM-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var start))
            {
                data = data.Where(x => x.CreatedOn >= start).ToList();
            }

            if (!string.IsNullOrEmpty(endDate) &&
                DateTime.TryParseExact(endDate, "dd-MMM-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var end))
            {
                data = data.Where(x => x.CreatedOn < end.Date.AddDays(1)).ToList();
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                var search = searchText.ToLower();

                data = data.Where(x =>
                    (x.OpportunityName ?? "").ToLower().Contains(search) ||
                    (x.ServiceName ?? "").ToLower().Contains(search)
                ).ToList();
            }

            // =========================
            // PAGINATION
            // =========================
            var totalCount = data.Count;

            var paged = data
                .OrderByDescending(x => x.CreatedOn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new OpportunityListVM
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Opportunities = paged
            };

            // =========================
            // VIEWBAGS
            // =========================
            ViewBag.Services = await _context.Services
                .Select(s => new SelectListItem
                {
                    Value = s.ServiceId.ToString(),
                    Text = s.ServiceName
                })
                .ToListAsync();

            //if (roleid == scoperRoleId)
            //{
                ViewBag.Stages = await _context.OpportunityStages
                    .Where(x =>
                        x.IsActive == true 
                        //&&
                        //x.RoleId == scoperRoleId.ToString()
                    )
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new
                    {
                        x.StageId,
                        x.StageName,

                        Count = (
                            from s in _context.OpportunityServices
                            join o in _context.Opportunities
                                on s.OpportunityId equals o.OpportunityId

                            where s.OpportunityStageId == x.StageId
                                  && o.TenantId == tenantId
                                  && s.ScoperId == _IUserContext.GUID_USERID

                            select s.OpportunityServiceId
                        ).Distinct().Count()
                    })
                    .ToListAsync();
            //}

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.SearchText = searchText;
            ViewBag.SelectedServices = serviceIds ?? Array.Empty<string>();
            ViewBag.SelectedStage = stageId;

            return View(model);
        }

        public async Task<IActionResult> EditOpportunity(
     Guid id,
     Guid? serviceId,
     string ApprovalStatus)
        {
            // =========================
            // 🔹 GET OPPORTUNITY
            // =========================
            var opportunity = await _context.Opportunities
                .FirstOrDefaultAsync(x => x.OpportunityId == id);

            if (opportunity == null)
                return NotFound();

            // =========================
            // 🔹 GET SINGLE SERVICE
            // =========================
            var service = await _context.OpportunityServices
                .FirstOrDefaultAsync(x =>
                    x.OpportunityId == id &&
                    x.ServiceId.HasValue &&
                    (serviceId == null || x.ServiceId == serviceId));

            var model = new OpportunityVM
            {
                OpportunityId = opportunity.OpportunityId,
                OpportunityName = opportunity.OpportunityName,
                EditServices = new List<OpportunityServiceVM>()
            };

            // =========================
            // 🔹 MAP SERVICE (ONLY ONE)
            // =========================
            if (service != null && service.ServiceId != null)
            {
                var serviceVM = new OpportunityServiceVM
                {
                    ServiceId = service.ServiceId.Value,
                    LeadId = service.LeadId,
                    AccountId = service.AccountId,
                    ScopeofWork = service.ScopeofWork,
                    FinalDeliverableUsage = service.FinalDeliverableUsage,
                    ExpectationSetting = service.ExpectationSetting,
                    CustomerResponsibilities = service.CustomerResponsibilities,
                    ScopeAnalystId = service.ScopeAnalystId,
                    EngagementModelId = service.EngagementModelId,
                    IndustryId = service.ProjectIndustryId,
                    TurnHoursDays = service.TurnHoursDays ?? 0,
                    NoOfMonth = service.NoOfMonth ?? 0,
                    CustomerSupport = service.SupportTime,
                    OpportunityStageId = service.OpportunityStageId,
                    DealTypeId = service.DealTypeId,
                    WinLossReasonId = service.WinLossReasonId,

                    Details = await _context.OpportunityModules
                        .Where(x => x.ServiceId == service.OpportunityServiceId)
                        .Select(x => new OpportunityDetailVM
                        {
                            ModuleName = x.ModuleName,
                            EstimatedHours = x.EstimatedHours,
                            QCHours = x.QCHours
                        }).ToListAsync()
                };

                // Ensure at least one row in UI
                if (serviceVM.Details == null || serviceVM.Details.Count == 0)
                {
                    serviceVM.Details = new List<OpportunityDetailVM>
            {
                new OpportunityDetailVM()
            };
                }

                model.EditServices.Add(serviceVM);

                // =========================
                // 🔹 LOAD LEAD DETAILS
                // =========================
                if (service.LeadId != null)
                {
                    var leadDetails = await GetLeadDetails(service.LeadId.Value);
                    ViewBag.LeadDetails = leadDetails;
                }
            }
            // ✅ STORE IN VIEWBAG
            ViewBag.ApprovalStatus = ApprovalStatus;

            // ✅ ENABLE ONLY WHEN CANCELLED
            ViewBag.EnableStage = ApprovalStatus == "Cancelled";
            // 🔹 LOAD DROPDOWNS
            // =========================
            await LoadDropdowns();

            return View(model);
        }

        private async Task LoadDropdowns()
        {
            ViewBag.ServiceList = await _context.Services
                .Select(x => new SelectListItem
                {
                    Value = x.ServiceId.ToString(),
                    Text = x.ServiceName
                }).ToListAsync();

            ViewBag.LeadList = await _context.Leads
                .Select(x => new SelectListItem
                {
                    Value = x.LeadId.ToString(),
                    Text = x.FirstName + " " + x.LastName
                }).ToListAsync();

            ViewBag.StageList = await _context.OpportunityStages
                .Select(x => new SelectListItem
                {
                    Value = x.StageId.ToString(),
                    Text = x.StageName
                }).ToListAsync();

            ViewBag.EngagementModelList = await _context.EngagementModels
                .Select(x => new SelectListItem
                {
                    Value = x.EngagementModelId.ToString(),
                    Text = x.EngagementModelName
                }).ToListAsync();

            ViewBag.DealTypeList = await _context.DealTypes
                .Select(x => new SelectListItem
                {
                    Value = x.DealTypeId.ToString(),
                    Text = x.DealTypeName
                }).ToListAsync();

            ViewBag.WinLossList = await _context.WinLossReasons
                .Select(x => new SelectListItem
                {
                    Value = x.WinLossReasonId.ToString(),
                    Text = x.ReasonName
                }).ToListAsync();

            ViewBag.Industries = await _context.Industries
                .Select(x => new SelectListItem
                {
                    Value = x.IndustryId.ToString(),
                    Text = x.IndustryName
                }).ToListAsync();


        }

        public async Task<LeadInfo> GetLeadDetails(Guid leadId)
        {
            var data = await (
               from l in _context.Leads.AsNoTracking()

               join c in _context.Countries on l.CountryId equals c.CountryId into cj
               from c in cj.DefaultIfEmpty()

               join g in _context.GenderMasters on l.GenderId equals g.GenderId into gj
               from g in gj.DefaultIfEmpty()

               where l.LeadId == leadId && l.IsActive == true

               select new
               {
                   l,
                   Country = c != null ? c.CountryName : null,
                   Gender = g != null ? g.GenderName : null,
                   OwnerId = l.OwnerId
               }
            ).FirstOrDefaultAsync();

            if (data == null)
                return null;

            // 🔹 Get Sales Executive
            string salesExecutive = null;

            if (data.OwnerId != null)
            {
                var user = await _identityDbContext.Users
                    .Where(u => u.Id == data.OwnerId.ToString())
                    .Select(u => u.UserName)
                    .FirstOrDefaultAsync();

                salesExecutive = user;
            }

            return new LeadInfo
            {
                LeadId = data.l.LeadId,
                FirstName = data.l.FirstName,
                LastName = data.l.LastName,
                Email = data.l.Email,
                Phone = data.l.Phone,
                City = data.l.City,
                Country = data.Country,
                CompanyName = data.l.CompanyName,
                Industry = data.l.IndustryId.ToString(),
                SalesExecutive = salesExecutive
            };
        }
        public async Task<IActionResult> OpportunityDetails(Guid id, Guid? serviceId)
        {
            // =========================
            // 🔹 OPPORTUNITY + LEAD
            // =========================
            var opportunity = await (
                from o in _context.Opportunities

                join os in _context.OpportunityServices
                    on o.OpportunityId equals os.OpportunityId into osGroup
                from os in osGroup.DefaultIfEmpty()

                join l in _context.Leads
                    on os.LeadId equals l.LeadId into leadGroup
                from l in leadGroup.DefaultIfEmpty()

                where o.OpportunityId == id

                select new OpportunityVM
                {
                    OpportunityId = o.OpportunityId,
                    OpportunityName = o.OpportunityName,
                    LeadId=os.LeadId,
                    LeadName = l != null ? l.FirstName + " " + l.LastName : ""
                }
            ).FirstOrDefaultAsync();

            if (opportunity == null)
                return NotFound();

            // =========================
            // 🔹 SINGLE SERVICE
            // =========================
            var service = await (
      from s in _context.OpportunityServices

      join em in _context.EngagementModels
          on s.EngagementModelId equals em.EngagementModelId into emGroup
      from em in emGroup.DefaultIfEmpty()
      join sm in _context.Services
         on s.ServiceId equals sm.ServiceId into smGroup
      from sm in smGroup.DefaultIfEmpty()
      join d in _context.Industries
         on s.ProjectIndustryId equals d.IndustryId into dGroup
      from d in dGroup.DefaultIfEmpty()
      where s.OpportunityId == id &&
            s.ServiceId == serviceId

      select new EditServiceVM
      {
          ServiceId = s.ServiceId ?? Guid.Empty,
          ServiceName = sm.ServiceName,

          EngagementModelName = em != null ? em.EngagementModelName : "",   // ✅ FIX

          ScopeofWork = s.ScopeofWork,
          FinalDeliverableUsage = s.FinalDeliverableUsage,
          ExpectationSetting = s.ExpectationSetting,
          CustomerResponsibilities = s.CustomerResponsibilities,
        IndustryName=d.IndustryName,
          OpportunityServiceId = s.OpportunityServiceId,
          OpportunityStageId = s.OpportunityStageId
      }
  ).FirstOrDefaultAsync();

            if (service == null)
                return View(opportunity);

            // =========================
            // 🔹 STAGE NAME
            // =========================
            service.StageName = await _context.OpportunityStages
                .Where(x => x.StageId == service.OpportunityStageId)
                .Select(x => x.StageName)
                .FirstOrDefaultAsync();

            // =========================
            // 🔹 MODULES
            // =========================
            service.Details = await _context.OpportunityModules
                .Where(m => m.ServiceId == service.OpportunityServiceId)
                .Select(m => new DetailVM
                {
                    ModuleName = m.ModuleName,
                    EstimatedHours = m.EstimatedHours,
                    QCHours = m.QCHours
                })
                .ToListAsync();
            if (opportunity.LeadId != null)
            {
                var leadDetails = await GetLeadDetails(opportunity.LeadId??Guid.NewGuid());
                ViewBag.LeadDetails = leadDetails;

            }
            // =========================
            // 🔹 ASSIGN SINGLE SERVICE
            // =========================
            opportunity.SingleService = service;

            return View(opportunity);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(OpportunityVM model)
        {

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            var currentRoleIds = _IUserContext.RoleIds;
            try
            {
                var opportunity = await _context.Opportunities
                    .FirstOrDefaultAsync(x => x.OpportunityId == model.OpportunityId);

                if (opportunity == null)
                    return NotFound();

                // 🔹 Update Opportunity
                //opportunity.OpportunityName = model.OpportunityName;
                opportunity.UpdatedOn = DateTime.Now;

                var svc = model.EditServices.FirstOrDefault();
                if (svc == null)
                    return BadRequest("Service data missing");

                // 🔹 Find or create service
                var service = await _context.OpportunityServices
                    .FirstOrDefaultAsync(x =>
                        x.OpportunityId == model.OpportunityId &&
                        x.ServiceId == svc.ServiceId);

                if (service == null)
                {
                    service = new OpportunityService
                    {
                        OpportunityServiceId = Guid.NewGuid(),
                        OpportunityId = model.OpportunityId,
                        ServiceId = svc.ServiceId,
                        CreatedOn = DateTime.Now,
                        TenantId = opportunity.TenantId ?? Guid.NewGuid()
                    };

                    _context.OpportunityServices.Add(service);
                }

                // 🔹 Update service fields
                service.LeadId = svc.LeadId;
                service.AccountId = svc.AccountId;
                service.ScopeofWork = svc.ScopeofWork;
                service.FinalDeliverableUsage = svc.FinalDeliverableUsage;
                service.ExpectationSetting = svc.ExpectationSetting;
                service.CustomerResponsibilities = svc.CustomerResponsibilities;
                service.ProjectIndustryId = svc.IndustryId;
                service.TurnHoursDays = svc.TurnHoursDays;
                service.NoOfMonth = svc.NoOfMonth;

                service.SupportTime = svc.CustomerSupport;
                service.ScopeAnalystId = svc.ScopeAnalystId;
                service.EngagementModelId = svc.EngagementModelId;
                service.OpportunityStageId = svc.OpportunityStageId;
                service.DealTypeId = svc.DealTypeId;
                service.WinLossReasonId = svc.WinLossReasonId;
                service.ScoperId = _IUserContext.GUID_USERID;
                service.ScoperUpdatedOn = DateTime.Now;

                // 🔹 Save first so we get service ID
                await _context.SaveChangesAsync();

                var serviceId = service.OpportunityServiceId;

                // =========================
                // 🔥 MODULES (REPLACE ALL)
                // =========================
                var oldModules = await _context.OpportunityModules
                    .Where(x => x.ServiceId == serviceId)
                    .ToListAsync();

                _context.OpportunityModules.RemoveRange(oldModules);

                if (svc.Details != null && svc.Details.Any())
                {
                    foreach (var item in svc.Details)
                    {
                        _context.OpportunityModules.Add(new OpportunityModule
                        {
                            OpportunityModuleId = Guid.NewGuid(),
                            ServiceId = serviceId,
                            ModuleName = item.ModuleName,
                            EstimatedHours = item.EstimatedHours,
                            QCHours = item.QCHours,
                            CreatedOn = DateTime.Now,
                            TenantId = opportunity.TenantId
                        });
                    }
                }

                // =========================
                // 🔥 WORKFLOW
                // =========================
                if (string.IsNullOrEmpty(service.ScoperStatus) || service.ScoperStatus == "Draft"  || service.ScoperStatus == "Rejected")
                {
                    var workflow = await _context.ApprovalWorkflows
                        .FirstOrDefaultAsync(x => x.ModuleName == "Opportunity" && x.IsActive==true);

                    if (workflow != null)
                    {
                        var firstStep = await _context.WorkflowSteps
                            .Where(x => x.WorkflowId == workflow.WorkflowId)
                            .OrderBy(x => x.LevelOrder)
                            .FirstOrDefaultAsync();

                        if (firstStep != null)
                        {
                            var alreadyPending = await _context.ApprovalTransactions
                                .AnyAsync(x =>
                                    x.RecordId == service.OpportunityServiceId &&
                                    x.ApprovalStatus == "Pending");

                            if (!alreadyPending)
                            {
                                _context.ApprovalTransactions.Add(new ApprovalTransaction
                                {
                                    ApprovalId = Guid.NewGuid(),
                                    WorkflowId = workflow.WorkflowId,
                                    RecordId = service.OpportunityServiceId,
                                    StepId = firstStep.StepId,
                                    ApprovalStatus = "Pending",
                                    ActionDate = DateTime.Now
                                });

                                service.ScoperStatus = "Submitted";
                                service.CurrentStepId = firstStep.StepId;
                                service.WorkflowId = workflow.WorkflowId;

                                var scopedStage = await _context.OpportunityStages
                                    .FirstOrDefaultAsync(x =>
                                        x.StageName == "Scoped" &&
                                        currentRoleIds.Select(r => r.ToString())
                                            .Contains(x.RoleId));

                                if (scopedStage != null)
                                {
                                    service.OpportunityStageId = scopedStage.StageId;
                                }
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return RedirectToAction("OpportunityScoperView");
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();

                ModelState.AddModelError("", ex.Message);
                await LoadDropdowns();

                return View(model);
            }
        }


        public async Task<IActionResult> ApprovalOpportunityList(
      int pageNumber = 1,
      int pageSize = 5,
      string startDate = null,
      string endDate = null,
      string searchText = null,
      string[] serviceIds = null,
      string stageId = null
  )
        {
            var tenantId = Guid.Parse("6200E0AE-F4C7-4509-B618-DC3490EE88D1");

            Guid scoperRoleId = Guid.Parse("ce8bb0b8-842b-45b6-85f0-5db039ac2f51");
            Guid salesRoleId = Guid.Parse("7bd843ef-9865-47ac-b6c3-9de8447a38d3");

            var currentRoleIds = _IUserContext.RoleIds;
            
            var roles = await _identityDbContext.Roles
    .Select(r => new
    {
        r.Id,
        r.Name
    })
    .ToListAsync();
            // =========================
            // USERS
            // =========================
            var users = await _identityDbContext.Users
                .Select(u => new
                {
                    u.Id,
                    u.FullName
                })
                .ToListAsync();

            // =========================
            // ROLE MAPS
            // =========================
            var roleMaps = await _context.ServiceUserRoleMaps
                .Where(x => x.RoleId == scoperRoleId || x.RoleId == salesRoleId)
                .ToListAsync();

            var scoperMapDict = roleMaps
                .Where(x => x.RoleId == scoperRoleId)
                .GroupBy(x => x.ServiceId)
                .ToDictionary(x => x.Key, x => x.FirstOrDefault());

            var salesMapDict = roleMaps
                .Where(x => x.RoleId == salesRoleId)
                .GroupBy(x => x.ServiceId)
                .ToDictionary(x => x.Key, x => x.FirstOrDefault());

            
            var query =
                from at in _context.ApprovalTransactions

                join ws in _context.WorkflowSteps
                    on at.StepId equals ws.StepId

                join os in _context.OpportunityServices
                    on at.RecordId equals os.OpportunityServiceId

                join o in _context.Opportunities
                    on os.OpportunityId equals o.OpportunityId

                join s in _context.Services
                    on os.ServiceId equals s.ServiceId into sj
                from s in sj.DefaultIfEmpty()

                join st in _context.OpportunityStages
                    on os.OpportunityStageId equals st.StageId into stj
                from st in stj.DefaultIfEmpty()

                join l in _context.Leads
                    on os.LeadId equals l.LeadId into lj
                from l in lj.DefaultIfEmpty()
          

                join ps in _context.Proposals
on os.ProposalId equals ps.ProposalId into psGroup
                from ps in psGroup.DefaultIfEmpty()


                join wo in _context.WorkOrders
on ps.ProposalId equals wo.ProposalId into woGroup
                from wo in woGroup.DefaultIfEmpty()
                where
    o.TenantId == tenantId
    && currentRoleIds.Select(x => x.ToString()).Contains(ws.RoleId)
    && (
        at.ApprovalStatus == "Pending"
        || at.ApprovalStatus == "Approved" || at.ApprovalStatus == "Cancelled"
    )
                select new
                {
                    at,

                    at.ApprovalId,

                    o.OpportunityId,

                    os.OpportunityServiceId,

                    OpportunityName = o.OpportunityName ?? "",

                    LeadName =
         (l != null ? l.FirstName : "") + " " +
         (l != null ? l.LastName : ""),

                    ServiceName = s != null
         ? s.ServiceName
         : "",

                    StageName = st != null
         ? st.StageName
         : "",

                    StageId = st != null
         ? st.StageId
         : Guid.Empty,

                    ServiceId = s != null
         ? s.ServiceId
         : Guid.Empty,

                    ws.RoleId,

                    ApprovalStatus = at.ApprovalStatus ?? "",

                    ScoperStatus = os.ScoperStatus ?? "",

                    SubmittedOn = at.ActionDate,

                    ApprovedBy = at.ApprovedBy,

                    ActionDate = at.ActionDate,
                    WorkOrderId = wo != null ? wo.WorkOrderId : Guid.Empty,
                    WorkOrderNumber = wo != null ? wo.WorkOrderNumber : "",
                    WorkOrderStatus = wo != null ? wo.WorkOrderStatus : "",
                    HasWorkOrder = wo != null,
                    ProposalId= ps != null ? ps.ProposalId : Guid.Empty,

                    EstimatedOn =
         _context.OpportunityModules
             .Where(x => x.ServiceId == os.OpportunityServiceId)
             .OrderByDescending(x => x.CreatedOn)
             .Select(x => x.CreatedOn)
             .FirstOrDefault()
                };
            // =========================
            // EXECUTE QUERY
            // =========================
            // =========================
            // EXECUTE QUERY
            // =========================
            var raw = await query.ToListAsync();

            // =========================
            // MAP DATA
            // =========================
            var data = raw.Select(x =>
            {
                Guid serviceId = x.ServiceId;

                scoperMapDict.TryGetValue(serviceId, out var scoperMap);

                salesMapDict.TryGetValue(serviceId, out var salesMap);

                var scoperUser = users.FirstOrDefault(u =>
                    scoperMap != null &&
                    u.Id == scoperMap.UserId.ToString());

                var salesUser = users.FirstOrDefault(u =>
                    salesMap != null &&
                    u.Id == salesMap.UserId.ToString());

                // =========================
                // ROLE NAME
                // =========================
                string approvedRoleName = "-";

                var workflowStep = _context.WorkflowSteps
                    .FirstOrDefault(w =>
                        w.StepId == x.at.StepId);

                if (workflowStep != null)
                {
                    approvedRoleName = roles
                        .FirstOrDefault(r =>
                            r.Id.ToString() == workflowStep.RoleId)
                        ?.Name ?? "-";
                }

                // =========================
                // APPROVED USER
                // =========================
                string approvedByName = "-";

                if (x.ApprovedBy != null)
                {
                    approvedByName = users
                        .FirstOrDefault(u =>
                            u.Id == x.ApprovedBy.ToString())
                        ?.FullName ?? "-";
                }

                return new ApprovalOpportunityVM
                {
                    ApprovalId = x.ApprovalId,

                    OpportunityId = x.OpportunityId,

                    OpportunityServiceId = x.OpportunityServiceId,

                    OpportunityName = x.OpportunityName,

                    LeadName = x.LeadName,

                    ServiceName = x.ServiceName,

                    StageName = x.StageName,

                    StageId = x.StageId,

                    ServiceId = x.ServiceId,

                    // =========================
                    // APPROVAL INFO
                    // =========================
                    RoleId = x.RoleId,

                    ApprovalStatus = x.ApprovalStatus,

                    ApprovedRole = approvedRoleName,

                    ApprovedByName = approvedByName,

                    LastApprovedActionDate = x.ActionDate,

                    // =========================
                    // OTHER INFO
                    // =========================
                    ScoperStatus = x.ScoperStatus,

                    SubmittedOn = x.SubmittedOn,

                    EstimatedOn = x.EstimatedOn,

                    Scoper = scoperUser?.FullName ?? "-",

                    SlaesExecutive = salesUser?.FullName ?? "-",
                    // ✅ WORK ORDER
                    WorkOrderId = x.WorkOrderId == Guid.Empty ? null : x.WorkOrderId,
                    WorkOrderNumber = x.WorkOrderNumber,
                    WorkOrderStatus = x.WorkOrderStatus,
                    HasWorkOrder = x.WorkOrderId != Guid.Empty,
                    ProposalId=x.ProposalId
                };
            }).ToList();
            ViewBag.HasWorkOrder =
    raw.Any(x => x.WorkOrderId != Guid.Empty);
            // =========================
            // FILTERS
            // =========================
            if (serviceIds?.Length > 0)
            {
                var ids = serviceIds
                    .Where(x => Guid.TryParse(x, out _))
                    .Select(Guid.Parse)
                    .ToList();

                data = data
                    .Where(x => x.ServiceId.HasValue && ids.Contains(x.ServiceId.Value))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(stageId) &&
                Guid.TryParse(stageId, out var stageGuid))
            {
                data = data.Where(x => x.StageId == stageGuid).ToList();
            }

            if (!string.IsNullOrEmpty(startDate) &&
                DateTime.TryParseExact(startDate, "dd-MMM-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var start))
            {
                data = data.Where(x => x.SubmittedOn >= start).ToList();
            }

            if (!string.IsNullOrEmpty(endDate) &&
                DateTime.TryParseExact(endDate, "dd-MMM-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var end))
            {
                data = data.Where(x => x.SubmittedOn < end.Date.AddDays(1)).ToList();
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                var search = searchText.ToLower();

                data = data.Where(x =>
                    (x.OpportunityName ?? "").ToLower().Contains(search) ||
                    (x.ServiceName ?? "").ToLower().Contains(search) ||
                    (x.LeadName ?? "").ToLower().Contains(search)
                ).ToList();
            }

            // =========================
            // PAGINATION
            // =========================
            var totalCount = data.Count;

            var paged = data
                .OrderByDescending(x => x.SubmittedOn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new ApprovalOpportunityListVM
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Opportunities = paged
            };

            // =========================
            // VIEWBAGS
            // =========================
            ViewBag.Services = await _context.Services
                .Select(s => new SelectListItem
                {
                    Value = s.ServiceId.ToString(),
                    Text = s.ServiceName
                })
                .ToListAsync();

            ViewBag.Stages = await _context.OpportunityStages
                .Where(x => x.IsActive == true)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new
                {
                    x.StageId,
                    x.StageName,
                    Count = _context.OpportunityServices
                        .Count(s => s.OpportunityStageId == x.StageId)
                })
                .ToListAsync();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.SearchText = searchText;
            ViewBag.SelectedServices = serviceIds ?? Array.Empty<string>();
            ViewBag.SelectedStage = stageId;

            return View(model);
        }


        public async Task<IActionResult> ApprovalOpportunityDetails(
    Guid id,
    Guid? serviceId,
    Guid approvalId, Guid? workOrderId,
    Guid? ProposalId// ✅ ADD THIS
)
        {


            var currentRoleIds = _IUserContext.RoleIds;
            Guid salesManagerRoleId = Guid.Parse("ce8bb0b8-842b-45b6-85f0-5db039ac2f52");
            // 🔹 OPPORTUNITY + LEAD
            // =========================
            var opportunity = await (
                from o in _context.Opportunities

                join os in _context.OpportunityServices
                    on o.OpportunityId equals os.OpportunityId into osGroup
                from os in osGroup.DefaultIfEmpty()

                join l in _context.Leads
                    on os.LeadId equals l.LeadId into leadGroup
                from l in leadGroup.DefaultIfEmpty()

                where o.OpportunityId == id

                select new OpportunityVM
                {
                    OpportunityId = o.OpportunityId,
                    OpportunityName = o.OpportunityName,
                    LeadId = os.LeadId,
                    LeadName = l != null
                        ? l.FirstName + " " + l.LastName
                        : ""
                }

            ).FirstOrDefaultAsync();

            if (opportunity == null)
                return NotFound();

            // =========================
            // 🔹 SERVICE
            // =========================
            var service = await (
                from s in _context.OpportunityServices

                join em in _context.EngagementModels
                    on s.EngagementModelId equals em.EngagementModelId into emGroup
                from em in emGroup.DefaultIfEmpty()

                join sm in _context.Services
                    on s.ServiceId equals sm.ServiceId into smGroup
                from sm in smGroup.DefaultIfEmpty()

                join d in _context.Industries
                    on s.ProjectIndustryId equals d.IndustryId into dGroup
                from d in dGroup.DefaultIfEmpty()

                join op in _context.OpportunityStages
                    on s.OpportunityStageId equals op.StageId into opGroup
                from op in opGroup.DefaultIfEmpty()

                where s.OpportunityId == id
                      && s.OpportunityServiceId == serviceId

                select new EditServiceVM
                {
                    ServiceId = s.ServiceId ?? Guid.Empty,
                    ServiceName = sm.ServiceName,

                    EngagementModelName =
                        em != null ? em.EngagementModelName : "",

                    IndustryName =
                        d != null ? d.IndustryName : "",

                    ScopeofWork = s.ScopeofWork,
                    FinalDeliverableUsage = s.FinalDeliverableUsage,
                    ExpectationSetting = s.ExpectationSetting,
                    CustomerResponsibilities = s.CustomerResponsibilities,
                    StageName=op.StageName,
                    OpportunityServiceId = s.OpportunityServiceId,
                    OpportunityStageId = s.OpportunityStageId
                }

            ).FirstOrDefaultAsync();

            if (service == null)
                return View(opportunity);

            // =========================
            // 🔹 STAGE
            // =========================
            service.StageName = await _context.OpportunityStages
                .Where(x => x.StageId == service.OpportunityStageId)
                .Select(x => x.StageName)
                .FirstOrDefaultAsync();

            // =========================
            // 🔹 MODULES
            // =========================
            service.Details = await _context.OpportunityModules
                .Where(m => m.ServiceId == service.OpportunityServiceId)
                .Select(m => new DetailVM
                {
                    ModuleName = m.ModuleName,
                    EstimatedHours = m.EstimatedHours,
                    QCHours = m.QCHours
                })
                .ToListAsync();

            // =========================
            // 🔹 LEAD DETAILS
            // =========================
            if (opportunity.LeadId != null)
            {
                var leadDetails = await GetLeadDetails(
                    opportunity.LeadId ?? Guid.NewGuid());

                ViewBag.LeadDetails = leadDetails;
            }


            var isSalesManager = currentRoleIds != null &&
                                 currentRoleIds.Contains(salesManagerRoleId);

            if (isSalesManager)
            {
                ViewBag.StageList = await _context.OpportunityStages
    .Where(x => x.IsActive == true &&
                x.RoleId == salesManagerRoleId.ToString())
    .OrderBy(x => x.DisplayOrder)
    .Select(x => new SelectListItem
    {
        Value = x.StageId.ToString(),
        Text = x.StageName
    })
    .ToListAsync();
            }
            else
            {
                ViewBag.StageList = new List<SelectListItem>();
            }

            ViewBag.ApprovalId = approvalId;
            ViewBag.IsSalesManager = isSalesManager;
            WorkOrderVM workOrderVm = null;

            if (workOrderId.HasValue && workOrderId != Guid.Empty)
            {
                workOrderVm = await _context.WorkOrders
                    .Where(x => x.WorkOrderId == workOrderId)
                    .Select(x => new WorkOrderVM
                    {
                        WorkOrderId = x.WorkOrderId,
                        WorkOrderNumber = x.WorkOrderNumber,
                        Description = x.Description,
                        WorkOrderStatus = x.WorkOrderStatus,
                        EstimatedHours = x.EstimatedHours,
                        ActualHours = x.ActualHours,
                        CreatedOn = x.CreatedOn
                    })
                    .FirstOrDefaultAsync();
            }
            ViewBag.HasWorkOrder = await _context.WorkOrders
    .AnyAsync(x => x.WorkOrderId == workOrderId);
            ViewBag.WorkOrder = workOrderVm;
            opportunity.SingleService = service;
            var approvalStatus = await _context.ApprovalTransactions
    .Where(x => x.ApprovalId == approvalId
             && x.RecordId == serviceId)
    .Select(x => x.ApprovalStatus)
    .FirstOrDefaultAsync();

            ViewBag.ApprovalStatus = approvalStatus;
            return View(opportunity);
        }
        public async Task<IActionResult> ApprovalOpportunityProposalDetails(
            Guid id,
            Guid? serviceId,
            Guid approvalId,
            Guid? workOrderId)
        {
            var currentRoleIds = _IUserContext.RoleIds;

            Guid salesManagerRoleId =
                Guid.Parse("ce8bb0b8-842b-45b6-85f0-5db039ac2f52");

            // =========================
            // 🔹 OPPORTUNITY
            // =========================
            var opportunity = await (
                from o in _context.Opportunities

                join os in _context.OpportunityServices
                    on o.OpportunityId equals os.OpportunityId into osGroup
                from os in osGroup.DefaultIfEmpty()

                join l in _context.Leads
                    on os.LeadId equals l.LeadId into leadGroup
                from l in leadGroup.DefaultIfEmpty()

                where o.OpportunityId == id

                select new OpportunityVM
                {
                    OpportunityId = o.OpportunityId,
                    OpportunityName = o.OpportunityName,
                    LeadId = os.LeadId,
                    LeadName = l != null
                        ? l.FirstName + " " + l.LastName
                        : ""
                }

            ).FirstOrDefaultAsync();

            if (opportunity == null)
                return NotFound();

            // =========================
            // 🔹 SERVICE
            // =========================
            var service = await (
                from s in _context.OpportunityServices

                join em in _context.EngagementModels
                    on s.EngagementModelId equals em.EngagementModelId into emGroup
                from em in emGroup.DefaultIfEmpty()

                join sm in _context.Services
                    on s.ServiceId equals sm.ServiceId into smGroup
                from sm in smGroup.DefaultIfEmpty()

                join lm in _context.Leads
                    on s.LeadId equals lm.LeadId into lmGroup
                from lm in lmGroup.DefaultIfEmpty()
                join d in _context.Industries
                    on s.ProjectIndustryId equals d.IndustryId into dGroup
                from d in dGroup.DefaultIfEmpty()

                join op in _context.OpportunityStages
                    on s.OpportunityStageId equals op.StageId into opGroup
                from op in opGroup.DefaultIfEmpty()
                join cm in _context.Countries
                  on lm.CountryId equals cm.CountryId into cmGroup
                from cm in cmGroup.DefaultIfEmpty()

                where s.OpportunityId == id
                      && s.OpportunityServiceId == serviceId

                select new EditServiceVM
                {
                    ServiceId = s.ServiceId ?? Guid.Empty,
                    ServiceName = sm.ServiceName,

                    EngagementModelName =
                        em != null ? em.EngagementModelName : "",

                    IndustryName =
                        d != null ? d.IndustryName : "",

                    ScopeofWork = s.ScopeofWork,
                    FinalDeliverableUsage = s.FinalDeliverableUsage,
                    ExpectationSetting = s.ExpectationSetting,
                    CustomerResponsibilities = s.CustomerResponsibilities,
                    CurrencyCode = cm.CurrencyCode,
                    StageName = op.StageName,

                    OpportunityServiceId = s.OpportunityServiceId,
                    OpportunityStageId = s.OpportunityStageId
                }

            ).FirstOrDefaultAsync();

            if (service == null)
                return View(opportunity);

            // =========================
            // 🔹 MODULES
            // =========================
            service.Details = await _context.OpportunityModules
                .Where(m => m.ServiceId == service.OpportunityServiceId)
                .Select(m => new DetailVM
                {
                    ModuleName = m.ModuleName,
                    EstimatedHours = m.EstimatedHours,
                    QCHours = m.QCHours
                })
                .ToListAsync();

            // =========================
            // 🔹 LEAD DETAILS
            // =========================
            if (opportunity.LeadId != null)
            {
                var leadDetails = await GetLeadDetails(
                    opportunity.LeadId ?? Guid.NewGuid());

                ViewBag.LeadDetails = leadDetails;
            }

            // =========================
            // 🔹 STAGE DROPDOWN
            // =========================
            var isSalesManager = currentRoleIds != null &&
                                 currentRoleIds.Contains(salesManagerRoleId);

            if (isSalesManager)
            {
                ViewBag.StageList = await _context.OpportunityStages
                    .Where(x => x.IsActive == true &&
                                x.RoleId == salesManagerRoleId.ToString())
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new SelectListItem
                    {
                        Value = x.StageId.ToString(),
                        Text = x.StageName
                    })
                    .ToListAsync();
            }
            else
            {
                ViewBag.StageList = new List<SelectListItem>();
            }

            ViewBag.ApprovalId = approvalId;
            ViewBag.IsSalesManager = isSalesManager;

            // =========================
            // 🔹 WORK ORDER
            // =========================
            WorkOrderVM workOrderVm = null;

            if (workOrderId.HasValue && workOrderId != Guid.Empty)
            {
                workOrderVm = await _context.WorkOrders
                    .Where(x => x.WorkOrderId == workOrderId)
                    .Select(x => new WorkOrderVM
                    {
                        WorkOrderId = x.WorkOrderId,
                        WorkOrderNumber = x.WorkOrderNumber,
                        Description = x.Description,
                        WorkOrderStatus = x.WorkOrderStatus,
                        EstimatedHours = x.EstimatedHours,
                        ActualHours = x.ActualHours,
                        WorkOrderURL =x.WorkOrderURL,
                    })
                    .FirstOrDefaultAsync();
            }

            ViewBag.HasWorkOrder = workOrderVm != null;
            ViewBag.WorkOrder = workOrderVm;

            // =========================
            // 🔹 APPROVAL STATUS
            // =========================
            var approvalStatus = await _context.ApprovalTransactions
                .Where(x => x.ApprovalId == approvalId
                         && x.RecordId == serviceId)
                .Select(x => x.ApprovalStatus)
                .FirstOrDefaultAsync();

            ViewBag.ApprovalStatus = approvalStatus;

            opportunity.SingleService = service;
            // ==========================================
            // PROPOSAL PRICING
            // ==========================================

            // TOTAL ESTIMATED HOURS
            decimal totalHours = service.Details?
                .Sum(x => x.EstimatedHours ?? 0) ?? 0;

            ViewBag.TotalHours = totalHours;

            // GET LEAD COUNTRY
            var leadCountryId = await _context.Leads
                .Where(x => x.LeadId == opportunity.LeadId)
                .Select(x => x.CountryId)
                .FirstOrDefaultAsync();

            // GET REGION FROM COUNTRY
            var regionId = await _context.Countries
                .Where(x => x.CountryId == leadCountryId)
                .Select(x => x.RegionId)
                .FirstOrDefaultAsync();

            // GET SERVICE PRICING
            var pricing = await _context.ServicePricings
                .Where(x =>
                    x.ServiceId == service.ServiceId &&
                    x.RegionId == regionId &&
                    x.IsActive == true)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();
          
            if (pricing != null)
            {
                ViewBag.LowPrice = pricing.LowPrice;
                ViewBag.HighPrice = pricing.HighPrice;

                // AVERAGE RATE
                decimal averageRate =
                    (pricing.LowPrice + pricing.HighPrice) / 2;

                ViewBag.AverageRate = averageRate;

                // DEFAULT SELLING PRICE
                ViewBag.SellingPrice =
                    totalHours * averageRate;
            }
            else
            {
                ViewBag.LowPrice = 0;
                ViewBag.HighPrice = 0;
                ViewBag.AverageRate = 0;
                ViewBag.SellingPrice = 0;
            }           
           


            ViewBag.CurrencyCode = service.CurrencyCode ?? "USD";
            return View(opportunity);
        }

        // ============================================
        // UPDATE PROPOSAL
        // ============================================

      [HttpPost]
public async Task<IActionResult> UpdateProposal(
    [FromBody] UpdateProposalVM model)
{
    try
    {
        if (model == null)
        {
            return Json(new
            {
                success = false,
                message = "Invalid request"
            });
        }

        // =========================
        // GET EXISTING PROPOSAL
        // =========================

        var proposal = await _context.Proposals
            .FirstOrDefaultAsync(x =>
                x.OpportunityId == model.OpportunityId &&
                x.ServiceId == model.ServiceId);

        // =========================
        // GET SERVICE
        // =========================

        var service = await _context.OpportunityServices
            .FirstOrDefaultAsync(x =>
                x.OpportunityServiceId == model.ServiceId);

        if (service == null)
        {
            return Json(new
            {
                success = false,
                message = "Service not found"
            });
        }

        // =========================
        // GET LEAD
        // =========================

        var lead = await _context.Leads
            .FirstOrDefaultAsync(x =>
                x.LeadId == service.LeadId);

        Guid? countryId = lead?.CountryId;

        // =========================
        // GET COUNTRY
        // =========================

        var country = await _context.Countries
            .FirstOrDefaultAsync(x =>
                x.CountryId == countryId);

        string currencyCode =
            country?.CurrencyCode ?? "USD";

        // =========================
        // GET CURRENCY
        // =========================

        var currency = await _context.Currencies
            .FirstOrDefaultAsync(x =>
                x.CurrencyCode == currencyCode);

        Guid? currencyId = currency?.CurrencyId;

        // =========================
        // CREATE NEW
        // =========================

        if (proposal == null)
        {
            proposal = new Proposal
            {
                ProposalId = Guid.NewGuid(),

                OpportunityId = model.OpportunityId,

                ServiceId = model.ServiceId,

                ProposalNumber =
                    "PROP_" +
                    DateTime.Now.ToString("yyyyMMddHHmmss"),

                ProposalTitle = "Updated Proposal",

                EstimatedHours = model.EstimatedHours,

                ProposedAmount = model.ProposedAmount,

                CurrencyId = currencyId,

                ProposalStatus = "Draft",

                VersionNo = 1,

                CreatedOn = DateTime.Now,

                TenantId = _IUserContext.TenantId
            };

            proposal.ProposalURL =
                $"{Request.Scheme}://{Request.Host}/SalesCRM/Opportunity/ProposalTemplate/{proposal.ProposalId}";

            _context.Proposals.Add(proposal);
        }
        else
        {
            // =========================
            // UPDATE EXISTING
            // =========================

            proposal.EstimatedHours =
                model.EstimatedHours;

            proposal.ProposedAmount =
                model.ProposedAmount;

            proposal.CurrencyId =
                currencyId;

            proposal.VersionNo =
                (proposal.VersionNo ?? 1) + 1;

            if (string.IsNullOrEmpty(proposal.ProposalURL))
            {
                proposal.ProposalURL =
                    $"{Request.Scheme}://{Request.Host}/SalesCRM/Opportunity/ProposalTemplate/{proposal.ProposalId}";
            }

            _context.Proposals.Update(proposal);
        }

        // =========================
        // SAVE
        // =========================

        await _context.SaveChangesAsync();

        // =========================
        // RESPONSE
        // =========================

        return Json(new
        {
            success = true,
            proposalId = proposal.ProposalId,
            proposalUrl = proposal.ProposalURL,
            currencyCode = currencyCode
        });
    }
    catch (Exception ex)
    {
        return Json(new
        {
            success = false,
            message = ex.Message
        });
    }
}
        [HttpPost]
        public async Task<IActionResult> ApproveOpportunity(
           Guid approvalId,
           Guid opportunityServiceId,
           Guid opportunityStageId,
           string comments)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // =========================
                // ROLE SETUP
                // =========================
                var currentRoleIds = _IUserContext.RoleIds;

                Guid deliveryManagerRoleId =
                    Guid.Parse("2d3cf690-820e-468f-961f-93db2e2f62af");

                bool isDeliveryManager =
                    currentRoleIds.Contains(deliveryManagerRoleId);

                // =========================
                // DM STAGE
                // =========================
                var dmApprovedStageId = await _context.OpportunityStages
                    .Where(x => x.StageName == "DM Approved")
                    .Select(x => x.StageId)
                    .FirstOrDefaultAsync();

                // =========================
                // CURRENT APPROVAL
                // =========================
                var approval = await _context.ApprovalTransactions
                    .FirstOrDefaultAsync(x =>
                        x.RecordId == opportunityServiceId &&
                        x.ApprovalStatus == "Pending");

                if (approval == null)
                    return NotFound("Pending approval not found");

                // APPROVE CURRENT
                approval.ApprovalStatus = "Approved";
                approval.Remarks = comments;
                approval.ApprovedBy = Guid.Parse(_IUserContext.UserId);
                approval.ActionDate = DateTime.Now;

                await _context.SaveChangesAsync();

                // =========================
                // SERVICE UPDATE
                // =========================
                var service = await _context.OpportunityServices
                    .FirstOrDefaultAsync(x =>
                        x.OpportunityServiceId == opportunityServiceId);

                if (service == null)
                    return NotFound();

                // ROLE BASED STAGE UPDATE
                service.OpportunityStageId = isDeliveryManager
                    ? dmApprovedStageId
                    : opportunityStageId;

                // =========================
                // WORKFLOW STEP
                // =========================
                var currentStep = await _context.WorkflowSteps
                    .FirstOrDefaultAsync(x => x.StepId == approval.StepId);

                if (currentStep == null)
                {
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return RedirectToAction("ApprovalOpportunityList");
                }

                var nextStep = await _context.WorkflowSteps
                    .Where(x =>
                        x.WorkflowId == currentStep.WorkflowId &&
                        x.LevelOrder > currentStep.LevelOrder)
                    .OrderBy(x => x.LevelOrder)
                    .FirstOrDefaultAsync();

              
                // =========================
                // =========================
                // FINAL STEP
                // =========================
                if (nextStep == null || currentStep.IsFinalLevel == true)
                {
                    service.ScoperStatus = "Approved";

                    service.CurrentStepId = null;

                    // =========================
                    // VALIDATION
                    // =========================

                    if (service.OpportunityId == null)
                    {
                        await transaction.RollbackAsync();

                        return BadRequest("OpportunityId is null");
                    }

                    // =========================
                    // CHECK EXISTING PROPOSAL
                    // =========================

                    var existingProposal = await _context.Proposals
                        .FirstOrDefaultAsync(x =>
                            x.ServiceId == service.OpportunityServiceId);

                    // =========================
                    // CREATE PROPOSAL ONLY IF NOT EXISTS
                    // =========================

                    if (existingProposal == null)
                    {
                        // =========================
                        // MODULE HOURS
                        // =========================

                        var moduleDetails = await _context.OpportunityModules
                            .Where(x => x.ServiceId == service.OpportunityServiceId)
                            .Select(x => new OpportunityDetailVM
                            {
                                ModuleName = x.ModuleName,

                                EstimatedHours = x.EstimatedHours,

                                QCHours = x.QCHours
                            })
                            .ToListAsync();

                        decimal totalEstimatedHours = moduleDetails
                            .Sum(x =>
                                (x.EstimatedHours ?? 0)
                                + (x.QCHours ?? 0));

                        // =========================
                        // CREATE PROPOSAL
                        // =========================

                        var proposalId = Guid.NewGuid();

  

                        var proposal = new Proposal
                        {
                            ProposalId = proposalId,

                            OpportunityId = service.OpportunityId.Value,

                            ServiceId = service.OpportunityServiceId,

                            ProposalNumber =
                                $"PROP_{DateTime.Now:yyyyMMddHHmmss}",


                            ProposalTitle = "Auto Generated Proposal",

                            ScopeOfWork = service.ScopeofWork,

                            Deliverables = service.FinalDeliverableUsage,

                            EstimatedHours = totalEstimatedHours,

                            ProposalStatus = "Draft",

                            CreatedBy = Guid.Parse(_IUserContext.UserId),

                            CreatedOn = DateTime.Now,

                            TenantId = service.TenantId
                        };

                        // =========================
                        // SAVE PROPOSAL
                        // =========================
                        // SAVE PROPOSAL
                        _context.Proposals.Add(proposal);

                        await _context.SaveChangesAsync();

                        // UPDATE SERVICE
                        service.ProposalId = proposalId;

                        await _context.SaveChangesAsync();
                        // =========================
                        // PROPOSAL ACTIVITY
                        // =========================

                        var proposalActivity = new ProposalActivity
                        {
                            ActivityId = Guid.NewGuid(),

                            ProposalId = proposalId,

                            ActivityType = "Proposal Created",

                            Remarks =
                                "Proposal auto created after final approval",

                            ActionBy = Guid.Parse(_IUserContext.UserId),

                            ActionDate = DateTime.UtcNow,

                            TenantId = service.TenantId
                        };

                        _context.ProposalActivities.Add(proposalActivity);

                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    // REMOVE OLD PENDING
                    var oldPendingList = await _context.ApprovalTransactions
                        .Where(x =>
                            x.RecordId == opportunityServiceId &&
                            x.ApprovalStatus == "Pending")
                        .ToListAsync();

                    _context.ApprovalTransactions.RemoveRange(oldPendingList);

                    // CREATE NEXT STEP
                    bool alreadyExists = await _context.ApprovalTransactions
                        .AnyAsync(x =>
                            x.RecordId == opportunityServiceId &&
                            x.StepId == nextStep.StepId &&
                            x.ApprovalStatus == "Pending");

                    if (!alreadyExists)
                    {
                        _context.ApprovalTransactions.Add(new ApprovalTransaction
                        {
                            ApprovalId = Guid.NewGuid(),
                            WorkflowId = nextStep.WorkflowId,
                            RecordId = opportunityServiceId,
                            StepId = nextStep.StepId,
                            ApprovalStatus = "Pending",
                            ActionDate = DateTime.Now
                        });
                    }

                    service.CurrentStepId = nextStep.StepId;
                    service.ScoperStatus = "In Review";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("ApprovalOpportunityList");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> ApproveProposal(Guid proposalId, bool isApproved, string reason)
        //{
        //    var proposal = await _context.Proposals
        //        .FirstOrDefaultAsync(x => x.ProposalId == proposalId);

        //    if (proposal == null)
        //        return NotFound();

        //    if (isApproved)
        //    {
        //        proposal.Status = "ClientApproved";
        //        proposal.ClientApprovedOn = DateTime.Now;

        //        // =========================
        //        // CREATE WORK ORDER HERE
        //        // =========================

        //        var workOrderId = Guid.NewGuid();

        //        var workOrder = new WorkOrder
        //        {
        //            WorkOrderId = workOrderId,
        //            OpportunityId = proposal.OpportunityId,
        //            ServiceId = proposal.ServiceId,
        //            WorkOrderNumber = $"BWO_{DateTime.Now:yyyyMMddHHmmss}",
        //            WorkOrderStatus = "Draft",
        //            CreatedOn = DateTime.Now,
        //            CreatedBy = Guid.Parse(_IUserContext.UserId),
        //            TenantId = proposal.TenantId,

        //            WorkOrderURL =
        //                $"{Request.Scheme}://{Request.Host}/SalesCRM/Opportunity/WorkOrderTemplate/{workOrderId}"
        //        };

        //        _context.WorkOrders.Add(workOrder);
        //    }
        //    else
        //    {
        //        proposal.Status = "Rejected";
        //        proposal.RejectionReason = reason;
        //    }

        //    await _context.SaveChangesAsync();

        //    return Ok();
        //}

        [HttpPost]
        public async Task<IActionResult> RejectOpportunity(
    Guid approvalId,
    Guid opportunityServiceId,
    Guid? opportunityStageId,
    string comments)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // =========================
                // ROLE SETUP
                // =========================
                var currentRoleIds = _IUserContext.RoleIds;

                Guid deliveryManagerRoleId =
                    Guid.Parse("2d3cf690-820e-468f-961f-93db2e2f62af");

                bool isDeliveryManager =
                    currentRoleIds.Contains(deliveryManagerRoleId);

                // =========================
                // GET APPROVAL
                // =========================
                var approval = await _context.ApprovalTransactions
                    .FirstOrDefaultAsync(x =>
                        x.ApprovalId == approvalId &&
                        x.RecordId == opportunityServiceId &&
                        x.ApprovalStatus == "Pending");

                if (approval == null)
                    return NotFound();

                // UPDATE APPROVAL
                approval.ApprovalStatus = "Rejected";
                approval.Remarks = comments;
                approval.ActionDate = DateTime.Now;
                approval.ApprovedBy = Guid.Parse(_IUserContext.UserId);

                // =========================
                // GET SERVICE
                // =========================
                var service = await _context.OpportunityServices
                    .FirstOrDefaultAsync(x =>
                        x.OpportunityServiceId == opportunityServiceId);

                if (service == null)
                    return NotFound();

                // =========================
                // ROLE BASED STAGE
                // =========================
                if (isDeliveryManager)
                {
                    var dmRejectedStageId = await _context.OpportunityStages
                        .Where(x => x.StageName == "DM Rejected")
                        .Select(x => x.StageId)
                        .FirstOrDefaultAsync();

                    service.OpportunityStageId = dmRejectedStageId;
                }
                else
                {
                    // Use stage selected from VIEW
                    if (opportunityStageId.HasValue && opportunityStageId != Guid.Empty)
                    {
                        service.OpportunityStageId = opportunityStageId.Value;
                    }
                }

                // =========================
                // STOP WORKFLOW
                // =========================
                service.ScoperStatus = "Rejected";
                service.CurrentStepId = null;

                // =========================
                // CANCEL OTHER PENDING
                // =========================
                var pendingApprovals = await _context.ApprovalTransactions
                    .Where(x =>
                        x.RecordId == opportunityServiceId &&
                        x.ApprovalStatus == "Pending")
                    .ToListAsync();

                foreach (var item in pendingApprovals)
                {
                    item.ApprovalStatus = "Cancelled";
                    item.ActionDate = DateTime.Now;
                    item.Remarks =
                        "Cancelled due to rejection in previous step";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("ApprovalOpportunityList");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        // =======================================================
        // PROPOSAL TEMPLATE
        // =======================================================

        [HttpGet]
        public async Task<IActionResult> ProposalTemplate(Guid id)
        {
            var proposalData = await (
                from p in _context.Proposals

                join os in _context.OpportunityServices
                    on p.ServiceId equals os.OpportunityServiceId into osGroup
                from os in osGroup.DefaultIfEmpty()

                join o in _context.Opportunities
                    on p.OpportunityId equals o.OpportunityId into oGroup
                from o in oGroup.DefaultIfEmpty()

                join l in _context.Leads
                    on os.LeadId equals l.LeadId into lGroup
                from l in lGroup.DefaultIfEmpty()

                join s in _context.Services
                    on os.ServiceId equals s.ServiceId into sGroup
                from s in sGroup.DefaultIfEmpty()

                join i in _context.Industries
                    on os.ProjectIndustryId equals i.IndustryId into iGroup
                from i in iGroup.DefaultIfEmpty()

                join c in _context.Countries
                    on l.CountryId equals c.CountryId into cGroup
                from c in cGroup.DefaultIfEmpty()

                join ct in _context.CustomerTypes
                    on l.CustomerTypeId equals ct.CustomerTypeId into ctGroup
                from ct in ctGroup.DefaultIfEmpty()

                join cur in _context.Currencies
                    on p.CurrencyId equals cur.CurrencyId into curGroup
                from cur in curGroup.DefaultIfEmpty()

                where p.ProposalId == id

                select new ProposalVM
                {
                    // ===================================================
                    // PROPOSAL
                    // ===================================================

                    ProposalId = p.ProposalId,
                    ProposalNumber = p.ProposalNumber,
                    ProposalTitle = p.ProposalTitle,
                    ProposalStatus = p.ProposalStatus,
                    ProposalURL = p.ProposalURL,
                    EstimatedHours = p.EstimatedHours,
                    ProposedAmount = p.ProposedAmount,
                    CreatedOn = p.CreatedOn,

                    CurrencyCode = cur.CurrencyCode,

                    // ===================================================
                    // CUSTOMER
                    // ===================================================

                    CustomerName = l.FirstName + " " + l.LastName,
                    CustomerCity = l.City,
                    CustomerCountry = c.CountryName,
                    CompanyName = l.CompanyName,
                    Address = l.Address,
                    CustomerBackground = l.CustomerBackground,
                    BusinessType = ct.CustomerTypeName,

                    // ===================================================
                    // PROJECT
                    // ===================================================

                    OpportunityName = o.OpportunityName,
                    ServiceName = s.ServiceName,
                    IndustryName = i.IndustryName,
                    ProjectIndustry = i.IndustryName,

                    ScopeOfWork = os.ScopeofWork,
                    FinalDeliverable = os.FinalDeliverableUsage,
                    ExpectationSetting = os.ExpectationSetting,
                    CustomerResponsibilities = os.CustomerResponsibilities
                }

            ).FirstOrDefaultAsync();

            if (proposalData == null)
                return NotFound();

            return View(proposalData);
        }

        [HttpPost]
        public async Task<IActionResult> ClientApproval(Guid proposalId, string status)
        {
            try
            {
                // =========================
                // GET PROPOSAL
                // =========================
                var proposal = await _context.Proposals
                    .FirstOrDefaultAsync(x => x.ProposalId == proposalId);

                if (proposal == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Proposal not found"
                    });
                }

                WorkOrder workOrder = null;

                // =========================
                // APPROVE FLOW
                // =========================
                if (status == "Approved")
                {
                    proposal.ProposalStatus = "Approved";
                    proposal.ClientApproved = true;
                    proposal.ClientApprovedOn = DateTime.Now;

                    // =========================
                    // CHECK WORK ORDER
                    // =========================
                    workOrder = await _context.WorkOrders
                        .FirstOrDefaultAsync(x => x.ProposalId == proposal.ProposalId);

                    if (workOrder == null)
                    {
                        workOrder = new WorkOrder
                        {
                            WorkOrderId = Guid.NewGuid(),

                            OpportunityId = proposal.OpportunityId,
                            ServiceId = proposal.ServiceId,
                            ProposalId = proposal.ProposalId,

                            WorkOrderNumber = "BWO_" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                            Description = "Auto generated Work Order",

                            WOSentOn = DateTime.Now,
                            EstimatedHours = proposal.EstimatedHours ?? 0,

                            WorkOrderStatus = "Draft",

                            WOAccepted = false,
                            FinanceApproved = false,
                            PaymentLinkSent = false,
                            ProjectCreated = false,

                            ApprovalToken = Guid.NewGuid().ToString(),

                            //ClientEmail = proposal.ClientEmail,
                            //ClientName = proposal.ClientName,

                            //CreatedBy = _IUserContext.UserId,
                            CreatedOn = DateTime.Now,
                            TenantId = _IUserContext.TenantId,

                            IsActive = true,
                            IsDeleted = false
                        };

                        _context.WorkOrders.Add(workOrder);
                    }
                    else
                    {
                        workOrder.OpportunityId = proposal.OpportunityId;
                        workOrder.ServiceId = proposal.ServiceId;
                        workOrder.EstimatedHours = proposal.EstimatedHours ?? workOrder.EstimatedHours;
                        workOrder.Description = workOrder.Description ?? "Updated from Proposal";

                        _context.WorkOrders.Update(workOrder);
                    }

                    // =========================
                    // SAVE FIRST (GET ID)
                    await _context.SaveChangesAsync();

                  
                    workOrder.WorkOrderURL =
                        $"{Request.Scheme}://{Request.Host}/SalesCRM/Opportunity/WorkOrderTemplate/{workOrder.WorkOrderId}";

                    _context.WorkOrders.Update(workOrder);
                    await _context.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        status = "Approved",
                        workOrderId = workOrder.WorkOrderId,
                        workOrderNumber = workOrder.WorkOrderNumber,
                        workOrderUrl = workOrder.WorkOrderURL,
                        redirectUrl = workOrder.WorkOrderURL
                    });
                }

                // =========================
                // REJECT FLOW
                // =========================
                else if (status == "Rejected")
                {
                    proposal.ProposalStatus = "Rejected";
                    proposal.ClientApproved = false;
                    proposal.ClientRejectedReason = "Rejected by client";

                    await _context.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        status = "Rejected"
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "Invalid status"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> WorkOrderTemplate(Guid id)
        {
            var workOrderData = await (
                from wo in _context.WorkOrders

                    // =========================
                    // PROPOSAL JOIN
                    // =========================
                join p in _context.Proposals
                    on wo.ProposalId equals p.ProposalId into pGroup
                from p in pGroup.DefaultIfEmpty()

            

                    // =========================
                    // OPPORTUNITY
                    // =========================
                join o in _context.Opportunities
                    on wo.OpportunityId equals o.OpportunityId into oGroup
                from o in oGroup.DefaultIfEmpty()
                    // =========================
                    // OPPORTUNITY SERVICE
                    // =========================
                join os in _context.OpportunityServices
                    on o.OpportunityId equals os.OpportunityId into osGroup
                from os in osGroup.DefaultIfEmpty()
                    // =========================
                    // LEAD
                    // =========================
                join l in _context.Leads
                    on os.LeadId equals l.LeadId into lGroup
                from l in lGroup.DefaultIfEmpty()

                    // =========================
                    // SERVICE MASTER
                    // =========================
                join s in _context.Services
                    on os.ServiceId equals s.ServiceId into sGroup
                from s in sGroup.DefaultIfEmpty()

                    // =========================
                    // INDUSTRY
                    // =========================
                join i in _context.Industries
                    on os.ProjectIndustryId equals i.IndustryId into iGroup
                from i in iGroup.DefaultIfEmpty()

                    // =========================
                    // COUNTRY
                    // =========================
                join c in _context.Countries
                    on l.CountryId equals c.CountryId into cGroup
                from c in cGroup.DefaultIfEmpty()

                    // =========================
                    // CUSTOMER TYPE
                    // =========================
                join ct in _context.CustomerTypes
                    on l.CustomerTypeId equals ct.CustomerTypeId into ctGroup
                from ct in ctGroup.DefaultIfEmpty()

                    // =========================
                    // CURRENCY
                    // =========================
                join cur in _context.Currencies
                    on p.CurrencyId equals cur.CurrencyId into curGroup
                from cur in curGroup.DefaultIfEmpty()

                where wo.WorkOrderId == id

                select new WorkOrderVM
                {
                    // =========================
                    // WORK ORDER
                    // =========================
                    WorkOrderId = wo.WorkOrderId,
                    WorkOrderNumber = wo.WorkOrderNumber,
                    WorkOrderURL = wo.WorkOrderURL,
                    Description = wo.Description,
                    EstimatedHours = wo.EstimatedHours,
                    CreatedOn = wo.CreatedOn,
                    WorkOrderStatus = wo.WorkOrderStatus,

                    // =========================
                    // PROPOSAL
                    // =========================
                    ProposalId = p.ProposalId,
                    ProposalNumber = p.ProposalNumber,
                    ProposalStatus = p.ProposalStatus,
                    ProposedAmount = p.ProposedAmount,
                    CurrencyCode = cur.CurrencyCode,

                    // =========================
                    // OPPORTUNITY
                    // =========================
                    OpportunityId = o.OpportunityId,
                    OpportunityName = o.OpportunityName,

                    // =========================
                    // CUSTOMER
                    // =========================
                    CustomerName = l.FirstName + " " + l.LastName,
                    CustomerCity = l.City,
                    CustomerCountry = c.CountryName,
                    CompanyName = l.CompanyName,
                    Address = l.Address,
                    CustomerBackground = l.CustomerBackground,
                    ProjectIndustry = i.IndustryName,
                    BusinessType = ct.CustomerTypeName,

                    // =========================
                    // PROJECT
                    // =========================
                    ServiceName = s.ServiceName,
                    IndustryName = i.IndustryName,

                    ScopeOfWork = os.ScopeofWork,
                    FinalDeliverable = os.FinalDeliverableUsage,
                    ExpectationSetting = os.ExpectationSetting,
                    CustomerResponsibilities = os.CustomerResponsibilities
                }

            ).FirstOrDefaultAsync();

            if (workOrderData == null)
            {
                return NotFound();

            }

            return View(workOrderData);
        }

        [HttpPost]
        public async Task<IActionResult> ClientSignOff(Guid workOrderId)
        {
            var workOrder = await _context.WorkOrders
                .FirstOrDefaultAsync(x => x.WorkOrderId == workOrderId);

            if (workOrder == null)
                return NotFound();

            workOrder.WorkOrderStatus = "Accepted";
            workOrder.WOAccepted = true;
            workOrder.WOAcceptedOn = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("ThankYou");
        }
        [HttpPost]
        public async Task<IActionResult> ReScopeOpportunity(
     Guid approvalId,
     Guid opportunityServiceId,
     Guid opportunityStageId,
     string comments)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // =========================
                // CURRENT APPROVAL
                // =========================
                var approval = await _context.ApprovalTransactions
                    .FirstOrDefaultAsync(x => x.ApprovalId == approvalId);

                if (approval == null)
                    return NotFound();

                if (approval.ApprovalStatus == "Approved")
                    return RedirectToAction("ApprovalOpportunityList");

                approval.ApprovalStatus = "Re-Scoping";
                approval.Remarks = comments;
                approval.ActionDate = DateTime.Now;

                // =========================
                // SERVICE
                // =========================
                var service = await _context.OpportunityServices
                    .FirstOrDefaultAsync(x =>
                        x.OpportunityServiceId == opportunityServiceId);

                if (service == null)
                    return NotFound();

                service.OpportunityStageId = opportunityStageId;

                // =========================
                // GET WORKFLOW FIRST STEP (LEVEL 1)
                // =========================
                var firstStep = await _context.WorkflowSteps
                    .Where(x => x.WorkflowId == approval.WorkflowId)
                    .OrderBy(x => x.LevelOrder)
                    .FirstOrDefaultAsync();

                if (firstStep == null)
                    return BadRequest("Workflow configuration missing.");

                // =========================
                // RESET SERVICE WORKFLOW
                // =========================
                service.CurrentStepId = firstStep.StepId;
                service.ScoperStatus = "Re-Scoping";

                // =========================
                // CREATE LEVEL 1 APPROVAL (IF NOT EXISTS)
                // =========================
                var exists = await _context.ApprovalTransactions
                    .AnyAsync(x =>
                        x.RecordId == opportunityServiceId &&
                        x.StepId == firstStep.StepId &&
                        x.ApprovalStatus == "Pending");

                if (!exists)
                {
                    _context.ApprovalTransactions.Add(new ApprovalTransaction
                    {
                        ApprovalId = Guid.NewGuid(),
                        WorkflowId = firstStep.WorkflowId,
                        RecordId = opportunityServiceId,
                        StepId = firstStep.StepId,
                        ApprovalStatus = "Pending",
                        ActionDate = DateTime.Now,
                        Remarks = "Re-scoped back to Level 1"
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("ApprovalOpportunityList");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateApprovalWorkflow()
        {
            var model = new ApprovalWorkflowVM();

            // USERS
            ViewBag.Users = await _identityDbContext.Users
                .Where(x => x.IsActive == true)
                .OrderBy(x => x.FullName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName
                })
                .ToListAsync();

            // SERVICES
            ViewBag.Services = await _context.Services
                .Where(x => x.IsActive == true)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new SelectListItem
                {
                    Value = x.ServiceId.ToString(),
                    Text = x.ServiceName
                })
                .ToListAsync();

            // ROLES
            ViewBag.Roles = await _identityDbContext.Roles
                .Where(x => x.IsActive == true)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToListAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateApprovalWorkflow(ApprovalWorkflowVM model)
        {
            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // =========================
                // VALIDATION
                // =========================

                if (string.IsNullOrWhiteSpace(model.ModuleName))
                    return BadRequest("Module Name is required");

                if (string.IsNullOrWhiteSpace(model.WorkflowName))
                    return BadRequest("Workflow Name is required");

                if (model.Levels == null || !model.Levels.Any())
                    return BadRequest("Add at least one approval level");

                if (model.Levels.Any(x => x.UserIds == null || !x.UserIds.Any()))
                    return BadRequest("Each level must have at least one approver");

                if (model.Levels.Count(x => x.IsFinalLevel) > 1)
                    return BadRequest("Only one final level allowed");

                // =========================
                // MASTER
                // =========================

                var workflow = new ApprovalWorkflowMaster
                {
                    WorkflowId = Guid.NewGuid(),
                    ModuleName = model.ModuleName,
                    WorkflowName = model.WorkflowName,
                    TenantId = Guid.Parse("6200E0AE-F4C7-4509-B618-DC3490EE88D1"),
                    IsActive = true,
                    CreatedBy = Guid.Parse(_IUserContext.UserId),
                    CreatedOn = DateTime.Now
                };

                _context.ApprovalWorkflowMasters.Add(workflow);

                // =========================
                // LEVELS (MULTIPLE USERS SUPPORT)
                // =========================

                foreach (var levelItem in model.Levels.OrderBy(x => x.LevelNo))
                {
                    var levelId = Guid.NewGuid();

                    foreach (var userId in levelItem.UserIds)
                    {
                        var level = new ApprovalWorkflowLevel
                        {
                            WorkflowLevelId = Guid.NewGuid(),
                            WorkflowId = workflow.WorkflowId,

                            LevelNo = levelItem.LevelNo,

                            UserId = userId,   // 🔥 MULTIPLE USERS HERE

                            IsFinalLevel = levelItem.IsFinalLevel,

                            CreatedOn = DateTime.Now
                        };

                        _context.ApprovalWorkflowLevels.Add(level);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] =
                    "Approval workflow created successfully";

                return RedirectToAction("ApprovalWorkflowList");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }



    }
}