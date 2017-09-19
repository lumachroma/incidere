using IdentityServer3.Core.Models;
using System.Collections.Generic;
using System.Configuration;

namespace web.identity.server.IdentityServer
{
    public static class Clients
    {
        private static string m_incidereBaseUrl = ConfigurationManager.AppSettings["IncidereBaseUrl"] ?? "http://localhost:50451/";
        private static string m_idSvrBaseUrl = ConfigurationManager.AppSettings["IdSvrBaseUrl"] ?? "http://localhost:50450/";

        public static IEnumerable<Client> Get()
        {
            return new[]
            {
                new Client
                {
                    Enabled = true,
                    ClientName = "MVC Client",
                    ClientId = "mvc",
                    Flow = Flows.Implicit,

                    RedirectUris = new List<string>
                    {
                        $"{m_idSvrBaseUrl}"
                    },

                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{m_idSvrBaseUrl}"
                    },

                    AllowAccessToAllScopes = true
                },
                new Client
                {
                    Enabled = true,
                    ClientName = "MVC Client (Incidere)",
                    ClientId = "mvc_incidere",
                    Flow = Flows.Implicit,

                    RedirectUris = new List<string>
                    {
                        $"{m_incidereBaseUrl}"
                    },

                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{m_incidereBaseUrl}"
                    },

                    AllowAccessToAllScopes = true
                },
                new Client
                {
                    ClientName = "MVC Client (incidere service communication)",
                    ClientId = "mvc_incidere_service",
                    Flow = Flows.ClientCredentials,

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedScopes = new List<string>
                    {
                        "incidereServiceApi"
                    }
                }
            };
        }
    }
}