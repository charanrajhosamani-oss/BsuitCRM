// Areas/Admin/Controllers/TenantSubscriptionsController.cs
using BSuit.API.Areas.Admin.Controllers.Base;
using BSuit.API.Areas.Admin.Models;
using BSuit.Contracts.Services;
using BSuit.Core.Data;
using BSuit.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSuit.API.Areas.Admin.Controllers
{
    public class TenantSubscriptionsController : BaseAdminController
    {
        private readonly CoreDbContext _context;
        private readonly IUserContext _userContext;
        public TenantSubscriptionsController(CoreDbContext context,
            IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index(TenantSubscriptionVM model)
        {
            var query = _context.TenantSubscriptions
                .Include(x => x.Tenant)
                .Include(x => x.Subscription)
                .AsQueryable();

            if (model.TenantId != Guid.Empty)
                query = query.Where(x => x.TenantId == model.TenantId);

            // SEARCH
            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(x =>
                    x.Tenant.Name.Contains(model.Search) ||
                    x.Subscription.Name.Contains(model.Search));
            }

            model.TotalCount = await query.CountAsync();

            model.Items = await query
                .OrderByDescending(x => x.Id)
                .Skip((model.PageNumber - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            // Dropdowns
            if (_userContext.IsSuperAdmin == true)
            {
                model.Tenants = await _context.Tenants                    
                    .ToListAsync();
            }
            else
            {
                model.Tenants = await _context.Tenants
                   .Where(t => t.Id == _userContext.TenantId)
                   .ToListAsync();
            }

            model.Subscriptions = await _context.Subscriptions
                .Where(s => s.IsActive)
                .ToListAsync();

            return View(model);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> Create(Guid tenantId, int subscriptionId)
        {
            var plan = await _context.Subscriptions
                .FirstOrDefaultAsync(x => x.Id == subscriptionId);

            if (plan == null)
            {
                TempData["Error"] = "Subscription plan not found.";
                return RedirectToAction(nameof(Index));
            }

            // Check existing subscription for tenant
            var existingSubscription = await _context.TenantSubscriptions
                .FirstOrDefaultAsync(x => x.TenantId == tenantId);

            if (existingSubscription != null)
            {
                // Update existing subscription
                existingSubscription.TenantId = tenantId;
                existingSubscription.SubscriptionId = subscriptionId;
                existingSubscription.StartDate = DateTime.UtcNow;
                existingSubscription.EndDate = DateTime.UtcNow.AddDays(plan.DurationInDays);
                
                _context.TenantSubscriptions.Update(existingSubscription);

                TempData["Success"] = "Subscription updated successfully.";
            }
            else
            {
                // Create new subscription
                var entity = new TenantSubscription
                {
                    TenantId = tenantId,
                    SubscriptionId = subscriptionId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(plan.DurationInDays),
                    IsActive = true
                };

                _context.TenantSubscriptions.Add(entity);

                TempData["Success"] = "Subscription created successfully.";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { tenantId });
        }

        // ================= TOGGLE =================
        [HttpPost]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var item = await _context.TenantSubscriptions.FindAsync(id);

            item.IsActive = !item.IsActive;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Subscription status changed successfully.";
            return RedirectToAction(nameof(Index), new { tenantId = item.TenantId });
        }

        // ================= DELETE =================
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _context.TenantSubscriptions.FindAsync(id);

            _context.TenantSubscriptions.Remove(item);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Subscription deleted successfully.";
            return RedirectToAction(nameof(Index), new { tenantId = item.TenantId });
        }
    }
}