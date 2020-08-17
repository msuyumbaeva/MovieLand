using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using MovieLand.BLL.Services;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieLand.IdentityServer
{
    public class ProfileService : IProfileService
    {
        protected AppUserManager mUserManager;

        public ProfileService(AppUserManager userManager) {
            mUserManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context) {
            AppUser user = await mUserManager.GetUserAsync(context.Subject);

            IList<string> roles = await mUserManager.GetRolesAsync(user);

            IList<Claim> roleClaims = new List<Claim>();
            foreach (string role in roles) {
                roleClaims.Add(new Claim(JwtClaimTypes.Role, role));
            }

            context.IssuedClaims.AddRange(roleClaims);
        }

        public Task IsActiveAsync(IsActiveContext context) {
            return Task.CompletedTask;
        }
    }
}
