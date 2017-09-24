using IdentityServer3.Core.Models;
using System.Collections.Generic;

namespace web.identity.server.IdentityServer
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            var scopes = new List<Scope>
            {
                new Scope
                {
                    Enabled = true,
                    Name = "roles",
                    Type = ScopeType.Identity,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role")
                    }
                },
                new Scope
                {
                    Enabled = true,
                    Name = "extra",
                    Type = ScopeType.Identity,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("preferred_username"),
                        new ScopeClaim("email"),
                        new ScopeClaim("address")
                    }
                },
                new Scope
                {
                    Enabled = true,
                    DisplayName = "Incidere Service API",
                    Name = "incidereServiceApi",
                    Description = "Access to API",
                    Type = ScopeType.Resource,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role"),
                        new ScopeClaim("preferred_username"),
                        new ScopeClaim("email"),
                        new ScopeClaim("address")
                    }
                }
            };

            scopes.AddRange(StandardScopes.All);

            return scopes;
        }
    }
}