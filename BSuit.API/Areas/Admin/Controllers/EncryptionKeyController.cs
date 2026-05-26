using BSuit.API.Areas.Admin.Controllers.Base;
using BSuit.API.Areas.Admin.Models;
using BSuit.Core.Data;
using BSuit.Core.Entities;
using BSuit.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSuit.API.Areas.Admin.Controllers
{
    public class EncryptionKeyController : BaseAdminController
    {
        private readonly CoreDbContext _context;

        public EncryptionKeyController(CoreDbContext context)
        {
            _context = context;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index(EncryptionKeyVM model)
        {
            var query = _context.EncryptionKeys.AsQueryable();

            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(x => x.Version.ToString().Contains(model.Search));
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

        // ================= REGENERATE KEY =================
        [HttpPost]
        public async Task<IActionResult> Regenerate(Guid tenantId)
        {
            // Get latest version
            var latest = await _context.EncryptionKeys
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.Version)
                .FirstOrDefaultAsync();

            int newVersion = (latest?.Version ?? 0) + 1;

            // Generate new key
            var protectedKey = CryptoHelper.Generate256BitKey();
           
            // Deactivate old keys
            var oldKeys = await _context.EncryptionKeys
                .Where(x => x.TenantId == tenantId && x.IsActive)
                .ToListAsync();

            foreach (var k in oldKeys)
            {
                k.IsActive = false;
                k.ModifiedOn = DateTime.UtcNow;
            }

            // Add new key
            var newKey = new EncryptionKey
            {
                TenantId = tenantId,
                Version = newVersion,
                EncryptedKey = protectedKey,
                IsActive = true,
                CreatedOn = DateTime.UtcNow
            };

            _context.EncryptionKeys.Add(newKey);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE =================
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _context.EncryptionKeys.FindAsync(id);

            _context.EncryptionKeys.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}