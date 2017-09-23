using System.Web.Mvc;

namespace incidere.debut.Controllers
{
    public class IncidereController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}