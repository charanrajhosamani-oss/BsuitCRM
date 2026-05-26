// Areas/Admin/Controllers/AdminUsersController.cs
using BSuit.API.Areas.Admin.Controllers.Base;
using BSuit.API.Areas.Admin.Models;
using BSuit.API.Infrastructure.Constants;
using BSuit.Identity;
using BSuit.Identity.Data;
using BSuit.Identity.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace BSuit.API.Areas.Admin.Controllers
{
    using BSuit.Core.Data;

    /*
    Requirement Summary
    If logged-in user is SuperAdmin → show all tenants in dropdown.
    If normal admin/user → auto-bind only their tenant.
    On tenant selection:
    Total Active Users count
    Total Inactive Users count
    Duplicate Records count
    Today created records count
    Last 7 days count
    Last 1 month count
    Last 1 year count
    Display:
    Summary cards/table
    Chart (bar/doughnut/line)
     */

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Graph.Models;
    using System.Data;

    [Area("Admin")]
    public class AnalyticsController : Controller
    {
        private readonly CoreDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AnalyticsController(
            CoreDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(
            Guid? tenantId,
            string tableName)
        {
            var vm = new AnalyticsVM();

            var user = await _userManager.GetUserAsync(User);
            bool isSuperAdmin = await _userManager
                .IsInRoleAsync(user, "SuperAdmin");

            //---------------------------------------
            // Load tenants dropdown
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

            vm.SelectedTenantId = tenantId.GetValueOrDefault();

            //---------------------------------------
            // Load database tables dynamically
            //---------------------------------------
            var connString = _configuration
                .GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();

                var cmd = new SqlCommand(@"
                            SELECT 
                                TABLE_SCHEMA,
                                TABLE_NAME
                            FROM INFORMATION_SCHEMA.TABLES
                            WHERE TABLE_TYPE='BASE TABLE'
                            ORDER BY TABLE_SCHEMA, TABLE_NAME", conn);

                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    string schemaName = reader["TABLE_SCHEMA"].ToString();
                    string table = reader["TABLE_NAME"].ToString();

                    vm.Tables.Add(new SelectListItem
                    {
                        Value = $"{schemaName}.{table}",   // store full schema.table
                        Text = $"{schemaName}.{table}"
                    });
                }
            }

            vm.SelectedTableName = tableName;

            //---------------------------------------
            // Fetch selected table analytics
            //---------------------------------------

            if (!string.IsNullOrEmpty(tableName))
            {
                // Split schema + table
                var parts = tableName.Split('.');

                if (parts.Length != 2)
                {
                    TempData["Error"] = "Invalid table selection.";
                    return View(vm);
                }

                string schemaName = parts[0];
                string actualTable = parts[1];

                string safeTableName = $"[{schemaName}].[{actualTable}]";
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();

                    string tenantFilter = tenantId.HasValue
                        ? $" AND TenantId={tenantId.Value}"
                        : "";

                    // Active count
                    vm.ActiveCount = await ExecuteScalarInt(conn, $@"
                    SELECT COUNT(*)
                    FROM {safeTableName}
                    WHERE IsActive=1 {tenantFilter}");

                    // Inactive count
                    vm.InactiveCount = await ExecuteScalarInt(conn, $@"
                    SELECT COUNT(*)
                    FROM {safeTableName}
                    WHERE IsActive=0 {tenantFilter}");

                    // Today count
                    vm.TodayCount = await ExecuteScalarInt(conn, $@"
                    SELECT COUNT(*)
                    FROM {safeTableName}
                    WHERE CAST(CreatedOn AS DATE)=CAST(GETDATE() AS DATE)
                    {tenantFilter}");

                    // Last 7 days
                    vm.Last7DaysCount = await ExecuteScalarInt(conn, $@"
                    SELECT COUNT(*)
                    FROM {safeTableName}
                    WHERE CreatedOn >= DATEADD(DAY,-7,GETDATE())
                    {tenantFilter}");

                    // Last 1 month
                    vm.Last1MonthCount = await ExecuteScalarInt(conn, $@"
                    SELECT COUNT(*)
                    FROM {safeTableName}
                    WHERE CreatedOn >= DATEADD(MONTH,-1,GETDATE())
                    {tenantFilter}");

                    // Last 1 year
                    vm.Last1YearCount = await ExecuteScalarInt(conn, $@"
                    SELECT COUNT(*)
                    FROM {safeTableName}
                    WHERE CreatedOn >= DATEADD(YEAR,-1,GETDATE())
                    {tenantFilter}");

                    //-----------------------------------
                    // Duplicate records
                    //-----------------------------------
                    vm.DuplicateCount = await ExecuteScalarInt(conn, $@"
                    SELECT COUNT(*)
                    FROM (
                        SELECT Name
                        FROM {safeTableName}
                        GROUP BY Name
                        HAVING COUNT(*) > 1
                    ) x");



                    //-----------------------------------
                    // Chart Data
                    //-----------------------------------
                    vm.ChartLabels = new List<string>
                    {
                        "Today",
                        "Last 7 Days",
                        "Last 1 Month",
                        "Last 1 Year"
                    };

                    vm.ChartValues = new List<int>
                    {
                        vm.TodayCount,
                        vm.Last7DaysCount,
                        vm.Last1MonthCount,
                        vm.Last1YearCount
                    };

                }
            }

            return View(vm);
        }

        private async Task<int> ExecuteScalarInt(
            SqlConnection conn,
            string query)
        {
            try
            {
                using SqlCommand cmd = new SqlCommand(query, conn);
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch
            {
                return default;
            }
        }
    }
}