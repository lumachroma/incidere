using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Models;
using Microsoft.Owin;
using Owin;
using System;
using System.Security.Cryptography.X509Certificates;
using web.identity.server.IdentityServer;

[assembly: OwinStartup(typeof(web.identity.server.Startup))]

namespace web.identity.server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/identity", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Web IdentityServer3",
                    SigningCertificate = LoadCertificate(),

                    Factory = new IdentityServerServiceFactory()
                                .UseInMemoryUsers(Users.Get())
                                .UseInMemoryClients(Clients.Get())
                                .UseInMemoryScopes(StandardScopes.All),

                    RequireSsl = false
                });
            });
        }

        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\bin\identityServer\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
    }
}