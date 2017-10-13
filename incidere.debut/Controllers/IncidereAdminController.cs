using System.Security.Claims;
using System.Web.Mvc;

namespace incidere.debut.Controllers
{
    public class IncidereAdminController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var caller = User as ClaimsPrincipal;

            if (caller.HasClaim("role", "Admin") || caller.HasClaim("role", "Administrator"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Incidere");
            }
        }
    }
}