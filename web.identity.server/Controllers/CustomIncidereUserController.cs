using System.Web.Mvc;
using web.identity.server.Services;

namespace web.identity.server.Controllers
{
    [Authorize]
    public class CustomIncidereUserController : Controller
    {
        private IncidereUserService m_incidereUserService;

        public CustomIncidereUserController()
        {
            m_incidereUserService = new IncidereUserService();
        }

        // GET: CustomIncidereUser
        public ActionResult Index()
        {
            var incidereUsers = m_incidereUserService.GetUsers();
            return View(incidereUsers);
        }

        // GET: CustomIncidereUser/Details/5
        public ActionResult Details(string id)
        {
            var incidereUser = m_incidereUserService.GetUser(id);

            if (string.IsNullOrEmpty(incidereUser.FirebaseKey))
            {
                return HttpNotFound();
            }

            return View(incidereUser);
        }

        // GET: CustomIncidereUser/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomIncidereUser/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CustomIncidereUser/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CustomIncidereUser/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CustomIncidereUser/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CustomIncidereUser/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
