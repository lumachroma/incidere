﻿using IdentityModel.Client;
using IdentityServer3.Core;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Helpers;
using web.identity.server.IdentityServer;
using web.identity.server.Services;

[assembly: OwinStartup(typeof(web.identity.server.Startup))]

namespace web.identity.server
{
    public class Startup
    {
        private string m_incidereBaseUrl;
        private string m_idSvrBaseUrl;

        public Startup()
        {
            m_incidereBaseUrl = ConfigurationManager.AppSettings["IncidereBaseUrl"] ?? "http://localhost:50451/";
            m_idSvrBaseUrl = ConfigurationManager.AppSettings["IdSvrBaseUrl"] ?? "http://localhost:50450/";
        }

        public void Configuration(IAppBuilder app)
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            var factory = new IdentityServerServiceFactory()
                .UseInMemoryClients(Clients.Get())
                .UseInMemoryScopes(Scopes.Get());
            var userService = new IncidereUserAuthenticationService();
            factory.UserService = new Registration<IUserService>(resolver => userService);

            app.Map("/identity", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Web IdentityServer3",
                    SigningCertificate = LoadCertificate(),

                    Factory = factory,

                    AuthenticationOptions = new IdentityServer3.Core.Configuration.AuthenticationOptions
                    {
                        EnablePostSignOutAutoRedirect = true,
                        //IdentityProviders = ConfigureIdentityProviders
                    },

                    RequireSsl = false
                });
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = $"{m_idSvrBaseUrl}identity",
                ClientId = "mvc",
                Scope = "openid profile roles extra incidereServiceApi",
                RedirectUri = $"{m_idSvrBaseUrl}",
                ResponseType = "id_token token",
                SignInAsAuthenticationType = "Cookies",
                UseTokenLifetime = false,

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = async n =>
                    {
                        var nid = new ClaimsIdentity(
                            n.AuthenticationTicket.Identity.AuthenticationType,
                            Constants.ClaimTypes.GivenName,
                            Constants.ClaimTypes.Role);

                        var userInfoClient = new UserInfoClient(
                            new Uri(n.Options.Authority + "/connect/userinfo"),
                            n.ProtocolMessage.AccessToken);

                        var userInfo = await userInfoClient.GetAsync();

                        userInfo.Claims.ToList().ForEach(ui => nid.AddClaim(new Claim(ui.Item1, ui.Item2)));
                        nid.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));
                        nid.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString()));
                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                        nid.AddClaim(new Claim("sid", n.AuthenticationTicket.Identity.FindFirst("sid").Value));
                        nid.AddClaim(new Claim("app_specific", "some data"));

                        n.AuthenticationTicket = new AuthenticationTicket(nid, n.AuthenticationTicket.Properties);
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
        }
        //private void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        //{
        //    var google = new GoogleOAuth2AuthenticationOptions
        //    {
        //        AuthenticationType = "Google",
        //        Caption = "Sign-in with Google",
        //        SignInAsAuthenticationType = signInAsType,

        //        // Google APIs Credentials: https://console.developers.google.com/apis/credentials?project=identityserver-lumachroma&organizationId=801644395166
        //        ClientId = "703287403163-cldlftau1ikq7bcb3qg1v0msejpf7rmu.apps.googleusercontent.com",
        //        ClientSecret = "JiTTgBocaU0kYrrQ9KJBeXbx"
        //    };
        //    app.UseGoogleAuthentication(google);

        //    var fb = new FacebookAuthenticationOptions
        //    {
        //        AuthenticationType = "Facebook",
        //        Caption = "Sign-in with Facebook",
        //        SignInAsAuthenticationType = signInAsType,

        //        // Facebook for Developers: https://developers.facebook.com/apps/273791976367235/dashboard/
        //        AppId = "273791976367235",
        //        AppSecret = "ee82053d9bde8b87b4a4b2e70ad971ee"
        //    };
        //    app.UseFacebookAuthentication(fb);
        //}

        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\bin\IdentityServer\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
    }
}