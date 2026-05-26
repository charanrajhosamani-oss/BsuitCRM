using AngleSharp.Common;
using BSuit.API.Areas.Admin.Models;
using BSuit.API.Areas.SalesCRM.Models;
using BSuit.API.Infrastructure.Services;
using BSuit.API.Models;
using BSuit.Contracts.Services;
using BSuit.Core.Data;
using BSuit.HR.Data;
using BSuit.Identity.Data;
using BSuit.Infrastructure.Email;
using BSuit.SalesCRM.Data;
using BSuit.SalesCRM.Entities;
using BSuit.SalesCRM.Models;
using BSuit.SalesCRM.Services.ILeadService;
using BSuit.SalesCRM.Services.IOpportunityService;
using BSuit.SalesCRM.Services.LeadService;
using BSuit.SalesCRM.VM._Opportunity_;
using BSuit.SalesCRM.VM.Lead;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Communications.OnlineMeetings.GetAllRecordingsmeetingOrganizerUserIdMeetingOrganizerUserIdWithStartDateTimeWithEndDateTime;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ExternalConnectors;
using Microsoft.Graph.Models.Security;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Mono.TextTemplating;
using SixLabors.ImageSharp.Metadata.Profiles.Xmp;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Intrinsics.Arm;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Path = System.IO.Path;

namespace BSuit.API.Areas.SalesCRM.Controllers
{

    [Area("SalesCRM")]
    public class AccountController : Controller
    {
        private readonly LeadService _leadService;
        private readonly SalesCRMContext _crmDbcontext;
        private readonly CoreDbContext _coreDbContext;

        private readonly IdentityDbContext _identityDbContext;
        private readonly INotificationService _iNotificationService;
        private readonly IEmailService _email;
        private readonly IUserContext _iUserContext;

        private bool? _HighPotentialStatus = null;
        public AccountController(SalesCRMContext crmDbcontext, CoreDbContext coreDbContext, IdentityDbContext identityDbContext, IUserContext iUserContext, INotificationService iNotificationService, IEmailService email, LeadService leadService)
        {
            _crmDbcontext = crmDbcontext;
            _coreDbContext = coreDbContext;
            _identityDbContext = identityDbContext;
            _iUserContext = iUserContext;
            _iNotificationService = iNotificationService;
            _email = email;
            _leadService = leadService;


        }


        public async Task<IActionResult> Dashboard()
        {
            AccountDashboardVM vm = new AccountDashboardVM();

            var leads =
                await _crmDbcontext.Accounts

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
                            ? _crmDbcontext.LeadSources
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
                .GroupBy(x => x.CustomerTypeID)
                .Select(g => (

                    Label:
                        g.Key.HasValue
                            ? _crmDbcontext.CustomerTypes
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





        [HttpGet]
        public IActionResult CreateAccount()
        {
            var model = new AccountVM();
            LoadDropdowns(model);
            return View(model);
        }

        [HttpGet]

        public async Task<IActionResult> AccountList(int pageNumber = 1, int pageSize = 50, string startDate = null, string endDate = null, string searchText = null, string selectedStage = "All")  // 🔥 Default All)
        {
            AccountListVM model = new AccountListVM();


            //var data = await _crmDbcontext.Accounts.Where(a => a.OwnerId == _iUserContext.GUID_USERID).OrderByDescending(a => a.CreatedOn).ToListAsync();

            var data = await _crmDbcontext.Accounts.OrderByDescending(a => a.CreatedOn).ToListAsync();

            var stageMaster = await _crmDbcontext.AccountCategorizations.OrderByDescending(a => a.DisplayOrder).ToListAsync();

            // ✅ Parse Dates
            DateTime? start = null;
            DateTime? end = null;

            if (!string.IsNullOrEmpty(startDate))
                start = DateTime.ParseExact(startDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(endDate))
                end = DateTime.ParseExact(endDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture);

            if (data != null && data.Any())
            {
                // ✅ Date Filter
                if (start.HasValue)
                    data = data.Where(x => x.CreatedOn >= start.Value).ToList();

                if (end.HasValue)
                    data = data.Where(x => x.CreatedOn <= end.Value).ToList();

                // ✅ Search Filter
                //if (!string.IsNullOrEmpty(searchText))
                //{
                //    searchText = searchText.ToLower();

                //    data = data.Where(x =>
                //        (!string.IsNullOrEmpty(x.AccountName) &&
                //         x.AccountName.ToLower().Contains(searchText)) ||

                //        (!string.IsNullOrEmpty(x.CompanyName) &&
                //         x.CompanyName.ToLower().Contains(searchText)) ||

                //        (!string.IsNullOrEmpty(x.BusinessEmail) &&
                //         x.BusinessEmail.ToLower().Contains(searchText)) ||

                //        (!string.IsNullOrEmpty(x.CompanyName) &&
                //         x.CompanyName.ToLower().Contains(searchText)) ||

                //        (!string.IsNullOrEmpty(x.PersonalPhone) &&
                //         x.PersonalPhone.ToLower().Contains(searchText))
                //    ).ToList();
                //}

                // 🔗 Joins (same as your logic)
                var industries = _crmDbcontext.Industries.ToList();
                var jobTitles = _crmDbcontext.JobTitles.ToList();
                var customerTypes = _crmDbcontext.CustomerTypes.ToList();
                var countries = _crmDbcontext.Countries.ToList();
                var users = _identityDbContext.Users.ToList();

                model.AccountList = (from A in data
                                     join I in industries on A.IndustryId equals I.IndustryId into iJoin
                                     from I in iJoin.DefaultIfEmpty()

                                     join J in jobTitles on A.JobTitleId equals J.JobTitleId into jJoin
                                     from J in jJoin.DefaultIfEmpty()

                                     join CT in customerTypes on A.CustomerTypeID equals CT.CustomerTypeId into ctJoin
                                     from CT in ctJoin.DefaultIfEmpty()

                                     join CN in countries on A.CountryId equals CN.CountryId into cJoin
                                     from CN in cJoin.DefaultIfEmpty()

                                     join U in users on A.OwnerId equals U.UserId into uJoin
                                     from U in uJoin.DefaultIfEmpty()

                                     select new AccountDto
                                     {
                                         AccountId = A.AccountId,
                                         AccountName = A.AccountName,
                                         CompanyName = A.CompanyName,
                                         CustomerId = A.CustomerId,
                                         IndustryName = I != null ? I.IndustryName : "",
                                         JobTitleName = J != null ? J.JobTitleName : "",
                                         CustomerTypeName = CT != null ? CT.CustomerTypeName : "",
                                         BusinessPhone = A.BusinessPhone,
                                         BusinessEmail = A.BusinessEmail,
                                         PersonalEmail = A.PersonalEmail,
                                         PersonalPhone = A.PersonalPhone,
                                         CountryName = CN != null ? CN.CountryName : "",
                                         UserName = U != null ? U.FullName : "",
                                         AccountCategorizationId = A.AccountCategorizationId
                                     }).ToList();


                // 🔥 Stage Counts
                var stageCounts = model.AccountList.Where(x => x.AccountCategorizationId.HasValue).GroupBy(x => x.AccountCategorizationId).ToDictionary(g => g.Key, g => g.Count());
                // 🔥 Create Ordered Dictionary
                model.StageCounts = new Dictionary<string, int>();
                // ✅ Add ALL tab first
                model.StageCounts.Add("All", model.AccountList.Count());
                // ✅ Add stages from master
                foreach (var stage in stageMaster.OrderBy(x => x.DisplayOrder))
                {
                    model.StageCounts.Add(stage.AccountCategorization1, stageCounts.ContainsKey(stage.AccountCategorizationId) ? stageCounts[stage.AccountCategorizationId] : 0);
                }

                // 🔥 Default Stage
                if (string.IsNullOrEmpty(selectedStage))
                    selectedStage = "All";

                model.SelectedStage = selectedStage;

                // 🔥 Stage Filter

                // ✅ Search Filter
                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();

                    model.AccountList = model.AccountList.Where(x =>
                        (!string.IsNullOrEmpty(x.AccountName) &&
                         x.AccountName.ToLower().Contains(searchText)) ||

                        (!string.IsNullOrEmpty(x.CompanyName) &&
                         x.CompanyName.ToLower().Contains(searchText)) ||

                        (!string.IsNullOrEmpty(x.BusinessEmail) &&
                         x.BusinessEmail.ToLower().Contains(searchText)) ||

                        (!string.IsNullOrEmpty(x.CompanyName) &&
                         x.CompanyName.ToLower().Contains(searchText)) ||

                          (!string.IsNullOrEmpty(x.UserName) &&
                         x.UserName.ToLower().Contains(searchText)) ||

                        (!string.IsNullOrEmpty(x.PersonalPhone) &&
                         x.PersonalPhone.ToLower().Contains(searchText))
                    ).ToList();
                }



                if (!string.IsNullOrEmpty(selectedStage) && selectedStage != "All")
                {
                    Guid _accountCategorizationId = stageMaster
                        .Where(x => x.AccountCategorization1 == selectedStage)
                        .Select(x => x.AccountCategorizationId)
                        .FirstOrDefault();

                    model.AccountList = model.AccountList.Where(x => x.AccountCategorizationId.Value == _accountCategorizationId).ToList();
                }

                // ✅ Total Count
                model.TotalCount = model.AccountList.Count();

                // ✅ Pagination
                model.AccountList = model.AccountList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                model.AccountList = new List<AccountDto>();
                model.TotalCount = 0;

                model.StageCounts = new Dictionary<string, int>
        {
            { "All", 0 }
        };
            }

            model.PageNumber = pageNumber;
            model.PageSize = pageSize;



            // ✅ Preserve Filters
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.SearchText = searchText;
            ViewBag.SelectedStage = selectedStage;

            return View(model);
        }

        [HttpPost]

        public IActionResult SaveAccount([FromForm] IFormCollection frm)
        {
            string customerId = GenerateCustomerID(frm["AccountName"], frm["CompanyName"]);

            bool? highPotentialStatus = null;
            if (frm["HighPotentialStatus"] == "1")
                highPotentialStatus = true;
            else if (frm["HighPotentialStatus"] == "0")
                highPotentialStatus = false;

            var account = new BSuit.SalesCRM.Entities.Account
            {
                LeadId = Guid.NewGuid(),
                CustomerId = customerId,

                AccountName = frm["AccountName"].FirstOrDefault(),
                CompanyName = frm["CompanyName"].FirstOrDefault(),
                Website = frm["Website"].FirstOrDefault(),
                LinkedInURL = frm["LinkedInURL"].FirstOrDefault(),
                FacebookURL = frm["FacebookURL"].FirstOrDefault(),
                TwitterURL = frm["TwitterURL"].FirstOrDefault(),
                SkypeID = frm["SkypeID"].FirstOrDefault(),
                InstantMessengerId = frm["InstantMessengerId"].FirstOrDefault(),

                Address = frm["Address"].FirstOrDefault(),
                State = frm["State"].FirstOrDefault(),
                City = frm["City"].FirstOrDefault(),
                ZipCode = frm["ZipCode"].FirstOrDefault(),
                CompanyRanking = frm["CompanyRanking"].FirstOrDefault(),
                CompanyRevenue = frm["CompanyRevenue"].FirstOrDefault(),
                CustomerBackground = frm["CustomerBackground"].FirstOrDefault(),

                PersonalEmail = frm["PersonalEmail"].FirstOrDefault(),
                PersonalPhone = frm["PersonalPhone"].FirstOrDefault(),
                BusinessPhone = frm["BusinessPhone"].FirstOrDefault(),
                BusinessEmail = frm["BusinessEmail"].FirstOrDefault(),

                HighPotentiaStatus = highPotentialStatus,
                ModifiedBy = _iUserContext.GUID_USERID,
                ModifiedOn = DateTime.Now
            };

            // GUID mappings (safe parsing)
            if (Guid.TryParse(frm["CountryId"], out Guid countryId))
                account.CountryId = countryId;

            if (Guid.TryParse(frm["CompanySizeId"], out Guid sizeId))
                account.CompanySizeId = sizeId;

            if (Guid.TryParse(frm["CustomerTypeID"], out Guid typeId))
                account.CustomerTypeID = typeId;

            if (Guid.TryParse(frm["IndustryId"], out Guid industryId))
                account.IndustryId = industryId;

            if (Guid.TryParse(frm["JobTitleId"], out Guid jobId))
                account.JobTitleId = jobId;

            if (Guid.TryParse(frm["GenderId"], out Guid genderId))
                account.GenderId = genderId;

            if (Guid.TryParse(frm["CustomerCategoryId"], out Guid customerCategoryId))
                account.CustomerCategoryId = customerCategoryId;

            if (Guid.TryParse(frm["AccountSourceId"], out Guid accountSourceId))
                account.AccountSourceId = accountSourceId;

            if (Guid.TryParse(frm["AccountCategorizationId"], out Guid accountCategorizationId))
                account.AccountCategorizationId = accountCategorizationId;

            if (Guid.TryParse(frm["LeadSourceId"], out Guid leadSourceId))
                account.LeadSourceId = leadSourceId;

            if (Guid.TryParse(frm["RegionId"], out Guid regionId))
                account.RegionId = regionId;

            if (Guid.TryParse(frm["ownerId"], out Guid ownerId))
                account.OwnerId = ownerId;

            _crmDbcontext.Accounts.Add(account);
            _crmDbcontext.SaveChanges();

            // Insert new record ReferredName
            var referredName = frm["ReferredName"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(referredName))
            {
                var newReferral = new Referral
                {
                    ReferralId = Guid.NewGuid(),
                    ReferrerAccountId = account.AccountId, // or account.AccountId if that is correct in your model
                    ReferredName = referredName,
                    ReferredPhone = frm["ReferredPhone"].FirstOrDefault(),
                    ReferredEmail = frm["ReferredEmail"].FirstOrDefault(),
                    Status = Convert.ToString(frm["IsReferredActive"]) == "1" ? true : false,
                    CreatedDate = DateTime.Now,
                };
                _crmDbcontext.Referrals.Add(newReferral);
            }

            // Add contact information records
            var contacts = new List<ContactInfo>();
            // Extract indexes dynamically
            var indexes = frm.Keys
                .Where(k => k.StartsWith("contactInfo[") && k.EndsWith("].FullName"))
                .Select(k => k.Split('[', ']')[1])
                .Distinct().ToList();
            List<ContactInfo> ConInfo = new List<ContactInfo>();
            foreach (var index in indexes)
            {
                var fullName = frm[$"contactInfo[{index}].FullName"].FirstOrDefault();
                var department = frm[$"contactInfo[{index}].Department"].FirstOrDefault();
                var designation = frm[$"contactInfo[{index}].Designation"].FirstOrDefault();
                var email = frm[$"contactInfo[{index}].Email"].FirstOrDefault();
                var mobile = frm[$"contactInfo[{index}].MobilePhone"].FirstOrDefault();
                var statusVal = frm[$"contactInfo[{index}].StatusId"].FirstOrDefault();
                bool isActive = Convert.ToString(frm["IsActive"]) == "1" ? true : false;
                ConInfo.Add(new ContactInfo
                {
                    FullName = fullName,
                    Department = department,
                    Designation = designation,
                    Email = email,
                    MobilePhone = mobile,
                    StatusId = statusVal
                });

            }

            var fNames = frm["FullName[]"];
            var _department = frm["Department[]"];
            var _designation = frm["Designation[]"];
            var emails1 = frm["Email[]"];
            var mobiles1 = frm["MobilePhone[]"];
            var statuses1 = frm["Status[]"];
            int cnt = fNames.Count();
            if (cnt > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    var fullName = fNames[i].ToString();
                    var department = _department[i].ToString();
                    var designation = _designation[i].ToString();
                    var email = emails1[i].ToString();
                    var mobile = mobiles1[i].ToString();
                    var statusVal = statuses1[i].ToString();

                    ConInfo.Add(new ContactInfo
                    {
                        FullName = fullName,
                        Department = _department,
                        Designation = _designation,
                        Email = email,
                        MobilePhone = mobile,
                        StatusId = statusVal
                    });
                }
            }
            if (ConInfo.Count > 0)
            {
                foreach (var itm in ConInfo)
                {
                    if (!string.IsNullOrWhiteSpace(itm.MobilePhone))
                    {
                        string mobile1 = itm.MobilePhone?.Trim(); // safer than Convert.ToString

                        var existingContact = _crmDbcontext.Contacts
                            .FirstOrDefault(x => x.AccountId == account.AccountId && x.MobilePhone == mobile1);

                        if (existingContact != null)
                        {
                            existingContact.FullName = itm.FullName;
                            existingContact.Department = itm.Department;
                            existingContact.Designation = itm.Designation;
                            existingContact.Email = itm.Email;
                            existingContact.MobilePhone = mobile1;
                            existingContact.IsActive = Convert.ToString(itm.StatusId) == "1" ? true : false;
                        }
                        else
                        {
                            var contact = new BSuit.SalesCRM.Entities.Contact
                            {
                                AccountId = account.AccountId,
                                FullName = itm.FullName,
                                Department = itm.Department,
                                Designation = itm.Designation,
                                Email = itm.Email,
                                MobilePhone = mobile1,
                                IsActive = Convert.ToString(itm.StatusId) == "1" ? true : false,
                            };

                            _crmDbcontext.Contacts.Add(contact);
                        }
                    }
                }
                // ✅ IMPORTANT: Save once outside loop
                _crmDbcontext.SaveChanges();

                // 4. Update fields
                string accountName = frm["AccountName"].FirstOrDefault();
                string companyName = frm["CompanyName"].FirstOrDefault();
                string title = "Account Created";
                string message = $"Account Name: {accountName}, Company Name: {companyName}";
                string createdBy = Convert.ToString(_iUserContext.GUID_USERID);
                List<string> userIds = new List<string>();
                userIds.Add(Convert.ToString(ownerId));
                _iNotificationService.CreateForUsers(title, message, userIds, createdBy);
            }
            return Json(new
            {
                success = true,
                message = "Account updated successfully!"
            });
        }

        private void LoadDropdowns(AccountVM model)
        {
            model.Countries = _crmDbcontext.Countries.OrderBy(x => x.CountryName).Select(x => new SelectListItem { Value = x.CountryId.ToString(), Text = x.CountryName }).ToList();

            model.CompanySizes = _crmDbcontext.CompanySizes.OrderBy(x => x.DisplayOrder)
                .Select(x => new SelectListItem { Value = x.LeadCompanySizeId.ToString(), Text = x.SizeName }).ToList();

            model.CustomerTypes = _crmDbcontext.CustomerTypes.OrderBy(x => x.DisplayOrder)
                .Select(x => new SelectListItem { Value = x.CustomerTypeId.ToString(), Text = x.CustomerTypeName }).ToList();

            model.Industries = _crmDbcontext.Industries.OrderBy(x => x.DisplayOrder)
                .Select(x => new SelectListItem { Value = x.IndustryId.ToString(), Text = x.IndustryName }).ToList();

            model.JobTitles = _crmDbcontext.JobTitles.OrderBy(x => x.DisplayOrder)
                .Select(x => new SelectListItem { Value = x.JobTitleId.ToString(), Text = x.JobTitleName }).ToList();

            model.Genders = _crmDbcontext.GenderMasters.OrderBy(x => x.DisplayOrder)
                .Select(x => new SelectListItem { Value = x.GenderId.ToString(), Text = x.GenderName }).ToList();
            model.AccountCategory = _crmDbcontext.AccountCategorizations.OrderBy(x => x.DisplayOrder)
               .Select(x => new SelectListItem { Value = x.AccountCategorizationId.ToString(), Text = x.AccountCategorization1 }).ToList();

            model.AccountSource = _crmDbcontext.AccountSources.OrderBy(x => x.DisplayOrder)
              .Select(x => new SelectListItem { Value = x.AccountSourceId.ToString(), Text = x.AccountSourceName }).ToList();

            model.CustomerCategories = _crmDbcontext.CustomerCategories.OrderBy(x => x.DisplayOrder)
           .Select(x => new SelectListItem { Value = x.CustomerCategoryId.ToString(), Text = x.CustomerCategory1 }).ToList();


            model.Regions = _crmDbcontext.Regions.OrderBy(x => x.RegionName)
            .Select(x => new SelectListItem { Value = x.RegionId.ToString(), Text = x.RegionName }).ToList();


            model.LeadSources = _crmDbcontext.LeadSources.OrderBy(x => x.DisplayOrder)
           .Select(x => new SelectListItem { Value = x.LeadSourceId.ToString(), Text = x.SourceName }).ToList();





        }

        public AccountListVM GetAccounts()
        {
            var lstDto = new AccountListVM(); // Make sure AccountListVM has a property: List<AccountDto> AccountList
            try
            {
                var accounts = _crmDbcontext.Accounts.ToList();
                var tenants = _coreDbContext.Tenants.ToList();
                var users = _identityDbContext.Users.ToList();
                var genders = _crmDbcontext.GenderMasters.ToList();
                var countries = _crmDbcontext.Countries.ToList();
                var industries = _crmDbcontext.Industries.ToList();
                var jobTitles = _crmDbcontext.JobTitles.ToList();
                var companySizes = _crmDbcontext.CompanySizes.ToList();
                var customerTypes = _crmDbcontext.CustomerTypes.ToList();

                lstDto.AccountList = (from A in accounts

                                      join T in tenants on A.TenantId equals T.Id into tJoin
                                      from T in tJoin.DefaultIfEmpty()

                                      join AU in users on A.OwnerId equals AU.UserId into uJoin
                                      from AU in uJoin.DefaultIfEmpty()

                                      join G in genders on A.GenderId equals G.GenderId into gJoin
                                      from G in gJoin.DefaultIfEmpty()

                                      join CN in countries on A.CountryId equals CN.CountryId into cJoin
                                      from CN in cJoin.DefaultIfEmpty()

                                      join I in industries on A.IndustryId equals I.IndustryId into iJoin
                                      from I in iJoin.DefaultIfEmpty()

                                      join J in jobTitles on A.JobTitleId equals J.JobTitleId into jJoin
                                      from J in jJoin.DefaultIfEmpty()

                                      join CM in companySizes on A.CompanySizeId equals CM.LeadCompanySizeId into cmJoin
                                      from CM in cmJoin.DefaultIfEmpty()

                                      join CT in customerTypes on A.CustomerTypeID equals CT.CustomerTypeId into ctJoin
                                      from CT in ctJoin.DefaultIfEmpty()

                                      select new AccountDto
                                      {
                                          AccountId = A.AccountId,
                                          AccountName = A.AccountName,
                                          CompanyName = A.CompanyName,
                                          CompanyRanking = A.CompanyRanking,

                                          TenantName = T != null ? T.Name : "",
                                          UserName = AU != null ? AU.FullName : "",

                                          LeadId = Guid.TryParse(A.LeadId?.ToString(), out Guid leadId)
                                                      ? leadId
                                                      : Guid.Empty,

                                          CustomerId = A.CustomerId,

                                          IndustryId = A.IndustryId,
                                          IndustryName = I != null ? I.IndustryName : "",
                                          GenderId = A.GenderId,
                                          GenderName = G != null ? G.GenderName : "",
                                          JobTitleId = A.JobTitleId,
                                          JobTitleName = J != null ? J.JobTitleName : "",
                                          CustomerBackground = A.CustomerBackground,
                                          CompanySizeId = A.CompanySizeId,
                                          SizeName = CM != null ? CM.SizeName : "",
                                          CompanyRevenue = A.CompanyRevenue,
                                          CustomerTypeID = A.CustomerTypeID,
                                          CustomerTypeName = CT != null ? CT.CustomerTypeName : "",
                                          CountryName = CN.CountryName,
                                          BusinessPhone = A.BusinessPhone,
                                          BusinessEmail = A.BusinessEmail,
                                          PersonalEmail = A.PersonalEmail,
                                          PersonalPhone = A.PersonalPhone,
                                          Website = A.Website,
                                          CountryId = A.CountryId,
                                          State = A.State,
                                          City = A.City,
                                          Address = A.Address,
                                          ZipCode = A.ZipCode,
                                          OwnerId = A.OwnerId,
                                      }).ToList();

                return lstDto;
            }
            catch (Exception ex)
            {
                // Optional: log ex.Message
                throw;
            }
        }



        public JsonResult SearchGetAccounts(string searchText, string startDate, string endDate)
        {
            var lstDto = new AccountListVM();

            try
            {
                var accounts = _crmDbcontext.Accounts.AsQueryable();

                // 🔍 SEARCH FILTER
                if (!string.IsNullOrEmpty(searchText))
                {
                    accounts = accounts.Where(A =>
                        A.AccountName.Contains(searchText) ||
                        A.CompanyName.Contains(searchText) ||
                        A.PersonalPhone.Contains(searchText));
                }

                // 📅 DATE FILTER
                if (!string.IsNullOrEmpty(startDate))
                {
                    DateTime start = Convert.ToDateTime(startDate);
                    accounts = accounts.Where(A => A.CreatedOn >= start);
                }

                if (!string.IsNullOrEmpty(endDate))
                {
                    DateTime end = Convert.ToDateTime(endDate);
                    accounts = accounts.Where(A => A.CreatedOn <= end);
                }

                var tenants = _coreDbContext.Tenants.ToList();
                var users = _identityDbContext.Users.ToList();
                var genders = _crmDbcontext.GenderMasters.ToList();
                var countries = _crmDbcontext.Countries.ToList();
                var industries = _crmDbcontext.Industries.ToList();
                var jobTitles = _crmDbcontext.JobTitles.ToList();
                var companySizes = _crmDbcontext.CompanySizes.ToList();
                var customerTypes = _crmDbcontext.CustomerTypes.ToList();

                lstDto.AccountList = (from A in accounts.ToList()

                                      join T in tenants on A.TenantId equals T.Id into tJoin
                                      from T in tJoin.DefaultIfEmpty()

                                      join AU in users on A.OwnerId equals AU.UserId into uJoin
                                      from AU in uJoin.DefaultIfEmpty()

                                      join G in genders on A.GenderId equals G.GenderId into gJoin
                                      from G in gJoin.DefaultIfEmpty()

                                      join CN in countries on A.CountryId equals CN.CountryId into cJoin
                                      from CN in cJoin.DefaultIfEmpty()

                                      join I in industries on A.IndustryId equals I.IndustryId into iJoin
                                      from I in iJoin.DefaultIfEmpty()

                                      join J in jobTitles on A.JobTitleId equals J.JobTitleId into jJoin
                                      from J in jJoin.DefaultIfEmpty()

                                      join CM in companySizes on A.CompanySizeId equals CM.LeadCompanySizeId into cmJoin
                                      from CM in cmJoin.DefaultIfEmpty()

                                      join CT in customerTypes on A.CustomerTypeID equals CT.CustomerTypeId into ctJoin
                                      from CT in ctJoin.DefaultIfEmpty()

                                      select new AccountDto
                                      {
                                          AccountId = A.AccountId,
                                          AccountName = A.AccountName,
                                          CompanyName = A.CompanyName,
                                          IndustryName = I != null ? I.IndustryName : "",
                                          JobTitleName = J != null ? J.JobTitleName : "",
                                          CustomerTypeName = CT != null ? CT.CustomerTypeName : "",
                                          PersonalPhone = A.PersonalPhone,
                                          CountryName = CN != null ? CN.CountryName : "",
                                          UserName = AU != null ? AU.FullName : "",
                                          OwnerId = A.OwnerId
                                      }).ToList();

                return Json(lstDto); // ✅ return JSON
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Export to excel
        public async Task<IActionResult> ExportToExcel(string startDate = null, string endDate = null, string searchText = null, string selectedStage = "All")
        {
            var data = await _crmDbcontext.Accounts.OrderByDescending(a => a.CreatedOn).ToListAsync();

            var stageMaster = await _crmDbcontext.AccountCategorizations.ToListAsync();

            // Date parsing
            DateTime? start = !string.IsNullOrEmpty(startDate)
                ? DateTime.ParseExact(startDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture)
                : null;

            DateTime? end = !string.IsNullOrEmpty(endDate)
                ? DateTime.ParseExact(endDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture)
                : null;

            // Filters
            if (start.HasValue)
                data = data.Where(x => x.CreatedOn >= start.Value).ToList();

            if (end.HasValue)
                data = data.Where(x => x.CreatedOn <= end.Value).ToList();

            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.ToLower();
                data = data.Where(x =>
                    (!string.IsNullOrEmpty(x.AccountName) && x.AccountName.ToLower().Contains(searchText)) ||
                    (!string.IsNullOrEmpty(x.CompanyName) && x.CompanyName.ToLower().Contains(searchText)) ||
                    (!string.IsNullOrEmpty(x.BusinessEmail) && x.BusinessEmail.ToLower().Contains(searchText)) ||
                    (!string.IsNullOrEmpty(x.PersonalPhone) && x.PersonalPhone.ToLower().Contains(searchText))
                ).ToList();
            }

            // Stage filter
            if (!string.IsNullOrEmpty(selectedStage) && selectedStage != "All")
            {
                var stageId = stageMaster
                    .Where(x => x.AccountCategorization1 == selectedStage)
                    .Select(x => x.AccountCategorizationId)
                    .FirstOrDefault();

                data = data.Where(x => x.AccountCategorizationId == stageId).ToList();
            }

            // Join extra tables
            var industries = _crmDbcontext.Industries.ToList();
            var jobTitles = _crmDbcontext.JobTitles.ToList();
            var customerTypes = _crmDbcontext.CustomerTypes.ToList();
            var countries = _crmDbcontext.Countries.ToList();
            var users = _identityDbContext.Users.ToList();

            var exportData = (from A in data
                              join I in industries on A.IndustryId equals I.IndustryId into iJoin
                              from I in iJoin.DefaultIfEmpty()

                              join J in jobTitles on A.JobTitleId equals J.JobTitleId into jJoin
                              from J in jJoin.DefaultIfEmpty()

                              join CT in customerTypes on A.CustomerTypeID equals CT.CustomerTypeId into ctJoin
                              from CT in ctJoin.DefaultIfEmpty()

                              join CN in countries on A.CountryId equals CN.CountryId into cJoin
                              from CN in cJoin.DefaultIfEmpty()

                              join U in users on A.OwnerId equals U.UserId into uJoin
                              from U in uJoin.DefaultIfEmpty()

                              select new
                              {
                                  A.AccountName,
                                  A.CompanyName,
                                  A.BusinessEmail,
                                  A.BusinessPhone,
                                  A.PersonalPhone,
                                  Industry = I?.IndustryName,
                                  JobTitle = J?.JobTitleName,
                                  CustomerType = CT?.CustomerTypeName,
                                  Country = CN?.CountryName,
                                  Owner = U?.FullName,
                                  A.CreatedOn
                              }).ToList();

            // Create Excel
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Accounts");

            // Header
            worksheet.Cell(1, 1).Value = "Account Name";
            worksheet.Cell(1, 2).Value = "Company Name";
            worksheet.Cell(1, 3).Value = "Business Email";
            worksheet.Cell(1, 4).Value = "Business Phone";
            worksheet.Cell(1, 5).Value = "Personal Phone";
            worksheet.Cell(1, 6).Value = "Industry";
            worksheet.Cell(1, 7).Value = "Job Title";
            worksheet.Cell(1, 8).Value = "Customer Type";
            worksheet.Cell(1, 9).Value = "Country";
            worksheet.Cell(1, 10).Value = "Owner";
            worksheet.Cell(1, 11).Value = "Created On";

            int row = 2;

            foreach (var item in exportData)
            {
                worksheet.Cell(row, 1).Value = item.AccountName;
                worksheet.Cell(row, 2).Value = item.CompanyName;
                worksheet.Cell(row, 3).Value = item.BusinessEmail;
                worksheet.Cell(row, 4).Value = item.BusinessPhone;
                worksheet.Cell(row, 5).Value = item.PersonalPhone;
                worksheet.Cell(row, 6).Value = item.Industry;
                worksheet.Cell(row, 7).Value = item.JobTitle;
                worksheet.Cell(row, 8).Value = item.CustomerType;
                worksheet.Cell(row, 9).Value = item.Country;
                worksheet.Cell(row, 10).Value = item.Owner;
                worksheet.Cell(row, 11).Value = item.CreatedOn?.ToString("dd-MMM-yyyy");

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Accounts.xlsx"
            );
        }
        // 🔍 Autocomplete API

        [HttpGet]
        public JsonResult GetOwners(string term)
        {
            var result = _identityDbContext.Users
                .Where(x => x.FullName.Contains(term))
                .Select(x => new
                {
                    label = x.FullName, // shown in dropdown
                    value = x.FullName, // filled in textbox
                    id = x.UserId
                })
                .ToList();

            return Json(result);
        }

        [HttpGet]
        public IActionResult EditAccount(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                return BadRequest(); // Invalid request

            // Parse string to Guid
            if (!Guid.TryParse(accountId, out Guid accGuid))
            {
                return BadRequest("Invalid AccountId"); // handle invalid Guid
            }

            // Load account details
            var accountDto = _crmDbcontext.Accounts
                                          .FirstOrDefault(x => x.AccountId == accGuid);


            string usersName = _identityDbContext.Users.Where(x => x.UserId == accountDto.OwnerId).Select(x => x.FullName).FirstOrDefault();
            if (accountDto == null)
                return NotFound(); // Account not found

            string Status = string.Empty;

            var value = Convert.ToString(accountDto.HighPotentiaStatus);

            if (!string.IsNullOrEmpty(value))
            {
                if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    Status = "1";
                }
                else if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    Status = "0";
                }
            }
            // Map DTO to ViewModel
            var model = new AccountVM
            {
                AccountId = accountDto.AccountId,
                LeadId = accountDto.LeadId ?? Guid.Empty,
                LeadSourceId = accountDto.LeadSourceId ?? Guid.Empty,
                AccountName = accountDto.AccountName,
                CompanyName = accountDto.CompanyName,
                CustomerId = accountDto.CustomerId,
                CompanyRanking = accountDto.CompanyRanking,
                IndustryId = accountDto.IndustryId,
                GenderId = accountDto.GenderId,
                JobTitleId = accountDto.JobTitleId,
                CustomerBackground = accountDto.CustomerBackground,
                CompanySizeId = accountDto.CompanySizeId,
                CompanyRevenue = accountDto.CompanyRevenue,
                CustomerTypeID = accountDto.CustomerTypeID,
                Website = accountDto.Website,
                CountryId = accountDto.CountryId,
                State = accountDto.State,
                City = accountDto.City,
                Address = accountDto.Address,
                ZipCode = accountDto.ZipCode,
                OwnerName = usersName,
                OwnerId = accountDto.OwnerId,
                CustomerCategoryId = accountDto.CustomerCategoryId,
                AccountCategorizationId = accountDto.AccountCategorizationId,
                HighPotentialStatus = Status,
                BusinessEmail = accountDto.BusinessEmail,
                BusinessPhone = accountDto.BusinessPhone,
                PersonalEmail = accountDto.PersonalEmail,
                PersonalPhone = accountDto.PersonalPhone,
                AccountSourceId = accountDto.AccountSourceId,
                TenantId = accountDto.TenantId ?? Guid.Empty,
                RegionId = accountDto.RegionId,
                IsActive = (accountDto.IsActive == true) ? "1" : "0",
                LinkedInURL = accountDto.LinkedInURL,
                FacebookURL = accountDto.FacebookURL,
                TwitterURL = accountDto.TwitterURL,
                SkypeID = accountDto.SkypeID,
                InstantMessengerId = accountDto.InstantMessengerId,
            };
            // Load Referrer Account details
            var referral = _crmDbcontext.Referrals.Where(x => x.ReferrerAccountId == accGuid).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            if (referral != null)
            {
                model.ReferredEmail = referral.ReferredEmail;
                model.ReferredPhone = referral.ReferredPhone;
                model.ReferredName = referral.ReferredName;
                model.IsReferredActive = (referral.Status == true) ? "1" : "0";
            }


            model.referralInfo = (from CT in _crmDbcontext.Referrals

                                  where CT.ReferrerAccountId == accGuid
                                  orderby CT.CreatedDate descending
                                  select new ReferralVM
                                  {
                                      ReferredName = CT.ReferredName,
                                      ReferredEmail = CT.ReferredEmail,
                                      ReferredPhone = CT.ReferredPhone,
                                  }).ToList();

            model.contactInfo = (from CT in _crmDbcontext.Contacts
                                     //join S in _crmDbcontext.SalutationMasters on CT.SalutationId equals S.SalutationId
                                 where CT.AccountId == accGuid
                                 orderby CT.CreatedOn descending
                                 select new ContactInfo
                                 {
                                     FullName = CT.FullName,
                                     Department = CT.Department,
                                     Designation = CT.Designation,
                                     Email = CT.Email,
                                     MobilePhone = CT.MobilePhone,
                                     StatusId = (CT.IsActive == true) ? "1" : "0",
                                 }).ToList();
            // Load dropdowns for edit
            LoadDropdowns(model);

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateAccount([FromForm] IFormCollection frm)
        {
            try
            {
                // 1. Validate AccountId
                if (string.IsNullOrEmpty(frm["AccountId"]))
                    return BadRequest("AccountId is required.");



                if (!Guid.TryParse(frm["AccountId"], out Guid accGuid))
                    return BadRequest("Invalid AccountId.");

                // 2. Load existing account
                var accountDto = _crmDbcontext.Accounts
                                              .FirstOrDefault(x => x.AccountId == accGuid);

                if (accountDto == null)
                    return NotFound("Account not found.");

                // 3. High Potential
                bool? highPotentialStatus = null;

                if (frm["HighPotentialStatus"] == "1")
                    highPotentialStatus = true;
                else if (frm["HighPotentialStatus"] == "0")
                    highPotentialStatus = false;

                // 4. Update fields
                accountDto.AccountName = frm["AccountName"].FirstOrDefault();
                accountDto.CompanyName = frm["CompanyName"].FirstOrDefault();
                accountDto.Website = frm["Website"].FirstOrDefault();
                accountDto.LinkedInURL = frm["LinkedInURL"].FirstOrDefault();
                accountDto.FacebookURL = frm["FacebookURL"].FirstOrDefault();
                accountDto.TwitterURL = frm["TwitterURL"].FirstOrDefault();
                accountDto.SkypeID = frm["SkypeID"].FirstOrDefault();
                accountDto.InstantMessengerId = frm["InstantMessengerId"].FirstOrDefault();



                accountDto.Address = frm["Address"].FirstOrDefault();
                accountDto.State = frm["State"].FirstOrDefault();
                accountDto.City = frm["City"].FirstOrDefault();
                accountDto.ZipCode = frm["ZipCode"].FirstOrDefault();
                accountDto.CompanyRanking = frm["CompanyRanking"].FirstOrDefault();
                accountDto.CompanyRevenue = frm["CompanyRevenue"].FirstOrDefault();
                accountDto.CustomerBackground = frm["CustomerBackground"].FirstOrDefault();

                // Correct phone/email mapping
                accountDto.PersonalEmail = frm["PersonalEmail"].FirstOrDefault();
                accountDto.PersonalPhone = frm["PersonalPhone"].FirstOrDefault();
                accountDto.BusinessPhone = frm["BusinessPhone"].FirstOrDefault();
                accountDto.BusinessEmail = frm["BusinessEmail"].FirstOrDefault();




                // 5. Safe Guid updates
                if (Guid.TryParse(frm["CountryId"], out Guid countryId))
                    accountDto.CountryId = countryId;

                if (Guid.TryParse(frm["CompanySizeId"], out Guid sizeId))
                    accountDto.CompanySizeId = sizeId;

                if (Guid.TryParse(frm["CustomerTypeID"], out Guid typeId))
                    accountDto.CustomerTypeID = typeId;

                if (Guid.TryParse(frm["IndustryId"], out Guid industryId))
                    accountDto.IndustryId = industryId;

                if (Guid.TryParse(frm["JobTitleId"], out Guid jobId))
                    accountDto.JobTitleId = jobId;

                if (Guid.TryParse(frm["GenderId"], out Guid genderId))
                    accountDto.GenderId = genderId;

                if (Guid.TryParse(frm["CustomerCategoryId"], out Guid customerCategoryId))
                    accountDto.CustomerCategoryId = customerCategoryId;

                if (Guid.TryParse(frm["AccountSourceId"], out Guid accountSourceId))
                    accountDto.AccountSourceId = accountSourceId;

                if (Guid.TryParse(frm["AccountCategorizationId"], out Guid AccountCategorizationId))
                    accountDto.AccountCategorizationId = AccountCategorizationId;

                if (Guid.TryParse(frm["LeadSourceId"], out Guid LeadSourceId))
                    accountDto.LeadSourceId = LeadSourceId;

                if (Guid.TryParse(frm["RegionId"], out Guid regionId))
                    accountDto.RegionId = regionId;

                if (Guid.TryParse(frm["ownerId"], out Guid ownerId))
                    accountDto.OwnerId = ownerId;
                // 6. Final fields
                accountDto.HighPotentiaStatus = highPotentialStatus;
                accountDto.ModifiedBy = _iUserContext.GUID_USERID;
                accountDto.ModifiedOn = DateTime.Now;
                // 7. Save
                _crmDbcontext.SaveChanges();

                // Add Refral information records
                var referralLst = new List<ReferralVM>();
                // Extract indexes dynamically
                var indexesRef = frm.Keys
                    .Where(k => k.StartsWith("referralInfo[") && k.EndsWith("].ReferredPhone"))
                    .Select(k => k.Split('[', ']')[1])
                    .Distinct().ToList();
                List<ReferralVM> RefInfo = new List<ReferralVM>();
                foreach (var index in indexesRef)
                {
                    var referredName = frm[$"referralInfo[{index}].ReferredName"].FirstOrDefault();
                    var referredPhone = frm[$"referralInfo[{index}].ReferredPhone"].FirstOrDefault();
                    var referredEmail = frm[$"referralInfo[{index}].ReferredEmail"].FirstOrDefault();
                    var statusVal = frm[$"referralInfo[{index}].StatusId"].FirstOrDefault();
                    bool isActive = Convert.ToString(frm["IsActive"]) == "1" ? true : false;
                    RefInfo.Add(new ReferralVM
                    {
                        ReferredName = referredName,
                        ReferredPhone = referredPhone,
                        ReferredEmail = referredEmail,
                        StatusId = statusVal
                    });

                }
                var ReflName = frm["ReferredName[]"];
                var ReflEmail = frm["ReferredEmail[]"];
                var ReflPhone = frm["ReferredPhone[]"];
                var RelStatusId = frm["StatusId[]"];

                int cntRefl = ReflName.Count();
                if (cntRefl > 0)
                {
                    for (int i = 0; i < cntRefl; i++)
                    {
                        var reflName = ReflName[i].ToString();
                        var reflEmail = ReflEmail[i].ToString();
                        var reflPhone = ReflPhone[i].ToString();
                        var reflstatus = RelStatusId[i].ToString();
                        RefInfo.Add(new ReferralVM
                        {
                            ReferredName = reflName,
                            ReferredPhone = reflPhone,
                            ReferredEmail = reflEmail,
                            StatusId = reflstatus
                        });
                    }
                }
                if (RefInfo.Count > 0)
                {
                    foreach (var itm in RefInfo)
                    {
                        if (!string.IsNullOrWhiteSpace(itm.ReferredPhone))
                        {
                            string mobile1 = itm.ReferredPhone?.Trim(); // safer than Convert.ToString

                            var existingReferrals = _crmDbcontext.Referrals
                                .FirstOrDefault(x => x.ReferrerAccountId == accGuid && x.ReferredPhone == mobile1);

                            if (existingReferrals != null)
                            {
                                existingReferrals.ReferredName = itm.ReferredName;
                                existingReferrals.ReferredPhone = itm.ReferredPhone;
                                existingReferrals.ReferredEmail = mobile1;
                                existingReferrals.Status = Convert.ToString(itm.StatusId) == "1" ? true : false;
                            }
                            else
                            {
                                var refl = new BSuit.SalesCRM.Entities.Referral
                                {
                                    ReferrerAccountId = accGuid,
                                    ReferredName = itm.ReferredName,
                                    ReferredEmail = itm.ReferredEmail,
                                    ReferredPhone = mobile1,
                                    Status = Convert.ToString(itm.StatusId) == "1" ? true : false,
                                };

                                _crmDbcontext.Referrals.Add(refl);
                            }
                        }
                    }
                    // ✅ IMPORTANT: Save once outside loop
                    _crmDbcontext.SaveChanges();
                }





                // Add contact information records
                var contacts = new List<ContactInfo>();
                // Extract indexes dynamically
                var indexes = frm.Keys
                    .Where(k => k.StartsWith("contactInfo[") && k.EndsWith("].FullName"))
                    .Select(k => k.Split('[', ']')[1])
                    .Distinct().ToList();
                List<ContactInfo> ConInfo = new List<ContactInfo>();
                foreach (var index in indexes)
                {
                    var fullName = frm[$"contactInfo[{index}].FullName"].FirstOrDefault();
                    var departmentName = frm[$"contactInfo[{index}].Department"].FirstOrDefault();
                    var designationName = frm[$"contactInfo[{index}].Designation"].FirstOrDefault();
                    var email = frm[$"contactInfo[{index}].Email"].FirstOrDefault();
                    var mobile = frm[$"contactInfo[{index}].MobilePhone"].FirstOrDefault();
                    var statusVal = frm[$"contactInfo[{index}].StatusId"].FirstOrDefault();
                    bool isActive = Convert.ToString(frm["IsActive"]) == "1" ? true : false;
                    ConInfo.Add(new ContactInfo
                    {
                        FullName = fullName,
                        Department = departmentName,
                        Designation = designationName,
                        Email = email,
                        MobilePhone = mobile,
                        StatusId = statusVal
                    });

                }

                var fNames = frm["FullName[]"];
                var depName = frm["Department[]"];
                var desName = frm["Designation[]"];
                var emails1 = frm["Email[]"];
                var mobiles1 = frm["MobilePhone[]"];
                var statuses1 = frm["Status[]"];

                int cnt = fNames.Count();
                if (cnt > 0)
                {
                    for (int i = 0; i < cnt; i++)
                    {
                        var fullName = fNames[i].ToString();
                        var DepName = depName[i].ToString();
                        var DesName = desName[i].ToString();

                        var email = emails1[i].ToString();
                        var mobile = mobiles1[i].ToString();
                        var statusVal = statuses1[i].ToString();

                        ConInfo.Add(new ContactInfo
                        {
                            FullName = fullName,
                            Department = DepName,
                            Designation = DesName,
                            Email = email,
                            MobilePhone = mobile,
                            StatusId = statusVal
                        });
                    }
                }
                if (ConInfo.Count > 0)
                {
                    foreach (var itm in ConInfo)
                    {
                        if (!string.IsNullOrWhiteSpace(itm.MobilePhone))
                        {
                            string mobile1 = itm.MobilePhone?.Trim(); // safer than Convert.ToString

                            var existingContact = _crmDbcontext.Contacts
                                .FirstOrDefault(x => x.AccountId == accGuid && x.MobilePhone == mobile1);

                            if (existingContact != null)
                            {
                                existingContact.FullName = itm.FullName;
                                existingContact.Department = itm.Department;
                                existingContact.Designation = itm.Designation;
                                existingContact.Email = itm.Email;
                                existingContact.MobilePhone = mobile1;
                                existingContact.IsActive = Convert.ToString(itm.StatusId) == "1" ? true : false;
                            }
                            else
                            {
                                var contact = new BSuit.SalesCRM.Entities.Contact
                                {
                                    AccountId = accGuid,
                                    FullName = itm.FullName,
                                    Department = itm.Department,
                                    Designation = itm.Designation,
                                    Email = itm.Email,
                                    MobilePhone = mobile1,
                                    IsActive = Convert.ToString(itm.StatusId) == "1" ? true : false,
                                };

                                _crmDbcontext.Contacts.Add(contact);
                            }
                        }
                    }
                    // ✅ IMPORTANT: Save once outside loop
                    _crmDbcontext.SaveChanges();

                    // 4. Update fields
                    string accountName = frm["AccountName"].FirstOrDefault();
                    string companyName = frm["CompanyName"].FirstOrDefault();
                    string title = "Account Edited";
                    string message = $"Account Name: {accountName}, Company Name: {companyName}";
                    string createdBy = Convert.ToString(_iUserContext.GUID_USERID);
                    List<string> userIds = new List<string>();
                    userIds.Add(Convert.ToString(accountDto.OwnerId));
                    _iNotificationService.CreateForUsers(title, message, userIds, createdBy);

                }
                return Json(new
                {
                    success = true,
                    message = "Account updated successfully!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,//
                    message = ex.Message
                });
            }
        }

        public IActionResult ViewAccount(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                return BadRequest();

            if (!Guid.TryParse(accountId, out Guid accGuid))
                return BadRequest("Invalid AccountId");

            try
            {
                var account = _crmDbcontext.Accounts
                    .FirstOrDefault(x => x.AccountId == accGuid);

                if (account == null)
                    return NotFound();

                var model = new AccountVM
                {
                    AccountId = account.AccountId,
                    AccountName = account.AccountName,
                    CompanyName = account.CompanyName,
                    CompanyRanking = account.CompanyRanking,

                    LeadId = account.LeadId ?? Guid.Empty,
                    CustomerId = account.CustomerId,

                    IndustryId = account.IndustryId,
                    GenderId = account.GenderId,
                    JobTitleId = account.JobTitleId,
                    CompanySizeId = account.CompanySizeId,
                    CustomerTypeID = account.CustomerTypeID,

                    CompanyRevenue = account.CompanyRevenue,
                    CustomerBackground = account.CustomerBackground,


                    BusinessEmail = account.BusinessEmail,
                    BusinessPhone = account.BusinessPhone,
                    PersonalEmail = account.PersonalEmail,
                    PersonalPhone = account.PersonalPhone,
                    Website = account.Website,

                    CountryId = account.CountryId,
                    State = account.State,
                    City = account.City,
                    Address = account.Address,
                    ZipCode = account.ZipCode,

                    OwnerId = account.OwnerId,

                    HighPotentialStatus = account.HighPotentiaStatus.HasValue ? (account.HighPotentiaStatus.Value ? "Yes" : "No") : "",
                    LinkedInURL = account.LinkedInURL,
                    FacebookURL = account.FacebookURL,
                    TwitterURL = account.TwitterURL,
                    SkypeID = account.SkypeID,
                    InstantMessengerId = account.InstantMessengerId,


                };

                var emailMessages = _crmDbcontext.EmailMessages.Where(m => m.ParentEntityId == accGuid).OrderByDescending(m => m.SentDate).ToList();
                var emailIds = emailMessages.Select(e => e.EmailId).ToList();
                var emailRecipients = _crmDbcontext.EmailRecipients.Where(r => emailIds.Contains(r.EmailId)).ToList();
                var clientCommunication = emailMessages.Select(email => new ClientCommunication
                {
                    EmailId = email.EmailId,
                    Subject = email.Subject,

                    BodyPreview = !string.IsNullOrEmpty(email.BodyHtml) ? (email.BodyHtml.Length > 100 ? email.BodyHtml.Substring(0, 100) + "..." : email.BodyHtml) : "",

                    SentDate = email.SentDate,
                    Status = email.Status,

                    ToEmails = string.Join(", ", emailRecipients.Where(r => r.EmailId == email.EmailId && r.Type == "To").Select(r => r.Address)),

                    CcEmails = string.Join(", ", emailRecipients.Where(r => r.EmailId == email.EmailId && r.Type == "CC").Select(r => r.Address)),

                    BccEmails = string.Join(", ", emailRecipients.Where(r => r.EmailId == email.EmailId && r.Type == "BCC").Select(r => r.Address))

                }).ToList();
                // Email Intial Account Communication
                model.EmailHistory = clientCommunication;

                var users = _identityDbContext.Users.ToList();
                // Document
                model.documentsVM = (from CT in _crmDbcontext.DocumentRepositories
                                         //  join U in users on CT.CreatedBy equals U.UserId into uJoin
                                         // from U in uJoin.DefaultIfEmpty()
                                     where CT.ReferenceId == accGuid
                                     orderby CT.CreatedDate descending
                                     select new DocumentsVM { UploadedOn = CT.CreatedDate, FileName = CT.FileName, FilePath = CT.FilePath, FullName = "" }).ToList();

                model.ServicesInfo = _crmDbcontext.Services.OrderBy(x => x.DisplayOrder)
               .Select(x => new SelectListItem { Value = x.ServiceId.ToString(), Text = x.ServiceName }).ToList();
                var referral = _crmDbcontext.Referrals.Where(x => x.ReferrerAccountId == accGuid).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                if (referral != null)
                {
                    model.ReferredName = referral.ReferredName;
                    model.ReferredPhone = referral.ReferredPhone;
                    model.ReferredEmail = referral.ReferredEmail;
                }

                model.referralInfo = (from CT in _crmDbcontext.Referrals

                                      where CT.ReferrerAccountId == accGuid
                                      orderby CT.CreatedDate descending
                                      select new ReferralVM
                                      {
                                          ReferredName = CT.ReferredName,
                                          ReferredEmail = CT.ReferredEmail,
                                          ReferredPhone = CT.ReferredPhone,
                                      }).ToList();


                model.contactInfo = (from CT in _crmDbcontext.Contacts
                                         // join S in _crmDbcontext.SalutationMasters on CT.SalutationId equals S.SalutationId
                                     where CT.AccountId == accGuid && CT.IsActive == true
                                     orderby CT.CreatedOn descending
                                     select new ContactInfo
                                     {
                                         FullName = CT.FullName,

                                         Department = CT.Department,

                                         Designation = CT.Designation,
                                         Email = CT.Email,
                                         MobilePhone = CT.MobilePhone
                                     }).ToList();

                model.opportunities = (from O in _crmDbcontext.Opportunities
                                       join OS in _crmDbcontext.OpportunityServices on O.OpportunityId equals OS.OpportunityId
                                       join S in _crmDbcontext.Services on OS.ServiceId equals S.ServiceId
                                       // LEFT JOIN OpportunityStages
                                       join WF in _crmDbcontext.ApprovalTransactions on OS.WorkflowId equals WF.WorkflowId into wfGroup
                                       from WF in wfGroup.DefaultIfEmpty()

                                       join ST in _crmDbcontext.OpportunityStages on OS.OpportunityStageId equals ST.StageId into stageGroup
                                       from ST in stageGroup.DefaultIfEmpty()

                                       where OS.AccountId == model.AccountId
                                       orderby O.CreatedOn descending
                                       select new ScopeOpportunity
                                       {
                                           Service = S.ServiceName,
                                           OpportunityStatus = ST.StageName,
                                           OpportunityName = O.OpportunityName,
                                           RequestedOn = O.CreatedOn.Value,
                                           SubmittedOn = OS.CreatedOn,
                                           ApprovedOn = WF.ActionDate,
                                       }).ToList();



                model.projectViews = (from P in _crmDbcontext.Projects
                                      where P.AccountId == model.AccountId
                                      orderby P.CreatedOn descending
                                      select new ProjectViewModel
                                      {
                                          ProjectId = P.ProjectId,
                                          ProjectName = P.ProjectName,
                                          Description = P.Description,
                                          ProjectCode = P.ProjectCode,
                                          ProjectHours = P.ProjectHours.HasValue ? (int)P.ProjectHours.Value + ":" + ((P.ProjectHours.Value - (int)P.ProjectHours.Value) * 60).ToString("00") : "00:00",
                                          BalanceHours = P.BalanceHours.HasValue ? (int)P.BalanceHours.Value + ":" + ((P.BalanceHours.Value - (int)P.BalanceHours.Value) * 60).ToString("00") : "00:00",
                                          TransferHours = P.TransferHours.HasValue ? (int)P.TransferHours.Value + ":" + ((P.TransferHours.Value - (int)P.TransferHours.Value) * 60).ToString("00") : "00:00",
                                          Notes = P.Notes,
                                          StartDate = P.StartDate.Value,
                                          EndDate = P.EndDate.Value
                                      }).ToList();






                // 🔽 Load related data separately (SAFE way)

                model.TenantName = _coreDbContext.Tenants
                    .Where(x => x.Id == account.TenantId)
                    .Select(x => x.Name)
                    .FirstOrDefault() ?? "";

                model.OwnerName = _identityDbContext.Users
                    .Where(x => x.UserId == account.OwnerId)
                    .Select(x => x.FullName)
                    .FirstOrDefault() ?? "";

                model.GenderName = _crmDbcontext.GenderMasters
                    .Where(x => x.GenderId == account.GenderId)
                    .Select(x => x.GenderName)
                    .FirstOrDefault() ?? "";

                model.CountryName = _crmDbcontext.Countries
                    .Where(x => x.CountryId == account.CountryId)
                    .Select(x => x.CountryName)
                    .FirstOrDefault() ?? "";

                model.IndustryName = _crmDbcontext.Industries
                    .Where(x => x.IndustryId == account.IndustryId)
                    .Select(x => x.IndustryName)
                    .FirstOrDefault() ?? "";

                model.JobTitleName = _crmDbcontext.JobTitles
                    .Where(x => x.JobTitleId == account.JobTitleId)
                    .Select(x => x.JobTitleName)
                    .FirstOrDefault() ?? "";

                model.SizeName = _crmDbcontext.CompanySizes
                    .Where(x => x.LeadCompanySizeId == account.CompanySizeId)
                    .Select(x => x.SizeName)
                    .FirstOrDefault() ?? "";

                model.CustomerTypeName = _crmDbcontext.CustomerTypes
                    .Where(x => x.CustomerTypeId == account.CustomerTypeID)
                    .Select(x => x.CustomerTypeName)
                    .FirstOrDefault() ?? "";

                model.AccountSourceName = _crmDbcontext.AccountSources
                   .Where(x => x.AccountSourceId == account.AccountSourceId)
                   .Select(x => x.AccountSourceName)
                   .FirstOrDefault() ?? "";


                model.CustomerCategory = _crmDbcontext.CustomerCategories
                  .Where(x => x.CustomerCategoryId == account.CustomerCategoryId)
                  .Select(x => x.CustomerCategory1)
                  .FirstOrDefault() ?? "";

                model.AccountCategorization = _crmDbcontext.AccountCategorizations
                  .Where(x => x.AccountCategorizationId == account.AccountCategorizationId)
                  .Select(x => x.AccountCategorization1).FirstOrDefault() ?? "";

                model.RegionName = _crmDbcontext.Regions
                  .Where(x => x.RegionId == account.RegionId)
                  .Select(x => x.RegionName).FirstOrDefault() ?? "";

                model.SourceName = _crmDbcontext.LeadSources
                  .Where(x => x.LeadSourceId == account.LeadSourceId)
                  .Select(x => x.SourceName).FirstOrDefault() ?? "";







                return View(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public IActionResult AccountDetails(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                return BadRequest();

            if (!Guid.TryParse(accountId, out Guid accGuid))
                return BadRequest("Invalid AccountId");

            try
            {
                var account = _crmDbcontext.Accounts
                    .FirstOrDefault(x => x.AccountId == accGuid);

                if (account == null)
                    return NotFound();

                var model = new AccountVM
                {
                    AccountId = account.AccountId,
                    AccountName = account.AccountName,
                    CompanyName = account.CompanyName,
                    CompanyRanking = account.CompanyRanking,

                    LeadId = account.LeadId ?? Guid.Empty,
                    CustomerId = account.CustomerId,

                    IndustryId = account.IndustryId,
                    GenderId = account.GenderId,
                    JobTitleId = account.JobTitleId,
                    CompanySizeId = account.CompanySizeId,
                    CustomerTypeID = account.CustomerTypeID,

                    CompanyRevenue = account.CompanyRevenue,
                    CustomerBackground = account.CustomerBackground,


                    BusinessEmail = account.BusinessEmail,
                    BusinessPhone = account.BusinessPhone,
                    PersonalEmail = account.PersonalEmail,
                    PersonalPhone = account.PersonalPhone,
                    Website = account.Website,

                    CountryId = account.CountryId,
                    State = account.State,
                    City = account.City,
                    Address = account.Address,
                    ZipCode = account.ZipCode,

                    OwnerId = account.OwnerId,

                    HighPotentialStatus = account.HighPotentiaStatus.HasValue ? (account.HighPotentiaStatus.Value ? "Yes" : "No") : "",
                    LinkedInURL = account.LinkedInURL,
                    FacebookURL = account.FacebookURL,
                    TwitterURL = account.TwitterURL,
                    SkypeID = account.SkypeID,
                    InstantMessengerId = account.InstantMessengerId,
                };
                // Referrals  List
                model.referralInfo = (from CT in _crmDbcontext.Referrals
                                      where CT.ReferrerAccountId == accGuid
                                      orderby CT.CreatedDate descending
                                      select new ReferralVM
                                      {
                                          ReferredEmail = CT.ReferredEmail,
                                          ReferredPhone = CT.ReferredPhone,
                                          ReferredName = CT.ReferredName,
                                          StatusId = CT.Status == true ? "<span style='color:green;'>Active</span>" : "<span style='color:red;'>Inactive</span>",
                                      }).ToList();
                // Contact List
                model.contactInfo = (from CT in _crmDbcontext.Contacts
                                         // join S in _crmDbcontext.SalutationMasters on CT.SalutationId equals S.SalutationId
                                     where CT.AccountId == accGuid && CT.IsActive == true
                                     orderby CT.CreatedOn descending
                                     select new ContactInfo
                                     {
                                         FullName = CT.FullName,
                                         Department = CT.Department,
                                         Designation = CT.Designation,
                                         Email = CT.Email,
                                         MobilePhone = CT.MobilePhone,
                                         // StatusId = CT.IsActive == true ? "Active" : "Inactive",
                                         StatusId = CT.IsActive == true ? "<span style='color:green;'>Active</span>" : "<span style='color:red;'>Inactive</span>",
                                     }).ToList();

                model.opportunities = (from O in _crmDbcontext.Opportunities

                                       join OS in _crmDbcontext.OpportunityServices on O.OpportunityId equals OS.OpportunityId

                                       join S in _crmDbcontext.Services on OS.ServiceId equals S.ServiceId
                                       join ST in _crmDbcontext.OpportunityStages on OS.OpportunityStageId equals ST.StageId
                                       where OS.LeadId == model.LeadId
                                       orderby O.CreatedOn descending
                                       select new ScopeOpportunity
                                       {
                                           Service = S.ServiceName,
                                           OpportunityStatus = ST.StageName,

                                       }).Take(2).ToList();



                model.projectViews = (from P in _crmDbcontext.Projects
                                      where P.AccountId == model.AccountId
                                      orderby P.CreatedOn descending
                                      select new ProjectViewModel
                                      {
                                          ProjectName = P.ProjectName,
                                          ProjectCode = P.ProjectCode,
                                          StartDate = P.StartDate.Value,
                                          EndDate = P.EndDate.Value
                                      })

                          .ToList();





                // 🔽 Load related data separately (SAFE way)

                model.TenantName = _coreDbContext.Tenants
                    .Where(x => x.Id == account.TenantId)
                    .Select(x => x.Name)
                    .FirstOrDefault() ?? "";

                model.OwnerName = _identityDbContext.Users
                    .Where(x => x.UserId == account.OwnerId)
                    .Select(x => x.FullName)
                    .FirstOrDefault() ?? "";

                model.GenderName = _crmDbcontext.GenderMasters
                    .Where(x => x.GenderId == account.GenderId)
                    .Select(x => x.GenderName)
                    .FirstOrDefault() ?? "";

                model.CountryName = _crmDbcontext.Countries
                    .Where(x => x.CountryId == account.CountryId)
                    .Select(x => x.CountryName)
                    .FirstOrDefault() ?? "";

                model.IndustryName = _crmDbcontext.Industries
                    .Where(x => x.IndustryId == account.IndustryId)
                    .Select(x => x.IndustryName)
                    .FirstOrDefault() ?? "";

                model.JobTitleName = _crmDbcontext.JobTitles
                    .Where(x => x.JobTitleId == account.JobTitleId)
                    .Select(x => x.JobTitleName)
                    .FirstOrDefault() ?? "";

                model.SizeName = _crmDbcontext.CompanySizes
                    .Where(x => x.LeadCompanySizeId == account.CompanySizeId)
                    .Select(x => x.SizeName)
                    .FirstOrDefault() ?? "";

                model.CustomerTypeName = _crmDbcontext.CustomerTypes
                    .Where(x => x.CustomerTypeId == account.CustomerTypeID)
                    .Select(x => x.CustomerTypeName)
                    .FirstOrDefault() ?? "";

                model.AccountSourceName = _crmDbcontext.AccountSources
                   .Where(x => x.AccountSourceId == account.AccountSourceId)
                   .Select(x => x.AccountSourceName)
                   .FirstOrDefault() ?? "";


                model.CustomerCategory = _crmDbcontext.CustomerCategories
                  .Where(x => x.CustomerCategoryId == account.CustomerCategoryId)
                  .Select(x => x.CustomerCategory1)
                  .FirstOrDefault() ?? "";

                model.AccountCategorization = _crmDbcontext.AccountCategorizations
                  .Where(x => x.AccountCategorizationId == account.AccountCategorizationId)
                  .Select(x => x.AccountCategorization1).FirstOrDefault() ?? "";

                model.RegionName = _crmDbcontext.Regions
                  .Where(x => x.RegionId == account.RegionId)
                  .Select(x => x.RegionName).FirstOrDefault() ?? "";

                model.SourceName = _crmDbcontext.LeadSources
                  .Where(x => x.LeadSourceId == account.LeadSourceId)
                  .Select(x => x.SourceName).FirstOrDefault() ?? "";







                return View(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public string GenerateCustomerID(string accountName, string companyName)
        {
            if (string.IsNullOrWhiteSpace(accountName))
                return "";

            // Split Account Name
            var nameParts = accountName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string prefix = "";

            // First letter of first name
            if (nameParts.Length > 0)
                prefix += char.ToUpper(nameParts[0][0]);

            // First letter of second name (if exists)
            if (nameParts.Length > 1)
                prefix += char.ToUpper(nameParts[1][0]);

            // First letter of CompanyName
            if (!string.IsNullOrWhiteSpace(companyName))
                prefix += char.ToUpper(companyName.Trim()[0]);

            // 👉 Get last number from DB (VERY IMPORTANT)
            int lastNumber = GetLastCustomerNumber(prefix);

            // Increment
            int newNumber = lastNumber + 1;

            // Format number → 0001
            string numberPart = newNumber.ToString("D4");

            return prefix + numberPart;
        }
        private int GetLastCustomerNumber(string prefix)
        {
            var lastRecord = _crmDbcontext.Accounts
                .Where(x => x.CustomerId.StartsWith(prefix))
                .OrderByDescending(x => x.CustomerId)
                .Select(x => x.CustomerId)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(lastRecord))
                return 0;

            // Extract numeric part (last 4 digits)
            string numberPart = lastRecord.Substring(prefix.Length);

            return int.TryParse(numberPart, out int num) ? num : 0;
        }

        [HttpPost]
        public JsonResult DeleteContact(string mobile)
        {
            var contact = _crmDbcontext.Contacts
                .FirstOrDefault(x => x.MobilePhone == mobile);

            if (contact == null)
            {
                return Json(new { success = false, message = "Mobile number not found!" });
            }

            _crmDbcontext.Contacts.Remove(contact);
            _crmDbcontext.SaveChanges();

            return Json(new { success = true });
        }


        [HttpPost]
        public JsonResult DeleteReferral(string mobile)
        {
            try
            {
                // find record
                var data = _crmDbcontext.Referrals
                                   .FirstOrDefault(x => x.ReferredPhone == mobile);

                if (data != null)
                {
                    _crmDbcontext.Referrals.Remove(data);
                    _crmDbcontext.SaveChanges();
                }

                return Json(new
                {
                    success = true,
                    message = "Deleted successfully"
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
        public async Task<IActionResult> CreateOpportunity(IFormCollection frm)
        {
            string msg;



            if (!Guid.TryParse(frm["ServiceId"], out Guid serviceId))
                return Json(new { success = false, message = "Invalid ServiceId" });

            if (!Guid.TryParse(frm["ancAccountId"], out Guid ancAccountId))
                return Json(new { success = false, message = "Invalid AccountId" });

            var existingAccDetails = await _crmDbcontext.Accounts
                .FirstOrDefaultAsync(e => e.AccountId == ancAccountId);

            if (existingAccDetails == null)
                return Json(new { success = false, message = "Account not found" });

            //if (!existingAccDetails.TenantId.HasValue)
            //    return Json(new { success = false, message = "TenantId is missing for this account" });

            Guid tenantId = _iUserContext.TenantId.Value;



            var StageId = _crmDbcontext.OpportunityStages
               .Where(o => o.StageName.Contains("Interim"))
               .Select(o => o.StageId)
               .FirstOrDefault();


            var opp = new Opportunity
            {
                OpportunityId = Guid.NewGuid(),
                OpportunityName = frm["OpportunityName"].ToString(),
                CreatedOn = DateTime.Now,
                TenantId = tenantId
            };

            await _crmDbcontext.Opportunities.AddAsync(opp);

            var serviceMapping = new OpportunityService
            {
                OpportunityServiceId = Guid.NewGuid(),
                AccountId = ancAccountId,
                OpportunityId = opp.OpportunityId,
                OpportunityStageId = StageId,
                ServiceId = serviceId,
                CreatedOn = DateTime.Now,
                TenantId = tenantId,
                CreatedBy = Guid.Parse(_iUserContext.UserId),
            };
            await _crmDbcontext.OpportunityServices.AddAsync(serviceMapping);
            await _crmDbcontext.SaveChangesAsync();
            msg = "Opportunity details added successfully.";
            return Json(new { success = true, message = msg, opportunityId = opp.OpportunityId });
        }

        [HttpGet]
        public async Task<IActionResult> GetOpportunityList(string accountId)
        {
            // Validate Guid
            if (!Guid.TryParse(accountId, out Guid parsedAccountId))
            {
                return BadRequest("Invalid Account Id");
            }

            var opportunities = await (from O in _crmDbcontext.Opportunities
                                       join OS in _crmDbcontext.OpportunityServices on O.OpportunityId equals OS.OpportunityId
                                       join S in _crmDbcontext.Services on OS.ServiceId equals S.ServiceId
                                       // LEFT JOIN OpportunityStages
                                       join WF in _crmDbcontext.ApprovalTransactions on OS.WorkflowId equals WF.WorkflowId into wfGroup
                                       from WF in wfGroup.DefaultIfEmpty()

                                       join ST in _crmDbcontext.OpportunityStages on OS.OpportunityStageId equals ST.StageId into stageGroup
                                       from ST in stageGroup.DefaultIfEmpty()

                                       where OS.AccountId == parsedAccountId
                                       orderby O.CreatedOn descending

                                       select new ScopeOpportunity
                                       {
                                           Service = S.ServiceName,
                                           OpportunityStatus = ST.StageName,
                                           OpportunityName = O.OpportunityName,
                                           RequestedOn = O.CreatedOn.Value,
                                           SubmittedOn = OS.CreatedOn,
                                           ApprovedOn = WF.ActionDate,
                                       }





            ).ToListAsync();

            return Json(opportunities);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateAccountOwner(IFormCollection frm)
        {
            if (!Guid.TryParse(frm["AccountOwnerId"], out Guid accountOwnerId))
            {
                return Json(new { success = false, message = "Invalid AccountOwnerId" });
            }

            var accountIds = frm["AccountIds"].ToList();

            foreach (var id in accountIds)
            {
                if (!Guid.TryParse(id, out Guid accountId))
                    continue;

                var account = await _crmDbcontext.Accounts
                    .FirstOrDefaultAsync(x => x.AccountId == accountId);

                if (account != null)
                {
                    account.OwnerId = accountOwnerId;   // ✅ FIXED PROPERTY
                    account.ModifiedBy = accountOwnerId;
                    account.ModifiedOn = DateTime.Now;
                }
            }
            await _crmDbcontext.SaveChangesAsync();
            string tittle = "Account Transfer";
            string _message = "Account tranfer done";
            string createdBy = Convert.ToString(_iUserContext.GUID_USERID);
            List<string> lst = new List<string>();
            lst.Add(Convert.ToString(accountOwnerId));
            _iNotificationService.CreateForUsers(tittle, _message, lst, createdBy);

            return Json(new
            {
                success = true,
                message = "Account owner updated successfully"
            });
        }
        [HttpGet]

        public JsonResult GetAccounOwners()
        {
            var result = _identityDbContext.Users
                .Where(x => x.IsActive == true)
                .OrderBy(x => x.FullName)
                .Select(x => new
                {
                    name = x.FullName,

                    id = x.UserId
                })
                .ToList();

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> UploadLeadDocument(Guid accountId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file selected");
                }

                // File size
                long fileSize = file.Length;

                // Root folder
                string uploadsRoot = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "accounts");

                // Account folder
                string accountFolder = Path.Combine(uploadsRoot, accountId.ToString());

                // Create folders if not exists
                if (!Directory.Exists(accountFolder))
                {
                    Directory.CreateDirectory(accountFolder);
                }

                // Original filename
                string originalFileName =
                    Path.GetFileNameWithoutExtension(file.FileName);

                // Extension
                string extension = Path.GetExtension(file.FileName);

                // Document type
                string documentType = extension.Replace(".", "").ToUpper();

                // Timestamp filename
                string fileName =
                    $"{originalFileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";

                // Final path
                string filePath = Path.Combine(accountFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Relative path
                string relativePath =
                    $"/uploads/accounts/{accountId}/{fileName}";

                // Content type
                string contentType = "application/octet-stream";

                var provider =
                    new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();

                if (provider.TryGetContentType(filePath, out var mime))
                {
                    contentType = mime;
                }

                // Save DB
                _crmDbcontext.DocumentRepositories.Add(new DocumentRepository
                {
                    AttachmentId = Guid.NewGuid(),
                    ReferenceId = accountId,
                    ReferenceType = "ACCOUNT",
                    OriginalFileName = originalFileName,
                    FileName = fileName,
                    FilePath = relativePath,
                    CreatedDate = DateTime.UtcNow,
                    FileExtension = extension,
                    FileSize = fileSize,
                    ContentType = contentType,
                    DocumentType = documentType,
                    //  ModifiedBy = _iUserContext.GUID_USERID,
                });

                await _crmDbcontext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    filePath = relativePath,
                    fileName,
                    fileSize,
                    contentType,
                    documentType
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> SendEmail()
        {
            Guid emailId = Guid.NewGuid();

            try
            {
                var to = Request.Form["To"];
                var cc = Request.Form["CC"];
                var bcc = Request.Form["BCC"];
                var subject = Request.Form["Subject"];
                var body = Request.Form["BodyHtml"];
                var parentType = Request.Form["ParentEntityType"];
                var parentIdStr = Request.Form["ParentEntityId"];

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

                _crmDbcontext.EmailMessages.Add(email);

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

                            _crmDbcontext.EmailAttachments.Add(new EmailAttachment
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
                await _crmDbcontext.SaveChangesAsync();

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

                await _crmDbcontext.SaveChangesAsync();

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
                _crmDbcontext.EmailRecipients.Add(new EmailRecipient
                {
                    RecipientId = Guid.NewGuid(),
                    EmailId = emailId,
                    Address = email.Trim(),
                    Type = type
                });
            }
        }



        public async Task<IActionResult> LoadLeadDetails(Guid leadid)
        {
            Guid leadId = Guid.Parse("7032CAC6-B0D2-4D09-B19D-0A812ED45549");

            var lead = await _leadService.GetLeadDetails(leadId);

            //return PartialView("~/Views/Lead/_LeadDetails.cshtml", lead);
            return PartialView("~/Areas/SalesCRM/Views/Lead/_LeadDetails.cshtml", lead);

            //return PartialView("_LeadDetails", lead);
        }


        public async Task<IActionResult> LoadNDADetails(Guid accountId)
        {
            var account = await _crmDbcontext.Accounts
                .FirstOrDefaultAsync(N => N.AccountId == accountId);

            Guid? leadId = accountId;

            if (account != null && account.LeadId.HasValue)
            {
                leadId = account.LeadId.Value;
            }

            var model = await _crmDbcontext.NDASignatures
                .Where(N => N.LeadId == leadId || N.AccountId == accountId)
                .Select(N => new NDASignatureVM
                {
                    NDAID = N.NDAID,
                    LeadId = N.LeadId,
                    AccountId = N.AccountId,
                    NDAVersionNumber = N.NDAVersionNumber,
                    NDANumber = N.NDANumber,
                    AcceptNonDisclosureAgreement = N.AcceptNonDisclosureAgreement,
                    AcceptScannedDocumentsNDA = N.AcceptScannedDocumentsNDA,
                    NDAFormat = N.NDAFormat,
                    NDASpecialClauseForOrg = N.NDASpecialClauseForOrg,
                    NDASpecialClauseForInternal = N.NDASpecialClauseForInternal,
                    NDAStartDate = N.NDAStartDate,
                    NDAEndDate = N.NDAEndDate,
                    NDARenewalDate = N.NDARenewalDate,
                    NDACustomerName = N.NDACustomerName,


                    NDALink = N.NDALink,


                    ExecutedDate = N.ExecutedDate,

                    NDAStatus =
                        N.NDAEndDate.HasValue &&
                        N.NDAEndDate.Value < DateOnly.FromDateTime(DateTime.Now)
                            ? "Expired"
                            : "Active"
                })
                .FirstOrDefaultAsync();

            // If no record found → create default model
            if (model == null)
            {
                model = new NDASignatureVM
                {
                    //LeadId = leadId,
                    NDALink =
                        $"{Request.Scheme}://{Request.Host}/SalesCRM/Lead/NDA" +
                        $"?leadId={leadId}" +
                        $"&requestedDate={Uri.EscapeDataString(DateTime.Now.ToString("dd-MMM-yyyy"))}"
                };
            }

            ///var lead = await _leadService.GetLeadDetails(model);

            return PartialView("~/Areas/SalesCRM/Views/Account/_ndaDetails.cshtml", model);
        }



        [HttpPost]
        public async Task<IActionResult> SaveNDA([FromBody] NDASignatureVM model)
        {
            if (model == null)

                return BadRequest("Invalid data");

            var entity = new NDASignature
            {
                NDAID = Guid.NewGuid(),
                LeadId = model.LeadId,
                AccountId = model.AccountId,
                AcceptNonDisclosureAgreement = model.AcceptNonDisclosureAgreement,
                AcceptScannedDocumentsNDA = model.AcceptScannedDocumentsNDA,
                NDAStartDate = model.NDAStartDate,
                NDAEndDate = model.NDAEndDate,
                NDARenewalDate = model.NDARenewalDate,
                NDACustomerName = model.NDACustomerName,
                NDAVersionNumber = model.NDAVersionNumber,
                NDANumber = model.NDANumber,
                NDAFormat = model.NDAFormat,
                NDASpecialClauseForOrg = model.NDASpecialClauseForOrg,
                NDASpecialClauseForInternal = model.NDASpecialClauseForInternal,
                ExecutedDate = model.ExecutedDate
            };

            _crmDbcontext.NDASignatures.Add(entity);
            await _crmDbcontext.SaveChangesAsync();

            return Json(new { success = true, message = "Saved successfully" });
        }


    }



}