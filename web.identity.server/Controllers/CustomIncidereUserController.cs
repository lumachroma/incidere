using incidere.debut.Models.LocalUser;
using System.Data;
using System.Net;
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
            ViewBag.roleEditboxes = 5;

            return View();
        }

        // POST: CustomIncidereUser/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Password,Email,FirstName,LastName,DateOfBirth,Location,Roles")]
            LocalUser localUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    localUser.Roles.RemoveAll(role => string.IsNullOrEmpty(role));

                    var result = m_incidereUserService.CreateUser(localUser);
                    if (result)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(localUser);
        }

        // GET: CustomIncidereUser/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.roleEditboxes = 5;

            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var localUser = m_incidereUserService.GetUser(id);
            if (string.IsNullOrEmpty(localUser.FirebaseKey))
            {
                return HttpNotFound();
            }

            return View(localUser);
        }

        // POST: CustomIncidereUser/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Username,Password,Email,FirstName,LastName,DateOfBirth,Location,Roles")]
            LocalUser localUser, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var localUserFromSource = m_incidereUserService.GetUser(id);
            if (string.IsNullOrEmpty(localUserFromSource.FirebaseKey))
            {
                return HttpNotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    localUser.Roles.RemoveAll(role => string.IsNullOrEmpty(role));
                    localUser.ReferenceNo = localUserFromSource.ReferenceNo;
                    localUser.ExternalUsers = localUserFromSource.ExternalUsers;

                    var result = m_incidereUserService.EditUser(localUser, id);
                    if (result)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(localUser);
        }

        // GET: CustomIncidereUser/Delete/5
        public ActionResult Delete(string id, bool? saveChangesError = false)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            var localUser = m_incidereUserService.GetUser(id);
            if (string.IsNullOrEmpty(localUser.FirebaseKey))
            {
                return HttpNotFound();
            }

            return View(localUser);
        }

        // POST: CustomIncidereUser/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            try
            {
                var result = m_incidereUserService.DeleteUser(id);
                if (!result)
                {
                    return RedirectToAction("Delete", new { id = id, saveChangesError = true });
                }

                var localUser = m_incidereUserService.GetUser(id);
                if (!string.IsNullOrEmpty(localUser.FirebaseKey))
                {
                    return RedirectToAction("Delete", new { id = id, saveChangesError = true });
                }
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

            return RedirectToAction("Index");
        }
    }
}
