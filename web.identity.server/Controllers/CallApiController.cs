using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmbeddedMvc.Controllers
{
    public class CallApiController : Controller
    {
        private string m_incidereBaseUrl;
        private string m_idSvrBaseUrl;

        public CallApiController()
        {
            m_incidereBaseUrl = ConfigurationManager.AppSettings["IncidereBaseUrl"] ?? "http://localhost:50451/";
            m_idSvrBaseUrl = ConfigurationManager.AppSettings["IdSvrBaseUrl"] ?? "http://localhost:50450/";
        }

        // GET: CallApi/ClientCredentials
        public async Task<ActionResult> ClientCredentials()
        {
            var response = await GetTokenAsync();
            var result = await CallApi(response.AccessToken);

            ViewBag.Json = result;
            return View("ShowApiResult");
        }

        // GET: CallApi/UserCredentials
        public async Task<ActionResult> UserCredentials()
        {
            var user = User as ClaimsPrincipal;
            var token = user.FindFirst("access_token").Value;
            var result = await CallApi(token);

            ViewBag.Json = result;
            return View("ShowApiResult");
        }

        private async Task<string> CallApi(string token)
        {
            var client = new HttpClient();
            client.SetBearerToken(token);

            var json = await client.GetStringAsync($"{m_incidereBaseUrl}identity");
            return JArray.Parse(json).ToString();
        }

        private async Task<TokenResponse> GetTokenAsync()
        {
            var client = new TokenClient(
                $"{m_idSvrBaseUrl}identity/connect/token",
                "mvc_incidere_service",
                "secret");

            return await client.RequestClientCredentialsAsync("incidereServiceApi");
        }
    }
}