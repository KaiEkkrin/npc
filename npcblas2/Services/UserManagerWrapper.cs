using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using npcblas2.Data;

namespace npcblas2.Services
{
    public class UserManagerWrapper : IUserManager
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserManagerWrapper(UserManager<ApplicationUser> userManager) => this.userManager = userManager;

        public async Task<IApplicationUser> FindByIdAsync(string userId) => await userManager.FindByIdAsync(userId);
    }
}