using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;

namespace incidere.debut.Services
{
    public class IncidereMailService
    {
        public static async Task SendEmail(IncidereMailServiceModel model)
        {
            using (var smtp = new SmtpClient())
            {
                var mail = new MailMessage(model.EmailFrom, model.EmailTo)
                {
                    Subject = model.EmailSubject,
                    Body = model.EmailBody,
                    IsBodyHtml = false
                };
                await smtp.SendMailAsync(mail);
            }
        }

        public static async Task SendCustomEmail(IncidereMailServiceModel model)
        {
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"] ?? "Incidere";
            var cutomEmailSubject = $"[{applicationName}] {model.EmailSubject}";
            var customEmailBody = $@"Greetings {model.EmailToName},
{model.EmailCustomText1}
{model.EmailCustomText2}
{model.EmailCustomText3}
Regards,
{model.EmailFromName}

[Powered by {applicationName}]
";
            using (var smtp = new SmtpClient())
            {
                var mail = new MailMessage(model.EmailFrom, model.EmailTo)
                {
                    Subject = cutomEmailSubject,
                    Body = customEmailBody,
                    IsBodyHtml = false
                };
                await smtp.SendMailAsync(mail);
            }
        }
    }

    public class IncidereMailServiceModel
    {
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EmailCustomText1 { get; set; }
        public string EmailCustomText2 { get; set; }
        public string EmailCustomText3 { get; set; }
        public string EmailFromName { get; set; }
        public string EmailToName { get; set; }
    }
}