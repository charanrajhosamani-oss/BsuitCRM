using BSuit.Contracts.Services;
using BSuit.Core.Data;
using BSuit.Identity.Data;
using BSuit.SalesCRM.Data;
using Microsoft.AspNetCore.Mvc;

namespace BSuit.API.Areas.SalesCRM.Controllers
{
    [Area("SalesCRM")]
    public class DashboardController : Controller
    {

        private readonly SalesCRMContext _crmDbcontext;
        private readonly CoreDbContext _coreDbContext;
        private readonly IdentityDbContext _identityDbContext;
        private readonly IUserContext _iUserContext;

        public DashboardController(SalesCRMContext crmDbcontext, 
            CoreDbContext coreDbContext, IdentityDbContext identityDbContext, 
            IUserContext iUserContext)
        {
            _crmDbcontext = crmDbcontext;
            _coreDbContext = coreDbContext;
            _identityDbContext = identityDbContext;
            _iUserContext = iUserContext;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
