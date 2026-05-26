// Areas/Admin/Controllers/SubscriptionsController.cs
using BSuit.API.Areas.Admin.Controllers.Base;
using BSuit.API.Areas.Admin.Models;
using BSuit.Core.Data;
using BSuit.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BSuit.API.Areas.Admin.Controllers
{
    public class SubscriptionsController : BaseAdminController
    {
        private readonly CoreDbContext _context;

        public SubscriptionsController(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(SubscriptionVM model)
        {
            var query = _context.Subscriptions.AsQueryable();

            if (!string.IsNullOrEmpty(model.Search))
                query = query.Where(x => x.Name.Contains(model.Search));

            model.TotalCount = await query.CountAsync();

            model.Items = await query
                .Skip((model.PageNumber - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SubscriptionMaster s)
        {
            _context.Subscriptions.Add(s);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Subscription created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SubscriptionMaster s)
        {
            _context.Subscriptions.Update(s);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Subscription changed successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            var item = await _context.Subscriptions.FindAsync(id);
            item.IsActive = !item.IsActive;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Status changed successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Subscriptions.FindAsync(id);
            _context.Subscriptions.Remove(item);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Subscription removed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}