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
                    IncludeAllClaimsForUser = true,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role")
                    }                    
                },
                new Scope
                {
                    Enabled = true,
                    DisplayName = "Incidere Service API",
                    Name = "incidereServiceApi",
                    Description = "Access to API",
                    Type = ScopeType.Resource,
                    IncludeAllClaimsForUser = true,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role")
                    }
                }
            };

            scopes.AddRange(StandardScopes.All);

            return scopes;
        }
    }
}