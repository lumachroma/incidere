using incidere.debut.Models.LocalUser;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;

namespace web.identity.server.Services
{
    public class IncidereUserService
    {
        private string m_incidereServiceUrl;
        private HttpClient m_incidereServiceClient;

        public IncidereUserService()
        {
            m_incidereServiceUrl = ConfigurationManager.AppSettings["IncidereBaseUrl"] ?? "http://localhost:50451";
            m_incidereServiceClient = new HttpClient { BaseAddress = new Uri(m_incidereServiceUrl) };
        }

        public List<LocalUser> GetUsers()
        {
            var firebaseUsers = new List<LocalUser>();
            try
            {
                var output = m_incidereServiceClient.GetStringAsync("api/local-users").Result;
                try
                {
                    var json = JObject.Parse(output).SelectToken("$._results");
                    foreach (var jtok in json)
                    {
                        var firebaseUser = jtok.ToObject<LocalUser>();
                        firebaseUsers.Add(firebaseUser);
                    }
                }
                catch (Exception)
                {
                    return firebaseUsers;
                }
            }
            catch (Exception)
            {
                return firebaseUsers;
            }

            return firebaseUsers;
        }
    }
}