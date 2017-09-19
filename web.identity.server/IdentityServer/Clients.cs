using IdentityServer3.Core.Models;
using System.Collections.Generic;

namespace web.identity.server.IdentityServer
{
    public static class Clients
    {
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
                        "http://localhost:50450/"
                    },

                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:50450/"
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
                        "http://localhost:50451/"
                    },

                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:50451/"
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