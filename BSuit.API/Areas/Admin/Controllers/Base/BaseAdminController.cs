using BSuit.API.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BSuit.API.Areas.Admin.Controllers.Base
{
    [Area(nameof(PARAMS.ADMIN))]
    [Authorize(Policy = POLICIES.Superadmin_Admin_TenantsOnly)]
    public class BaseAdminController : Controller
    {
        //
    }

}
