using Microsoft.AspNetCore.Identity;

namespace npcblas2.Data
{
    /// <summary>
    /// Our application user record.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// This handle should be unique per user and is shown publically.
        /// </summary>
        public string Handle { get; set; }
    }
}