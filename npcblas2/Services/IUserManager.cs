using System.Threading.Tasks;
using npcblas2.Data;

namespace npcblas2.Services
{
    /// <summary>
    /// Provides an abstraction around the user manager stuff so that I can run unit tests
    /// with a database context without any users.
    /// </summary>
    public interface IUserManager
    {
        /// <summary>
        /// Looks up a user by id.
        /// </summary>
        Task<IApplicationUser> FindByIdAsync(string userId);
    }
}