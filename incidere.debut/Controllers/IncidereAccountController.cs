using incidere.debut.Models.Internals.Settings;
using incidere.debut.Models.LocalUser;
using incidere.debut.Services;
using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;

namespace incidere.debut.Controllers
{
    [RoutePrefix("incidere-account")]
    public class IncidereAccountController : Controller
    {
        private IncidereUserService m_incidereUserService;
        private IncidereSettingService m_incidereSettingService;
        private string m_baseUrl;

        public IncidereAccountController()
        {
            m_incidereUserService = new IncidereUserService();
            m_incidereSettingService = new IncidereSettingService();
            m_baseUrl = ConfigurationManager.AppSettings["IncidereBaseUrl"] ?? "http://localhost:50451/";
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

        [Authorize]
        [Route("send-create-password-mail/{id}")]
        [HttpGet]
        public async Task<ActionResult> SendCreatePasswordMail(string id)
        {
            var resultSuccess = true;
            var resultStatus = "OK";

            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var localUser = m_incidereUserService.GetUser(id);
            if (string.IsNullOrEmpty(localUser.FirebaseKey))
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            if (string.IsNullOrEmpty(localUser.Email))
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            var SettingKey = Guid.NewGuid().ToString();
            Setting setting = GenerateSetting(SettingKey, "Create Password Request", localUser.Email);
            try
            {
                setting = m_incidereSettingService.CreateSetting(setting);
            }
            catch (Exception ex)
            {
                resultSuccess = false;
                resultStatus = $"Error: {ex.Message}";
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { success = resultSuccess, status = resultStatus, userId = localUser.FirebaseKey }, JsonRequestBehavior.AllowGet);
            }

            IncidereMailServiceModel mail = GenerateCreatePasswordMail(localUser, setting);
            try
            {
                await IncidereMailService.SendCustomEmail(mail);
            }
            catch (Exception ex)
            {
                resultSuccess = false;
                resultStatus = $"Error: {ex.Message}";
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { success = resultSuccess, status = resultStatus, userId = localUser.FirebaseKey, settingId = setting.FirebaseKey }, JsonRequestBehavior.AllowGet);
            }

            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { success = resultSuccess, status = resultStatus, userId = localUser.FirebaseKey, settingId = setting.FirebaseKey }, JsonRequestBehavior.AllowGet);
        }

        private IncidereMailServiceModel GenerateCreatePasswordMail(LocalUser user, Setting setting)
        {
            return new IncidereMailServiceModel
            {
                EmailTo = user.Email,
                EmailToName = $"{user.FirstName} {user.LastName}",
                EmailFrom = "admin@incidere.com", // TODO: refactor
                EmailFromName = "Incidere Admin Team", // TODO: refactor
                EmailSubject = "Create Password",
                EmailBody = string.Empty,
                EmailCustomText1 = $"\nHelp us secure your account by verifying your email address ({user.Email}) and create your password.\n",
                EmailCustomText2 = $"\n\tFollow this link to continue: {m_baseUrl}incidere-account/create-password/{setting.FirebaseKey} \n",
                EmailCustomText3 = $"\nYou’re receiving this email because you recently created a new account or added a new email address. If this wasn’t you, please ignore this email.\n\n"
            };
        }

        private static Setting GenerateSetting(string key, string subject, string value)
        {
            return new Setting
            {
                Key = key,
                Value = value,
                Subject = subject,
                Text = string.Empty, // TODO: description here...
                IsActive = true
            };
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