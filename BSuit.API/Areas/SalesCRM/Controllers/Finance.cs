
using BSuit.Contracts.Services;
using BSuit.Core.Data;
using BSuit.Identity.Data;
using BSuit.SalesCRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSuit.API.Areas.SalesCRM.Controllers
{
    [Area("SalesCRM")]
    public class Finance : Controller
    {
        public readonly BSuit.SalesCRM.Data.SalesCRMContext _crmContext;
        public readonly IdentityDbContext _identityContext;
        public readonly CoreDbContext _coreContext;
        public readonly IUserContext _userContext;

        public Finance(BSuit.SalesCRM.Data.SalesCRMContext cRMContext, IdentityDbContext identityDbContext, CoreDbContext coreDbContext, IUserContext userContext)
        {
            _crmContext = cRMContext;
            _identityContext = identityDbContext;
            _coreContext = coreDbContext;
            _userContext = userContext;
        }

        public async  Task<IActionResult> Index()
        {
            //Get Tenant specific Data.
            List<BSuit.SalesCRM.Entities.Payment> payments = await _crmContext.Payments.Where(m => m.TenantId == _userContext.TenantId && m.IsFinanceRequestSent == true).ToListAsync();
            var users = await _identityContext.Users.Where(m => m.TenantId == _userContext.TenantId).ToListAsync();
            var paymentTypes = await _crmContext.PaymentTypes.ToListAsync();
            FinanceVM model = new FinanceVM();

            model.PaymentTypes = paymentTypes;

            List<PaymentMilestoneDetails> paymentList = new List<PaymentMilestoneDetails>();

            foreach (var p in payments)
            {
                var wo = _crmContext.WorkOrders.FirstOrDefault(p => p.WorkOrderId == p.WorkOrderId);
                var opD = _crmContext.Opportunities.FirstOrDefault(p => p.OpportunityId == wo.OpportunityId);

                PaymentMilestoneDetails data = new PaymentMilestoneDetails();
                data.BDRemarks = p.BDRemarks ?? "";
                data.PaymentId = p.PaymentId;
                data.PaymentLink = p.PaymentLink ?? "";
                data.PaymentMilestone = p.PaymentMilestone ?? "";
                data.InvoiceAmount = p.InvoiceAmount ?? 0;
                data.Hours = p.Hours ?? 0;
                data.HourlyRate = p.HourlyRate ?? 0;
                data.WorkOrderId = p.WorkOrderId ?? Guid.Empty;
                data.PaymentDueDate = p.PaymentDueDate;
                data.PaymentType = paymentTypes.FirstOrDefault(m => m.PaymentTypeId == p.PaymentTypeId)?.PaymentTypeName ?? "";
                data.OpportunityName = opD.OpportunityName ?? "";
                data.ServiceName = "";
                paymentList.Add(data);
            }


            model.payments = paymentList;

            return View(model);
        }

        public async Task<IActionResult> Details(Guid paymentId)
        {
            var payment = _crmContext.Payments.FirstOrDefault(m => m.PaymentId == paymentId);

            if (payment == null)
            {
                return NotFound();
            }
            //Work Order Details
            var workOrder = await _crmContext.WorkOrders.FirstOrDefaultAsync(m => m.WorkOrderId == payment.WorkOrderId);
            if (workOrder == null)
            {
                return NotFound();
            }
            //Opportunity Details
            var opp = await _crmContext.Opportunities.FirstOrDefaultAsync(m => m.OpportunityId == workOrder.OpportunityId);

            if(opp == null)
            {
                return NotFound();
            }
            //Opportunity Service
            var opps = await _crmContext.OpportunityServices.FirstOrDefaultAsync(m => m.OpportunityId == opp.OpportunityId);

            if (opps == null)
            {
                return NotFound();
            }
            //Service Name
            var service = await _crmContext.Services.FirstOrDefaultAsync(p => p.ServiceId == opps.ServiceId);
            if (service == null)
            {
                return NotFound();
            }
            PaymentMilestoneDetails model = new PaymentMilestoneDetails();
            model.BDRemarks = payment.BDRemarks ?? "";
            model.OpportunityName = opp.OpportunityName ?? "";
            model.ServiceName = service.ServiceName;
            model.ClientName = "";
            model.PaymentId = paymentId;
            model.PaymentLink = payment.PaymentLink ?? "";
            model.WorkOrderLink = "";
            model.Hours = payment.Hours ?? 0;
            model.HourlyRate = payment.HourlyRate ?? 0;
            model.PaymentDueDate = payment.PaymentDueDate;
            model.InvoiceAmount = payment.InvoiceAmount ?? 0;
            model.WorkOrderId = payment.WorkOrderId ?? Guid.Empty;


            //Finance team will update the below details

            //Transaction Number, Payment Received Date and Time(Date and Time Picker), Amount Received, Mode of payment(Dropdown), Payment Currency(Dropdown)

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFinanceDetails(PaymentMilestoneDetails model)
        {
            if (!ModelState.IsValid)
            {
                return View("Details", model);
            }

            // Get Payment
            var payment = await _crmContext.Payments
                .FirstOrDefaultAsync(x => x.PaymentId == model.PaymentId);

            if (payment == null)
            {
                return NotFound();
            }

            // Update Finance Details
            payment.TransactionNumber = model.TransactionNumber;
            payment.PaymentReceivedOn = model.PaymentReceivedDateTime;
            payment.AmountRecieved = model.AmountReceived;
            //payment.PaymentModeId = model.ModeOfPayment;
            //payment.PaymentCurrency = model.PaymentCurrency;
            payment.FinanceRemarks = model.FinanceRemarks;

            // Optional Approval Status
            //payment.FinanceStatus = "Approved";

            // Audit Fields
            //payment.ModifiedOn = DateTime.UtcNow;

            // If you have logged-in user tracking
            // payment.ModifiedBy = User.Identity.Name;

            await _crmContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Finance details updated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
