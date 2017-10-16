using IdentityServer3.Core;
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;
using incidere.debut.Models.LocalUser;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace web.identity.server.Services
{
    public class IncidereUserAuthenticationService : UserServiceBase
    {
        private IncidereUserService m_incidereUserService;
        public static List<CustomLocalUser> IncidereUsers = new List<CustomLocalUser>();

        public IncidereUserAuthenticationService()
        {
            m_incidereUserService = new IncidereUserService();
        }

        public override Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            var user = IncidereUsers.SingleOrDefault(x => x.Username == context.UserName
                && Crypto.VerifyHashedPassword(x.Password, context.Password));
            if (user == null)
            {
                RefreshUsersList();
                user = IncidereUsers.SingleOrDefault(x => x.Username == context.UserName
                    && Crypto.VerifyHashedPassword(x.Password, context.Password));
                if (user == null)
                {
                    return Task.FromResult(0);
                }
            }

            context.AuthenticateResult = new AuthenticateResult(user.Id, user.Username);

            return Task.FromResult(0);
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = IncidereUsers.SingleOrDefault(x => x.Id == context.Subject.GetSubjectId());
            if (user != null)
            {
                context.IssuedClaims = user.Claims.Where(x => context.RequestedClaimTypes.Contains(x.Type));
            }

            return Task.FromResult(0);
        }

        private void RefreshUsersList()
        {
            IncidereUsers.Clear();

            var localUsers = m_incidereUserService.GetUsers();
            foreach (var localUser in localUsers)
            {
                var customLocalUsers = new CustomLocalUser
                {
                    Id = localUser.Id,
                    Username = localUser.Username,
                    Password = localUser.Password,
                    Email = localUser.Email
                };
                customLocalUsers.Claims.Add(new Claim(Constants.ClaimTypes.Subject, localUser.FirebaseKey));
                customLocalUsers.Claims.Add(new Claim(Constants.ClaimTypes.PreferredUserName, localUser.Username));
                customLocalUsers.Claims.Add(new Claim(Constants.ClaimTypes.Email, localUser.Email));
                customLocalUsers.Claims.Add(new Claim(Constants.ClaimTypes.GivenName, localUser.FirstName));
                customLocalUsers.Claims.Add(new Claim(Constants.ClaimTypes.FamilyName, localUser.LastName));
                customLocalUsers.Claims.Add(new Claim(Constants.ClaimTypes.Address, localUser.Location));
                customLocalUsers.Claims.Add(new Claim(Constants.ClaimTypes.BirthDate, localUser.DateOfBirth));
                foreach (var role in localUser.Roles)
                {
                    customLocalUsers.Claims.Add(new Claim(Constants.ClaimTypes.Role, role));
                }
                IncidereUsers.Add(customLocalUsers);
            }
        }
    }

    public class CustomLocalUser : LocalUser
    {
        public List<Claim> Claims { get; set; } = new List<Claim>();
    }
}