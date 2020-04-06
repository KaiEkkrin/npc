using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using npcblas2.Data;
using npcblas2.Models;

namespace npcblas2.Services
{
    /// <summary>
    /// An override ClaimsPrincipal factory that adds some custom claims for ourselves.
    /// See https://korzh.com/blogs/dotnet-stories/add-extra-user-claims-aspnet-core-webapp
    /// </summary>
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            if (!string.IsNullOrWhiteSpace(user.Handle))
            {
                identity.AddClaim(new Claim(ApplicationClaimType.Handle, user.Handle));
            }

            if (user.IsAdmin == true)
            {
                identity.AddClaim(new Claim(ApplicationClaimType.Permission, ApplicationClaimValue.Admin));
            }

            return identity;
        }
    }
}