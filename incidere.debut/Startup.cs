using IdentityServer3.AccessTokenValidation;
using IdentityServer3.Core;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;

[assembly: OwinStartup(typeof(incidere.debut.Startup))]

namespace incidere.debut
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = "http://localhost:50450/identity",
                ClientId = "mvc_incidere",
                Scope = "openid profile roles",
                RedirectUri = "http://localhost:50451/",
                ResponseType = "id_token",
                SignInAsAuthenticationType = "Cookies",

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = n =>
                    {
                        var id = n.AuthenticationTicket.Identity;

                        var name = id.FindFirst(Constants.ClaimTypes.Name);
                        var givenName = id.FindFirst(Constants.ClaimTypes.GivenName);
                        var familyName = id.FindFirst(Constants.ClaimTypes.FamilyName);
                        var sub = id.FindFirst(Constants.ClaimTypes.Subject);
                        var roles = id.FindAll(Constants.ClaimTypes.Role);

                        var nid = new ClaimsIdentity(
                            id.AuthenticationType,
                            Constants.ClaimTypes.GivenName,
                            Constants.ClaimTypes.Role);

                        if (null != name)
                            nid.AddClaim(name);
                        if (null != givenName)
                            nid.AddClaim(givenName);
                        if (null != familyName)
                            nid.AddClaim(familyName);
                        nid.AddClaim(sub);
                        nid.AddClaims(roles);
                        nid.AddClaim(new Claim("app_specific", "some data"));

                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            nid,
                            n.AuthenticationTicket.Properties);

                        return Task.FromResult(0);
                    },

                    RedirectToIdentityProvider = n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }

                        return Task.FromResult(0);
                    }
                }
            });

            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = "http://localhost:50450/identity",
                ValidationMode = ValidationMode.ValidationEndpoint,

                RequiredScopes = new[] { "sampleApi" }
            });
        }
    }
}