using BSuit.API.Areas.Admin.Controllers.Base;
using BSuit.API.Areas.Admin.Models;
using BSuit.Core.Data;
using BSuit.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSuit.API.Areas.Admin.Controllers
{
    public class PIIConfigController : BaseAdminController
    {
        private readonly CoreDbContext _context;

        public PIIConfigController(CoreDbContext context)
        {
            _context = context;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index(PIIConfigVM model)
        {
            var query = _context.PIIConfigs.AsQueryable();

            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(x =>
                    x.TableName.Contains(model.Search) ||
                    x.ColumnName.Contains(model.Search));
            }

            model.TotalCount = await query.CountAsync();

            model.Items = await query
                .OrderByDescending(x => x.CreatedOn)
                .Skip((model.PageNumber - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            model.Tenants = await _context.Tenants
                .Where(t => t.IsActive)
                .ToListAsync();

            return View(model);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> Create(PIIConfig p)
        {
            p.CreatedOn = DateTime.UtcNow;
            p.ModifiedOn = DateTime.UtcNow;

            _context.PIIConfigs.Add(p);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Data created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT =================
        [HttpPost]
        public async Task<IActionResult> Edit(PIIConfig p)
        {
            var existing = await _context.PIIConfigs.FindAsync(p.Id);

            if (existing == null)
                return NotFound();

            existing.TenantId = p.TenantId;
            existing.TableName = p.TableName;
            existing.ColumnName = p.ColumnName;
            existing.ModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Data updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ================= TOGGLE =================
        [HttpPost]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var item = await _context.PIIConfigs.FindAsync(id);

            item.IsActive = !item.IsActive;
            item.ModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Status updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE =================
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _context.PIIConfigs.FindAsync(id);

            _context.PIIConfigs.Remove(item);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Data removed successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ================= GET TABLES =================
        [HttpGet]
        public IActionResult GetTables()
        {
            var tables = _context.Model
                .GetEntityTypes()
                .Select(t => t.GetTableName())
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return Json(tables);
        }

        // ================= GET COLUMNS =================
        [HttpGet]
        public IActionResult GetColumns(string tableName)
        {
            var entity = _context.Model
                .GetEntityTypes()
                .FirstOrDefault(t => t.GetTableName() == tableName);

            if (entity == null)
                return Json(new List<string>());

            var columns = entity.GetProperties()
                .Where(p => p.ClrType == typeof(string)) // Only string columns (PII)
                .Select(p => p.Name)
                .ToList();

            return Json(columns);
        }
    }
}