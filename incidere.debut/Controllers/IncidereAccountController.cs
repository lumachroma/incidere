using incidere.debut.Services;
using System;
using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;

namespace incidere.debut.Controllers
{
    [RoutePrefix("incidere-account")]
    public class IncidereAccountController : Controller
    {
        private IncidereUserService m_incidereUserService;
        private IncidereMailService m_incidereMailService;

        public IncidereAccountController()
        {
            m_incidereUserService = new IncidereUserService();
            m_incidereMailService = new IncidereMailService();
        }

        [Authorize]
        [Route("change-password/{id}")]
        [HttpPost]
        public ActionResult ChangePassword(IncidereAccountChangePasswordModel model, string id)
        {
            var resultSuccess = true;
            var resultStatus = "OK";

            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (!ValidatePasswords(model.NewPassword, model.ConfirmPassword))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var localUser = m_incidereUserService.GetUser(id);
            if (string.IsNullOrEmpty(localUser.FirebaseKey))
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            if (!VerifyPassword(localUser.Password, model.OldPassword))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            localUser.Password = CreateHashedPassword(model.ConfirmPassword);
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

        private static bool ValidatePasswords(string newPassword, string confirmPassword)
        {
            // TODO: validate password strength
            if ((newPassword == confirmPassword) && (confirmPassword.Length >= 8))
            {
                return true;
            }
            return false;
        }

        private static bool VerifyPassword(string hashedPassword, string password)
        {
            return Crypto.VerifyHashedPassword(hashedPassword, password);
        }

        private static string CreateHashedPassword(string password)
        {
            return Crypto.HashPassword(password);
        }
    }

    public class IncidereAccountChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}