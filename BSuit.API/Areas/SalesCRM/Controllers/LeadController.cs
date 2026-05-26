using BSuit.API.Areas.Admin.Models;
using BSuit.API.Infrastructure.Services;
using BSuit.API.Models;
using BSuit.Contracts.Services;
using BSuit.Identity.Models;
using BSuit.Infrastructure.Email;
using BSuit.SalesCRM.Data;
using BSuit.SalesCRM.Entities;
using BSuit.SalesCRM.Services.ILeadService;
using BSuit.SalesCRM.Services.LeadService;
using BSuit.SalesCRM.VM.Lead;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.Mail;

namespace BSuit.API.Areas.SalesCRM.Controllers
{
    [Area("SalesCRM")]
    public class LeadController : Controller
    {
        private readonly LeadService _leadService;
        private readonly SalesCRMContext _context;
        private readonly IdentityDbContext _identity;
        private readonly IEmailService _email;
        private readonly IUserContext _userContext;
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        //private Guid tenantId = Guid.Parse("6200E0AE-F4C7-4509-B618-DC3490EE88D1");
        public LeadController(LeadService lead, SalesCRMContext context, IEmailService email, IUserContext userContext, INotificationService notificationService, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _leadService = lead;
            _context = context;
            _email = email;
            _userContext = userContext;
            _notificationService = notificationService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            LeadDashboardVM vm = new LeadDashboardVM();

            var leads =
                await _context.Leads
                    .Where(x => x.IsActive == true)
                    .ToListAsync();

            // ============================================
            // 1. LEADS BY SOURCE
            // ============================================

            vm.LeadSources =
                leads
                .GroupBy(x => x.LeadSourceId)
                .Select(g => (

                    Label:
                        g.Key.HasValue
                            ? _context.LeadSources
                                .Where(s => s.LeadSourceId == g.Key.Value)
                                .Select(s => s.SourceName)
                                .FirstOrDefault() ?? "Others"
                            : "Others",

                    Value:
                        g.Count()

                ))
                .OrderByDescending(x => x.Value)
                .ToList();

            // ============================================
            // 2. MONTHLY LEAD TREND
            // ============================================

            vm.MonthlyTrend =
    leads
    .Where(x => x.CreatedOn.HasValue)
    .GroupBy(x => new
    {
        Year = x.CreatedOn.Value.Year,
        Month = x.CreatedOn.Value.Month
    })
    .OrderBy(x => x.Key.Year)
    .ThenBy(x => x.Key.Month)
    .Select(g => (

        Label:
            new DateTime(
                g.Key.Year,
                g.Key.Month,
                1).ToString("MMM yyyy"),

        Value:
            g.Count()

    ))
    .ToList();



            // ============================================
            // 4. CUSTOMER TYPES
            // ============================================

            vm.CustomerTypes =
                leads
                .GroupBy(x => x.CustomerTypeId)
                .Select(g => (

                    Label:
                        g.Key.HasValue
                            ? _context.CustomerTypes
                                .Where(c => c.CustomerTypeId == g.Key.Value)
                                .Select(c => c.CustomerTypeName)
                                .FirstOrDefault() ?? "Others"
                            : "Others",

                    Value:
                        g.Count()

                ))
                .OrderByDescending(x => x.Value)
                .ToList();

            // ============================================
            // 5. COUNTRY WISE LEADS
            // ============================================

            //        vm.CountryWiseLeads =
            //leads
            //.GroupBy(x =>

            //    x.CountryId.HasValue

            //        ? _context.Countries
            //            .Where(c => c.CountryId == x.CountryId.Value)
            //            .Select(c => c.RegionId)
            //            .FirstOrDefault()

            //        : null
            //)
            //.Select(g => (

            //    Label:

            //        g.Key.HasValue

            //            ? _context.Regions
            //                .Where(r => r.RegionId == g.Key.Value)
            //                .Select(r => r.RegionName)
            //                .FirstOrDefault() ?? "Others"

            //            : "Others",

            //    Value:
            //        g.Count()

            //))
            //.OrderByDescending(x => x.Value)
            //.ToList();

            // ============================================
            // 6. CONVERSION RATE
            // ============================================

            return View(vm);
        }


        public async Task<IActionResult> ConfigureServicePricing()
        {
            bool isHavingAccess = User.IsInRole(BSuit.API.Infrastructure.Constants.ROLES.Sales_Manager) == true;

            if (!isHavingAccess)
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
            }
            ServicePricingVM model = new ServicePricingVM();

            List<ServiceInfo> services = new List<ServiceInfo>();
            services = await _context.Services.Where(m => m.IsActive == true).Select(p => new ServiceInfo() { ServiceId = p.ServiceId, ServiceName = p.ServiceName }).ToListAsync();
            List<RegionMaster> regions = new List<RegionMaster>();
            regions = await _context.Regions.Where(m => m.IsActive == true).Select(p => new RegionMaster() { RegionId = p.RegionId, RegionName = p.RegionName }).ToListAsync();
            List<CurrencyInfo> curr = new List<CurrencyInfo>();
            curr = await _context.Currencies.Where(m => m.IsActive == true).Select(p => new CurrencyInfo() { CurrencyId = p.CurrencyId, CurrencyCode = p.CurrencyCode }).ToListAsync();

            List<ServicePricingInfo> ServicePricingList = new();
            ServicePricingList = await _context.ServicePricings.Select(p => new ServicePricingInfo() { CurrencyId = p.CurrencyId ?? new Guid(), RegionId = p.RegionId ?? new Guid(), HighPrice = p.HighPrice, LowPrice = p.LowPrice, IsActive = p.IsActive ?? false, ServiceId = p.ServiceId }).ToListAsync();

            model.ServiceList = services;
            model.Regions = regions;
            model.CurrencyList = curr;
            model.ServicePricingList = ServicePricingList;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveServicePricing(
     List<ServicePricingInfo> pricingList)
        {
            try
            {
                if (pricingList == null || !pricingList.Any())
                {
                    return BadRequest("No pricing data found");
                }

                foreach (var item in pricingList)
                {
                    if (item.ServiceId == Guid.Empty)
                        continue;

                    if (item.CurrencyId == Guid.Empty)
                        continue;

                    if (item.RegionId == Guid.Empty)
                        continue;

                    var existing =
                        await _context.ServicePricings
                            .FirstOrDefaultAsync(x =>
                                x.ServicePricingId == item.ServicePricingId);

                    // 🔥 UPDATE
                    if (existing != null)
                    {
                        existing.ServiceId = item.ServiceId;
                        existing.LowPrice = item.LowPrice;
                        existing.HighPrice = item.HighPrice;
                        existing.CurrencyId = item.CurrencyId;
                        existing.IsActive = item.IsActive;
                        existing.RegionId = item.RegionId;
                    }
                    else
                    {
                        // 🔥 INSERT
                        _context.ServicePricings.Add(new ServicePricing
                        {
                            ServicePricingId = Guid.NewGuid(),
                            ServiceId = item.ServiceId,
                            LowPrice = item.LowPrice,
                            HighPrice = item.HighPrice,
                            CurrencyId = item.CurrencyId,
                            IsActive = item.IsActive,
                            RegionId = item.RegionId,
                            CreatedDate = DateTime.Now,
                            TenantId = _userContext.TenantId ?? Guid.Empty
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Pricing configuration saved successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadLeadDocument(Guid leadId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file selected");
                }

                // 🔥 uploads/leaddocuments
                string uploadsRoot = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "leaddocuments");

                // 🔥 Lead specific folder
                string leadFolder = Path.Combine(
                    uploadsRoot,
                    leadId.ToString());

                // 🔥 Create uploads/leaddocuments folder
                if (!Directory.Exists(uploadsRoot))
                {
                    Directory.CreateDirectory(uploadsRoot);
                }

                // 🔥 Create lead folder
                if (!Directory.Exists(leadFolder))
                {
                    Directory.CreateDirectory(leadFolder);
                }

                // 🔥 Original file name
                string originalFileName =
                    Path.GetFileNameWithoutExtension(file.FileName);

                // 🔥 Extension
                string extension =
                    Path.GetExtension(file.FileName);

                // 🔥 Timestamp filename
                string fileName =
                    $"{originalFileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";

                // 🔥 Final file path
                string filePath = Path.Combine(
                    leadFolder,
                    fileName);

                // 🔥 Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 🔥 Relative path
                string relativePath =
                    $"/uploads/leaddocuments/{leadId}/{fileName}";

                // 🔥 Save DB
                _context.LeadAttachments.Add(new LeadAttachment
                {
                    LeadId = leadId,
                    FileName = fileName,
                    FilePath = relativePath,
                    CreatedOn = DateTime.Now,
                    TenantId = _userContext.TenantId ?? Guid.Empty
                });

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    filePath = relativePath
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> Index(
    int pageNumber = 1,
    int pageSize = 50,
    string startDate = null,
    string endDate = null,
    string searchText = null,
    string selectedStage = "All"
)
        {
            bool isHavingAccess =
    User.IsInRole(BSuit.API.Infrastructure.Constants.ROLES.Sales_Executive) ||
    User.IsInRole(BSuit.API.Infrastructure.Constants.ROLES.Sales_Manager);

            if (!isHavingAccess)
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
            }

            LeadListVM model = new LeadListVM();

            var data = await _leadService.GetLeadList(
                _userContext.TenantId ??
                Guid.Parse("6200E0AE-F4C7-4509-B618-DC3490EE88D1"));

            var stageMaster = await _leadService.GetLeadStages();

            // ✅ Parse Dates
            DateTime? start = null;
            DateTime? end = null;

            if (!string.IsNullOrEmpty(startDate))
                start = DateTime.ParseExact(
                    startDate,
                    "dd-MMM-yyyy",
                    CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(endDate))
                end = DateTime.ParseExact(
                    endDate,
                    "dd-MMM-yyyy",
                    CultureInfo.InvariantCulture);

            if (data != null && data.Any())
            {
                // ✅ Date Filter
                if (start.HasValue)
                {
                    data = data
                        .Where(x => x.LeadCreatedOn >= start.Value)
                        .ToList();
                }

                if (end.HasValue)
                {
                    data = data
                        .Where(x => x.LeadCreatedOn <= end.Value)
                        .ToList();
                }
                if (!User.IsInRole(BSuit.API.Infrastructure.Constants.ROLES.Sales_Manager))
                {
                    //Filter based on the Sales Executive
                    data = data.Where(m => m.LeadOwnerId == _userContext.GUID_USERID).ToList();
                }
                // ✅ Search Filter
                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();

                    data = data.Where(x =>

                        (!string.IsNullOrEmpty(x.FirstName) &&
                         x.FirstName.ToLower().Contains(searchText))

                        ||

                        (!string.IsNullOrEmpty(x.LastName) &&
                         x.LastName.ToLower().Contains(searchText))

                        ||

                        (!string.IsNullOrEmpty(x.Email) &&
                         x.Email.ToLower().Contains(searchText))

                        ||

                        (!string.IsNullOrEmpty(x.CompanyName) &&
                         x.CompanyName.ToLower().Contains(searchText))

                        ||

                        (!string.IsNullOrEmpty(x.Phone) &&
                         x.Phone.ToLower().Contains(searchText))

                    ).ToList();
                }

                // 🔥 Stage Counts
                var stageCounts = data
                    .Where(x => !string.IsNullOrEmpty(x.LeadStage))
                    .GroupBy(x => x.LeadStage)
                    .ToDictionary(g => g.Key, g => g.Count());

                // 🔥 Create Ordered Dictionary
                model.StageCounts = new Dictionary<string, int>();

                // ✅ Add ALL tab first
                model.StageCounts.Add("All", data.Count());

                // ✅ Add stages from master
                foreach (var stage in stageMaster.OrderBy(x => x.DisplayOrder))
                {
                    model.StageCounts.Add(
                        stage.StageName,
                        stageCounts.ContainsKey(stage.StageName)
                            ? stageCounts[stage.StageName]
                            : 0
                    );
                }

                // 🔥 Default Stage
                if (string.IsNullOrEmpty(selectedStage))
                    selectedStage = "All";

                model.SelectedStage = selectedStage;

                // 🔥 Stage Filter
                if (!string.IsNullOrEmpty(selectedStage) &&
                    selectedStage != "All")
                {
                    data = data
                        .Where(x => x.LeadStage == selectedStage)
                        .ToList();
                }

                // 🔥 Rejected Tab Sorting
                // Pending approvals first, approved last
                if (selectedStage == "Rejected")
                {
                    data = data
                        .OrderBy(x => x.LeadRejectionApprovedDate != null ? 1 : 0)
                        .ThenByDescending(x => x.LeadCreatedOn)
                        .ToList();
                }
                else
                {
                    // Optional default sorting for other tabs
                    data = data
                        .OrderByDescending(x => x.LeadCreatedOn)
                        .ToList();
                }

                

                // ✅ Total Count
                model.TotalCount = data.Count();

                // ✅ Pagination
                model.LeadList = data
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            else
            {
                model.LeadList = new List<LeadInfo>();
                model.TotalCount = 0;

                model.StageCounts = new Dictionary<string, int>
        {
            { "All", 0 }
        };
            }

            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            model.SalesExecutiveList =
                await _leadService.GetSalesExecutiveList();

            model.RejectReasonsList =
                await _leadService.GetLeadRejectionReasonsList();

            // ✅ Preserve Filters
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.SearchText = searchText;
            ViewBag.SelectedStage = selectedStage;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RejectLead(
           Guid leadId,
           Guid rejectReasonId,
           string remarks)

        {
            try
            {
                var leadRejection = await _context.LeadRejections
                    .FirstOrDefaultAsync(x => x.LeadId == leadId);

                var lead = await _context.Leads
                    .FirstOrDefaultAsync(x => x.LeadId == leadId);

                Guid? existingStage = lead?.LeadStageId;

                var rejectStage = _context.LeadStages.FirstOrDefault(p => p.StageName == "Rejected")?.LeadStageId;

                // INSERT
                if (leadRejection == null)
                {
                    leadRejection = new LeadRejection
                    {
                        LeadRejectionId = Guid.NewGuid(), // Change if your PK differs
                        LeadId = leadId,
                        RejectReasonId = rejectReasonId,
                        RejectionRemarks = remarks,
                        RejectedBy = Guid.Parse(_userContext.UserId),
                        RejectedDate = DateTime.Now,
                        IsActive = true,
                        PrevStageId = existingStage
                    };

                    if (lead != null)
                    {
                        lead.LeadStageId = rejectStage;
                        await _context.LeadRejections.AddAsync(leadRejection);
                    }


                }
                // UPDATE
                else
                {
                    leadRejection.RejectReasonId = rejectReasonId;
                    leadRejection.RejectionRemarks = remarks;
                    leadRejection.RejectedBy = Guid.Parse(_userContext.UserId);
                    leadRejection.RejectedDate = DateTime.Now;

                    if (lead != null)
                    {
                        lead.LeadStageId = rejectStage;
                    }
                }

                await _context.SaveChangesAsync();

                var u = await _userManager.FindByIdAsync(_userContext.UserId);

                string title = "Lead Rejection";
                string message = $"Lead rejection request raised by {u?.FullName}";
                var recipients = await _userManager.GetUsersInRoleAsync(BSuit.API.Infrastructure.Constants.ROLES.Sales_Manager);
                List<string> userIds = new List<string>();
                string? createdBy = _userContext.UserId;
                if (recipients != null && recipients.Count > 0)
                {
                    foreach (var user in recipients)
                    {

                        userIds.Add(user.Id.ToString());
                    }
                }
                if (userIds.Count > 0)
                {
                    await _notificationService.CreateForUsers(title, message, userIds, createdBy);
                }
                return Json(new
                {
                    success = true
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
        public async Task<IActionResult> NDA(Guid leadId, DateTime requestedDate)
        {
            if (leadId == Guid.Empty)
                return BadRequest();

            var lead =
                await _context.Leads
                .FirstOrDefaultAsync(x => x.LeadId == leadId);

            var ndasignature = _context.NDASignatures.FirstOrDefault(m => m.LeadId == leadId);

            var countryDetails = _context.Countries.FirstOrDefault(k => k.CountryId == lead.CountryId);

            if (lead == null)
                return NotFound();

            var model = new NDAAgreementVM
            {
                LeadId = lead.LeadId,

                CustomerName =
                    lead.FirstName + " " + lead.LastName,

                City = lead.City,

                Country = countryDetails?.CountryName,

                RequestedDate =
                    DateTime.Now.ToString("dd-MMM-yyyy"),
                RequestedBy = lead.OwnerId.ToString(),
                NDAData = ndasignature
            };

            return View(model);
        }

        // ============================================
        // SUBMIT NDA
        // ============================================

        [HttpPost]
        public async Task<IActionResult> SubmitAgreement(
      [FromBody] NDAAgreementSubmitVM model)
        {
            if (model == null)
                return BadRequest();

            var leadData =
                await _context.Leads
                    .FirstOrDefaultAsync(m =>
                        m.LeadId == model.LeadId);

            if (leadData == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Lead not found"
                });
            }

            // ============================================
            // SAFE OWNER ID
            // ============================================

            Guid ownerId =
                leadData.OwnerId != null
                    ? Guid.Parse(leadData.OwnerId.ToString())
                    : Guid.NewGuid();

            // ============================================
            // CHECK EXISTING NDA
            // ============================================

            var existingNDA =
                await _context.NDASignatures
                    .FirstOrDefaultAsync(x =>
                        x.LeadId == model.LeadId);

            // ============================================
            // UPDATE EXISTING
            // ============================================

            if (existingNDA != null)
            {
                existingNDA.AcceptNonDisclosureAgreement =
                    model.AcceptedAgreement;

                existingNDA.AcceptScannedDocumentsNDA =
                    model.AcceptScannedDocuments;

                existingNDA.ModifiedBy =
                    ownerId;

                existingNDA.ModifiedDate =
                    DateTime.Now;

                existingNDA.ExecutedDate =
                    DateTime.Now;

                existingNDA.IsActive = true;

                _context.NDASignatures.Update(existingNDA);
            }

            // ============================================
            // INSERT NEW
            // ============================================

            else
            {
                NDASignature data = new NDASignature();

                data.LeadId =
                    model.LeadId;

                data.CreatedBy =
                    ownerId;

                data.AcceptNonDisclosureAgreement =
                    model.AcceptedAgreement;

                data.AcceptScannedDocumentsNDA =
                    model.AcceptScannedDocuments;

                data.CreatedDate =
                    !string.IsNullOrWhiteSpace(model.RequestedDate)
                        ? Convert.ToDateTime(model.RequestedDate)
                        : DateTime.Now;

                data.ExecutedDate =
                    DateTime.Now;

                data.IsActive = true;

                _context.NDASignatures.Add(data);
            }

            // ============================================
            // SAVE
            // ============================================

            await _context.SaveChangesAsync();

            // ============================================
            // RESPONSE
            // ============================================

            return Json(new
            {
                success = true,
                message = "NDA submitted successfully"
            });
        }

        [HttpPost]
        public async Task<IActionResult> AssignSalesExecutive(List<Guid> leadIds)
        {
            try
            {
                // 🔥 Validation
                if (leadIds == null || !leadIds.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "No leads selected"
                    });
                }

                Guid? currentUserId = _userContext.GUID_USERID;

                if (currentUserId == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "User not found"
                    });
                }

                // 🔥 Fetch leads
                var leads = await _context.Leads
                    .Where(x => leadIds.Contains(x.LeadId))
                    .ToListAsync();

                if (!leads.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "No matching leads found"
                    });
                }

                // 🔥 Prepare assignment history
                List<LeadAssignment> assignments = new();

                foreach (var item in leads)
                {
                    // Assign owner
                    item.OwnerId = currentUserId;

                    // Add history
                    assignments.Add(new LeadAssignment
                    {
                        LeadId = item.LeadId,
                        AssignedTo = currentUserId,
                        AssignedDate = DateTime.Now
                    });
                }

                // 🔥 Bulk add
                await _context.LeadAssignments.AddRangeAsync(assignments);

                // 🔥 Single save
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Sales executive assigned successfully"
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

        //[HttpPost]
        //public async Task<IActionResult> AssignSalesExecutive(List<Guid> leadIds)
        //{
        //    try
        //    {
        //        var m = leadIds.AsQueryable();
        //        var leads = await _context.Leads
        //            .Where(x => m.Contains(x.LeadId))
        //            .ToListAsync();
        //        List<LeadAssignment> leadAssignments = new List<LeadAssignment>();
        //        foreach (var item in leads)
        //        {
        //            item.OwnerId = Guid.Parse(_userContext.UserId);

        //            //leadAssignments.Add(new LeadAssignment() { LeadId = item.LeadId, AssignedDate = DateTime.Now, AssignedTo = Guid.Parse(_userContext.UserId), TenantId = _userContext.TenantId });
        //        }

        //        await _context.SaveChangesAsync();

        //        return Json(new
        //        {
        //            success = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = ex.Message
        //        });
        //    }
        //}








        [HttpPost]
        public async Task<IActionResult> ApproveLeadRejection(List<Guid> leadIds)
        {
            try
            {
                var m = leadIds.AsQueryable();

                var rejections = await _context.LeadRejections
                    .Where(x => m.Contains(x.LeadId))
                    .ToListAsync();
                List<Guid?> leadOwners = await _context.Leads
     .Where(p => m.Contains(p.LeadId))
     .Select(p => p.OwnerId)
     .ToListAsync();


                //var _usersData = _userManager.FindByIdAsync();

//                var users = await (
//    from lead in _context.Leads
//    join user in _usersData
//        on lead.OwnerId equals user.UserId
//    where m.Contains(lead.LeadId)
//    select user
//).Distinct().ToListAsync();

                foreach (var item in rejections)
                {
                    item.IsApproved = true;
                    item.ApprovedBy = Guid.Parse(_userContext.UserId);
                    item.ApprovedDate = DateTime.Now;
                }
                var u = await _userManager.FindByIdAsync(_userContext.UserId);

                string title = "Lead Rejection Approval";
                string message = $"Lead rejection request approved by {u?.FullName}";
                var recipients = await _userManager.GetUsersInRoleAsync(BSuit.API.Infrastructure.Constants.ROLES.Sales_Manager);
                List<string> userIds = new List<string>();
                string? createdBy = _userContext.UserId;
                if (recipients != null && recipients.Count > 0)
                {
                    foreach (var user in recipients)
                    {
                        if (u?.UserId.ToString() != user.Id)
                        {
                            userIds.Add(user.Id.ToString());
                        }
                    }
                }
                //userIds.Add()
                if (userIds.Count > 0)
                {
                    await _notificationService.CreateForUsers(title, message, userIds, createdBy);
                }
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true
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
        public async Task<IActionResult> EditLeadConfiguration(
    string serviceName,
    string regionName,
    string planType)
        {
            try
            {
                var serviceID = _context.Services
                    .FirstOrDefault(m =>
                        m.ServiceName.Trim().ToLower()
                        == serviceName.ToLower().Trim())
                    ?.ServiceId;

                var regionID = _context.Regions
                    .FirstOrDefault(m =>
                        m.RegionName.Trim().ToLower()
                        == regionName.ToLower().Trim())
                    ?.RegionId;

                var data = await _context.LeadConfigurations
                    .Where(x =>
                        x.ServiceId == serviceID &&
                        x.RegionId == regionID)
                    .OrderBy(x => x.EffectiveFrom)
                    .ToListAsync();

                if (!data.Any())
                {
                    TempData["ErrorMessage"] =
                        "Lead configuration not found";

                    return RedirectToAction("LeadConfiguration");
                }

                var currentDate = DateTime.Today;

                List<LeadConfiguration> result;

                // CURRENT
                if (planType == "Current")
                {
                    var current = data
                        .Where(x => x.EffectiveFrom <= currentDate)
                        .OrderByDescending(x => x.EffectiveFrom)
                        .Take(1)
                        .ToList();

                    result = current;
                }

                // FUTURE
                else
                {
                    result = data
                        .Where(x => x.EffectiveFrom > currentDate)
                        .OrderBy(x => x.EffectiveFrom)
                        .ToList();
                }

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction("LeadConfiguration");
            }
        }

        [HttpGet]
        public async Task<IActionResult> LeadSummary(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            var lead = await _leadService.GetLeadDetails(id);

            if (lead == null)
                return NotFound();

            // ============================================
            // NDA PAGE LINK
            // ============================================

            lead.NDALink =
    $"{Request.Scheme}://{Request.Host}/SalesCRM/Lead/NDA" +
    $"?leadId={lead.LeadId}" +
    $"&requestedDate={Uri.EscapeDataString(DateTime.Now.ToString("dd-MMM-yyyy"))}";

            // ============================================

            ViewBag.Services =
                _context.Services
                    .Where(m => m.IsActive == true);

            return View(lead);
        }


        [HttpPost]
        public async Task<IActionResult> CreateOpportunity(
    List<Guid> ServiceIds,
    Guid LeadId)
        {
            try
            {
                // ============================================
                // GET LEAD DETAILS
                // ============================================

                var lead = await _context.Leads
                    .FirstOrDefaultAsync(x => x.LeadId == LeadId);

                if (lead == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Lead not found"
                    });
                }

                // ============================================
                // TOTAL OPPORTUNITIES COUNT UNDER LEAD
                // ============================================

                int totalOpportunities = await _context.OpportunityServices
                    .CountAsync(x => x.LeadId == LeadId);

                int nextNumber = totalOpportunities + 1;

                // ============================================
                // READ SERVICES
                // ============================================

                var services = await _context.Services
                    .Where(x => ServiceIds.Contains(x.ServiceId))
                    .Select(x => x.ServiceName)
                    .ToListAsync();

                // ============================================
                // GENERATE SERVICE SHORT NAME
                // ============================================

                string serviceCode = string.Join("",
                    services.Select(service =>
                    {
                        if (string.IsNullOrWhiteSpace(service))
                            return "X";

                        var words = service
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (words.Length == 1)
                        {
                            return words[0].Substring(0, 1).ToUpper();
                        }

                        return string.Concat(
                            words.Select(w => w[0]))
                            .ToUpper();
                    })
                );

                // ============================================
                // GENERATE OPPORTUNITY NUMBER
                // ============================================

                string generatedOpportunityName =
                    $"O_{lead.EnquiryId}_{serviceCode}_{nextNumber.ToString("D3")}";

                if (ServiceIds == null || !ServiceIds.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Please select at least one service"
                    });
                }

                // ============================================
                // COMMON VALUES
                // ============================================

                var opportunityId =
                    Guid.NewGuid();

                var createdOn =
                    DateTime.Now;

                var currentUserId =
                    Guid.Parse(_userContext.UserId);

                var tenantId =
                    _userContext?.TenantId ?? Guid.NewGuid();

                // ============================================
                // DEFAULT STAGE
                // ============================================

                var defaultStage =
                    await _context.OpportunityStages
                        .Where(x => x.StageName == "Interim")
                        .Select(x => x.StageId)
                        .FirstOrDefaultAsync();

                // ============================================
                // CREATE OPPORTUNITY
                // ============================================

                var opp = new Opportunity
                {
                    OpportunityId = opportunityId,
                    OpportunityName = generatedOpportunityName,
                    CreatedOn = createdOn,
                    TenantId = tenantId
                };

                _context.Opportunities.Add(opp);

                // ============================================
                // CREATE MULTIPLE SERVICES
                // ============================================

                foreach (var serviceId in ServiceIds.Distinct())
                {
                    var module = new OpportunityService
                    {
                        OpportunityServiceId = Guid.NewGuid(),
                        OpportunityId = opportunityId,
                        ServiceId = serviceId,
                        LeadId = LeadId,
                        OpportunityStageId = defaultStage,
                        CreatedOn = createdOn,
                        CreatedBy = currentUserId,
                        TenantId = tenantId
                    };

                    Guid? oppStageId = _context.LeadStages.FirstOrDefault(m => m.StageName == "Opportunity")?.LeadStageId;

                    lead.LeadStageId = oppStageId;

                    _context.OpportunityServices.Add(module);
                }

                // ============================================
                // SAVE
                // ============================================

                await _context.SaveChangesAsync();

                // ============================================
                // RESPONSE
                // ============================================

                return Json(new
                {
                    success = true,
                    message = "Opportunity created successfully"
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
        public async Task<IActionResult> SendEmail()
        {
            Guid emailId = Guid.NewGuid();

            try
            {
                var to = Request.Form["To"].ToString();
                var cc = Request.Form["CC"].ToString();
                var bcc = Request.Form["BCC"].ToString();
                var subject = Request.Form["Subject"].ToString();
                var body = Request.Form["BodyHtml"].ToString();
                var parentType = Request.Form["ParentEntityType"].ToString();
                var parentIdStr = Request.Form["ParentEntityId"].ToString();

                if (!Guid.TryParse(parentIdStr, out Guid parentId))
                {
                    return Json(new { success = false, message = "Invalid Parent ID" });
                }

                var files = Request.Form.Files;

                // =============================
                // 🔹 Convert to List
                // =============================
                List<string> toList = to.ToString()
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                List<string> ccList = string.IsNullOrWhiteSpace(cc)
                    ? new List<string>()
                    : cc.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

                List<string> bccList = string.IsNullOrWhiteSpace(bcc)
                    ? new List<string>()
                    : bcc.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

                //Check for Email Communication, If first response then change the Lead stage to contacted or else leave as it is.
                var anyCommunicationHappened = await _context.EmailMessages.AnyAsync(m => m.ParentEntityType == parentType && m.ParentEntityId == parentId);


                if (!anyCommunicationHappened && parentType == "Lead")
                {
                    var lead = await _context.Leads
                        .FirstOrDefaultAsync(p => p.LeadId == parentId);

                    if (lead != null)
                    {
                        var contactedStageId = await _context.LeadStages
                            .Where(x => x.StageName == "Contacted")
                            .Select(x => x.LeadStageId)
                            .FirstOrDefaultAsync();

                        if (contactedStageId != Guid.Empty)
                        {
                            lead.LeadStageId = contactedStageId;
                        }
                    }
                }

                // =============================
                // 🔹 STEP 1: Save as Pending
                // =============================
                var email = new EmailMessage
                {
                    EmailId = emailId,
                    Subject = subject,
                    BodyHtml = body,
                    SentDate = DateTime.Now,
                    Status = "Pending", // 🔥 IMPORTANT
                    ParentEntityType = parentType,
                    ParentEntityId = parentId,
                    CreatedOn = DateTime.Now
                };

                _context.EmailMessages.Add(email);

                AddRecipients(emailId, to, "To");
                AddRecipients(emailId, cc, "CC");
                AddRecipients(emailId, bcc, "BCC");

                // =============================
                // 🔹 Attachments (Save + Prepare)
                // =============================
                List<IFormFile> attachmentList = new();

                if (files != null && files.Count > 0)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            var uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
                            var filePath = Path.Combine(uploadPath, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            _context.EmailAttachments.Add(new EmailAttachment
                            {
                                AttachmentId = Guid.NewGuid(),
                                EmailId = emailId,
                                FileName = file.FileName,
                                FilePath = "/uploads/" + uniqueFileName,
                                MimeType = file.ContentType
                            });

                            attachmentList.Add(file);
                        }
                    }
                }

                // 🔥 SAVE FIRST (important)
                await _context.SaveChangesAsync();

                // =============================
                // 🔹 STEP 2: Try sending
                // =============================
                try
                {
                    await _email.SendAsync(
                        toList,
                        ccList,
                        bccList,
                        subject,
                        body,
                        attachmentList
                    );

                    // ✅ Success
                    email.Status = "Sent";
                    email.SentDate = DateTime.Now;
                }
                catch (Exception ex)
                {
                    // ❌ Failed but already saved
                    email.Status = "Failed";
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = email.Status == "Sent"
                        ? "Email sent successfully"
                        : "Email saved but failed to send"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Unexpected error",
                    error = ex.Message
                });
            }
        }

        private void AddRecipients(Guid emailId, string emails, string type)
        {
            if (string.IsNullOrWhiteSpace(emails)) return;

            var list = emails.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var email in list)
            {
                _context.EmailRecipients.Add(new EmailRecipient
                {
                    RecipientId = Guid.NewGuid(),
                    EmailId = emailId,
                    Address = email.Trim(),
                    Type = type
                });
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            var lead = await _leadService.GetLeadDetails(id);

            if (lead == null)
                return NotFound();

            return View(lead);
        }
        // CREATE
        public async Task<IActionResult> Create()
        {
            var vm = await _leadService.GetLeadEditData(
                _userContext?.TenantId
                    ?? Guid.Parse("6200E0AE-F4C7-4509-B618-DC3490EE88D1"),
                Guid.Empty);

            return View("Edit", vm);
        }

        // EDIT
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            var vm = await _leadService.GetLeadEditData(
                _userContext?.TenantId
                    ?? Guid.Parse("6200E0AE-F4C7-4509-B618-DC3490EE88D1"),
                id);

            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LeadEditVM model)
        {
            // ============================================
            // VALIDATION
            // ============================================

            if (string.IsNullOrWhiteSpace(model.FirstName))
                ModelState.AddModelError(
                    nameof(model.FirstName),
                    "First name is required");

            if (string.IsNullOrWhiteSpace(model.LastName))
                ModelState.AddModelError(
                    nameof(model.LastName),
                    "Last name is required");

            if (string.IsNullOrWhiteSpace(model.Email))
                ModelState.AddModelError(
                    nameof(model.Email),
                    "Email is required");

            if (string.IsNullOrWhiteSpace(model.Phone))
                ModelState.AddModelError(
                    nameof(model.Phone),
                    "Phone is required");

            if (model.Country == null ||
                model.Country == Guid.Empty)
                ModelState.AddModelError(
                    nameof(model.Country),
                    "Country is required");

            if (model.LeadType == null ||
                model.LeadType == Guid.Empty)
                ModelState.AddModelError(
                    nameof(model.LeadType),
                    "Lead Type is required");

            if (model.LeadStage == null ||
                model.LeadStage == Guid.Empty)
                ModelState.AddModelError(
                    nameof(model.LeadStage),
                    "Lead Stage is required");

            if (model.LeadPriority == null ||
                model.LeadPriority == Guid.Empty)
                ModelState.AddModelError(
                    nameof(model.LeadPriority),
                    "Lead Priority is required");

            // ============================================
            // VALIDATION FAILED
            // ============================================

            if (!ModelState.IsValid)
            {
                model.GenderList =
                    await _leadService.GetGenderList();

                model.IndustryList =
                    await _leadService.GetIndustryList();

                model.CompanySizeList =
                    await _leadService.GetCompanySizeList();

                model.LeadTypeList =
                    await _leadService.GetLeadTypeList();

                model.LeadSourceList =
                    await _leadService.GetLeadSourceList();

                model.LeadStageList =
                    await _leadService.GetLeadStageList();

                model.LeadPriorityList =
                    await _leadService.GetLeadPriorityList();

                model.RatingList =
                    await _leadService.GetRatingList();

                model.SalesExecutiveList =
                    await _leadService.GetSalesExecutiveList();

                model.CountryMaster =
                    await _leadService.GetCountryList();

                model.CustomerTypeList =
                    await _leadService.GetCustomerTypeList();

                model.JobTitleList = await _leadService.GetJobTitlesList();

                return View(model);
            }

            // ============================================
            // CREATE / UPDATE
            // ============================================

            Lead existingLead = null;

            if (model.LeadId != Guid.Empty)
            {
                existingLead =
                    await _context.Leads
                        .FirstOrDefaultAsync(x =>
                            x.LeadId == model.LeadId);
            }

            // ============================================
            // CREATE
            // ============================================

            if (existingLead == null)
            {
                string firstLetter = !string.IsNullOrWhiteSpace(model.FirstName)
                    ? model.FirstName.Trim()[0].ToString().ToUpper()
                    : "X";

                string lastLetter = !string.IsNullOrWhiteSpace(model.LastName)
                    ? model.LastName.Trim()[0].ToString().ToUpper()
                    : "X";

                string enquiryId;

                do
                {
                    string randomNumber = Random.Shared
                        .Next(1000000, 9999999)
                        .ToString();

                    enquiryId = $"EQ-{firstLetter}{lastLetter}{randomNumber}";

                } while (await _context.Leads
                    .AnyAsync(x => x.EnquiryId == enquiryId));

                existingLead = new Lead
                {
                    LeadId = Guid.NewGuid(),
                    TenantId = model.TeanantId,
                    CreatedOn = DateTime.Now,
                    EnquiryId = enquiryId,
                    CreatedBy = !string.IsNullOrWhiteSpace(_userContext?.UserId)
                        ? Guid.Parse(_userContext.UserId)
                        : Guid.NewGuid()
                };

                _context.Leads.Add(existingLead);
            }
            else
            {
                existingLead.ModifiedOn =
                    DateTime.Now;

                existingLead.ModifiedBy =
                    !string.IsNullOrWhiteSpace(_userContext?.UserId)
                        ? Guid.Parse(_userContext.UserId)
                        : Guid.NewGuid();

                _context.Leads.Update(existingLead);
            }

            // ============================================
            // BASIC INFO
            // ============================================

            existingLead.FirstName =
                model.FirstName;

            existingLead.LastName =
                model.LastName;


            existingLead.GenderId =
                model.Gender;

            existingLead.JobTitleId =
                model.JobTitle;

            // ============================================
            // CONTACT INFO
            // ============================================

            existingLead.Email =
                model.Email;

            existingLead.Phone =
                model.Phone;

            existingLead.PersonalEmail1 =
                model.PersonalEmail1;

            existingLead.PersonalEmail2 =
                model.PersonalEmail2;

            existingLead.Address =
                model.Address;

            existingLead.City =
                model.City;

            existingLead.ZipCode =
                model.ZipCode;

            existingLead.CountryId =
                model.Country;

            // ============================================
            // COMPANY INFO
            // ============================================

            existingLead.CompanyName =
                model.CompanyName;

            existingLead.Website =
                model.Website;

            existingLead.CompanySizeId =
                model.CompanySize;

            existingLead.CompanyRevenue =
                model.CompanyRevenue;

            existingLead.CompanyRanking =
                model.CompanyRanking;

            existingLead.IndustryId =
                model.Industry;

            // ============================================
            // LEAD INFO
            // ============================================

            existingLead.LeadTypeId =
                model.LeadType;

            existingLead.LeadSourceId =
                model.LeadSource;

            existingLead.LeadStageId =
                model.LeadStage;

            existingLead.LeadPriorityId =
                model.LeadPriority;

            existingLead.RatingId =
                model.Rating;

            existingLead.OwnerId =
                model.SalesExecutive;

            // ============================================
            // ADDITIONAL INFO
            // ============================================

            existingLead.RequirementDetails =
                model.RequirementDetails;

            existingLead.CustomerBackground =
                model.CustomerBackground;

            existingLead.SkpeId =
                model.SkpeId;

            existingLead.TwitterURL =
                model.TwitterURL;

            existingLead.FacebookURL =
                model.FacebookURL;

            existingLead.LinkedinURL =
                model.LinkedinURL;

            existingLead.BDComments =
                model.BDComments;

            existingLead.PreferredModeofCommunication =
                model.PreferredModeofCommunication;

            existingLead.CustomerTypeId =
                model.CustomerType;

            // ============================================
            // STATUS
            // ============================================

            existingLead.IsActive =
                model.IsActive;

            // ============================================
            // SAVE
            // ============================================

            await _context.SaveChangesAsync();

            // ============================================
            // SAVE SERVICES
            // ============================================

            var existingServices =
                _context.LeadServiceMappings
                    .Where(x => x.LeadId == existingLead.LeadId);

            _context.LeadServiceMappings
                .RemoveRange(existingServices);

            if (model.SelectedServices != null &&
                model.SelectedServices.Any())
            {
                var serviceMappings =
                    model.SelectedServices
                        .Select(serviceId =>
                            new LeadServiceMapping
                            {
                                LeadServiceId = Guid.NewGuid(),
                                LeadId = existingLead.LeadId,
                                ServiceId = serviceId.ServiceId
                            })
                        .ToList();

                await _context.LeadServiceMappings
                    .AddRangeAsync(serviceMappings);

                await _context.SaveChangesAsync();
            }

            // ============================================
            // REDIRECT
            // ============================================

            TempData["SuccessMessage"] =
                model.LeadId == Guid.Empty
                    ? "Lead created successfully"
                    : "Lead updated successfully";

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteLeadService(string leadId, string serviceId)
        {
            // ✅ Validate inputs
            if (leadId == null || serviceId == null)
            {
                return BadRequest("Invalid data");
            }
            // ✅ Redirect back (or same page)
            return RedirectToAction("Edit", new { id = leadId });
        }

        public async Task<IActionResult> LeadConfiguration()
        {
            bool isHavingAccess = User.IsInRole(BSuit.API.Infrastructure.Constants.ROLES.Sales_Manager) == true;

            if (!isHavingAccess)
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
            }
            LeadConfigurationVM data = await _leadService.LeadConfigurationVM(_userContext?.TenantId ?? Guid.Parse("6200E0AE-F4C7-4509-B618-DC3490EE88D1"));
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveConfiguration(
    List<Guid> SelectedServices,
    List<Guid> SelectedRegions,
    Guid? PrimarySalesExecutive,
    Guid? SecondarySalesExecutive,
    DateTime EffectiveFrom,
    string action)
        {
            var alerts = new List<AlertMessage>();

            try
            {
                // Prevent null issues
                SelectedServices ??= new List<Guid>();
                SelectedRegions ??= new List<Guid>();

                // Validation
                if (!SelectedServices.Any()
                    || !SelectedRegions.Any()
                    || PrimarySalesExecutive == null)
                {
                    alerts.Add(new AlertMessage
                    {
                        Type = "error",
                        Message = "Please select at least one Service, Region and Primary Sales Executive."
                    });

                    TempData["Alerts"] =
                        Newtonsoft.Json.JsonConvert.SerializeObject(alerts);

                    return RedirectToAction("LeadConfiguration");
                }

                // Optional validation
                if (PrimarySalesExecutive == SecondarySalesExecutive)
                {
                    alerts.Add(new AlertMessage
                    {
                        Type = "error",
                        Message = "Primary and Secondary Sales Executive cannot be same."
                    });

                    TempData["Alerts"] =
                        Newtonsoft.Json.JsonConvert.SerializeObject(alerts);

                    return RedirectToAction("LeadConfiguration");
                }

                Guid ruleId = Guid.NewGuid();

                if (action == "SaveConfiguration")
                {
                    var configurations = new List<LeadConfiguration>();

                    foreach (var serviceId in SelectedServices)
                    {
                        foreach (var regionId in SelectedRegions)
                        {
                            configurations.Add(new LeadConfiguration
                            {
                                LeadConfigId = Guid.NewGuid(),

                                ServiceId = serviceId,
                                RegionId = regionId,
                                // Primary Executive
                                SalesExecutiveId = PrimarySalesExecutive.Value,
                                // Secondary Executive
                                SecondarySalesExecutiveId = SecondarySalesExecutive,
                                RuleId = ruleId,
                                EffectiveFrom = EffectiveFrom,
                                IsActive = true,
                                CreatedOn = DateTime.UtcNow,
                                TenantId = _userContext?.TenantId ?? Guid.Parse("6200E0AE-F4C7-4509-B618-DC3490EE88D1")
                            });
                        }
                    }

                    await _leadService.SaveLeadConfigurationAsync(configurations);

                    alerts.Add(new AlertMessage
                    {
                        Type = "success",
                        Message = "Configuration saved successfully!"
                    });
                }
                else if (action == "execute")
                {
                    alerts.Add(new AlertMessage
                    {
                        Type = "success",
                        Message = "Rule executed successfully!"
                    });
                }

                TempData["Alerts"] =
                    Newtonsoft.Json.JsonConvert.SerializeObject(alerts);

                return RedirectToAction("LeadConfiguration");
            }
            catch (Exception)
            {
                alerts.Add(new AlertMessage
                {
                    Type = "error",
                    Message = "Something went wrong while saving configuration."
                });

                TempData["Alerts"] =
                    Newtonsoft.Json.JsonConvert.SerializeObject(alerts);

                return RedirectToAction("LeadConfiguration");
            }
        }
    }
}
