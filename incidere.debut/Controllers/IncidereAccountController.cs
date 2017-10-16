using incidere.debut.Services;
using System;
using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;

namespace incidere.debut.Controllers
{
    [Authorize(Roles = "Admin, Administrator")]
    [RoutePrefix("incidere-account")]
    public class IncidereAccountController : Controller
    {
        private IncidereUserService m_incidereUserService;

        public IncidereAccountController()
        {
            m_incidereUserService = new IncidereUserService();
        }

        [Route("change-password/{id}")]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel passwordModel, string id)
        {
            var resultSuccess = true;
            var resultStatus = "OK";

            if (!(passwordModel.NewPassword == passwordModel.ConfirmPassword)
                || !(passwordModel.ConfirmPassword.Length >= 8))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var localUser = m_incidereUserService.GetUser(id);
            if (string.IsNullOrEmpty(localUser.FirebaseKey))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var verify = Crypto.VerifyHashedPassword(localUser.Password, passwordModel.OldPassword);
            if (!verify)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            localUser.Password = Crypto.HashPassword(passwordModel.ConfirmPassword);
            try
            {
                var result = m_incidereUserService.EditUser(localUser, id);
            }
            catch (Exception ex)
            {
                resultSuccess = false;
                resultStatus = $"Error: {ex.Message}";
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { success = resultSuccess, status = resultStatus, id = id });
            }

            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { success = resultSuccess, status = resultStatus, id = id });
        }
    }

    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}