using IdentityServer3.Core;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web.Mvc;

namespace incidere.debut.Controllers
{
    [RoutePrefix("config")]
    public class CustomConfigController : Controller
    {
        [Authorize]
        [Route("")]
        public ActionResult CreateConfig()
        {
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var claims = (User as ClaimsPrincipal).Claims;

            var userRoles = new List<string>();
            var allClaimsDeclaration = new StringBuilder();
            var allClaimsReturn = new StringBuilder();
            var allRoles = new StringBuilder();

            foreach (var claim in claims)
            {
                if (claim.Type == Constants.ClaimTypes.Subject
                    || claim.Type == Constants.ClaimTypes.PreferredUserName
                    || claim.Type == Constants.ClaimTypes.Email
                    || claim.Type == Constants.ClaimTypes.GivenName
                    || claim.Type == Constants.ClaimTypes.FamilyName
                    || claim.Type == Constants.ClaimTypes.Address
                    || claim.Type == Constants.ClaimTypes.BirthDate)
                {
                    allClaimsDeclaration.AppendLine($"\tvar {claim.Type} = \"{claim.Value}\";");
                    allClaimsReturn.AppendLine($"\t\t{claim.Type}: {claim.Type},");
                }
                if (claim.Type == Constants.ClaimTypes.Role)
                {
                    userRoles.Add(claim.Value);
                }
            }

            allRoles.Append("[");
            foreach (var userRole in userRoles)
            {
                if (userRole != userRoles.Last())
                {
                    allRoles.Append($"\"{userRole}\", ");
                }
                else
                {
                    allRoles.Append($"\"{userRole}\"");
                }
            }
            allRoles.Append("]");

            allClaimsDeclaration.AppendLine($"\tvar roles = {allRoles.ToString()};");
            allClaimsReturn.AppendLine($"\t\troles: roles");

            var js = $@"define('services/config', [], function() {{
    var application_name = ""{applicationName}"";
{allClaimsDeclaration.ToString()}
                        
    return {{
        application_name: application_name,
{allClaimsReturn.ToString()}
    }};
}});";

            return Content(js);
        }
    }
}