using BSuit.API.Areas.Admin.Models;
using BSuit.Core.Data;
using BSuit.Core.Entities;
using BSuit.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BSuit.API.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuditLogController : Controller
    {
        private readonly CoreDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuditLogController(
            CoreDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(
      Guid? tenantId,
      string tableName,
      string actionType,
      string userSearch,
      DateTime? fromDate,
      DateTime? toDate,
      int page = 1)
        {
            int pageSize = 20;

            var vm = new AuditLogVM();
            var user = await _userManager.GetUserAsync(User);

            bool isSuperAdmin = await _userManager
                .IsInRoleAsync(user, "SuperAdmin");

            //---------------------------------------
            // Tenant filtering
            //---------------------------------------
            if (isSuperAdmin)
            {
                vm.Tenants = await _context.Tenants
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    }).ToListAsync();
            }
            else
            {
                tenantId = user.TenantId;
            }

            var query = _context.AuditLogs.AsQueryable();

            if (tenantId.HasValue)
                query = query.Where(x => x.TenantId == tenantId);

            if (!string.IsNullOrEmpty(tableName))
                query = query.Where(x => x.TableName == tableName);

            if (!string.IsNullOrEmpty(actionType))
                query = query.Where(x => x.Action == actionType);

            if (!string.IsNullOrEmpty(userSearch))
                query = query.Where(x => x.UserId.Contains(userSearch));

            if (fromDate.HasValue)
                query = query.Where(x => x.ChangedOn >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.ChangedOn <= toDate.Value);

            var totalRecords = await query.CountAsync();

            vm.Logs = await query
                .OrderByDescending(x => x.ChangedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var item in vm.Logs)
            {
                item.UserId = _userManager.FindByIdAsync(item.UserId)?.Result?.UserName;
            }
            vm.CurrentPage = page;
            vm.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            vm.UserSearch = userSearch;
            vm.FromDate = fromDate;
            vm.ToDate = toDate;

            return View(vm);
        }


        public async Task<IActionResult> ExportToExcel(
    Guid? tenantId,
    string tableName,
    string actionType)
        {
            var logs = await _context.AuditLogs
                .Where(x =>
                    (!tenantId.HasValue || x.TenantId == tenantId) &&
                    (string.IsNullOrEmpty(tableName) || x.TableName == tableName) &&
                    (string.IsNullOrEmpty(actionType) || x.Action == actionType))
                .ToListAsync();

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("AuditLogs");

            worksheet.Cell(1, 1).Value = "Table";
            worksheet.Cell(1, 2).Value = "Action";
            worksheet.Cell(1, 3).Value = "User";
            worksheet.Cell(1, 4).Value = "Changed On";
            worksheet.Cell(1, 5).Value = "Remarks";

            int row = 2;

            foreach (var item in logs)
            {
                worksheet.Cell(row, 1).Value = item.TableName;
                worksheet.Cell(row, 2).Value = item.Action;
                worksheet.Cell(row, 3).Value = item.UserId;
                worksheet.Cell(row, 4).Value = item.ChangedOn;
                worksheet.Cell(row, 5).Value = item.Remarks;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "AuditLogs.xlsx");
        }


    }
}