using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace npcblas2.Data
{
    /// <summary>
    /// Our application user record.
    /// </summary>
    /// <remarks>
    /// We associate our own permission system with this record rather than the real IdentityRole support
    /// because it appears to be broken when using the Cosmos DB provider for Entity Framework Core.
    /// </remarks>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// This handle should be unique per user and is shown publically.
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// If true, this user has the Admin permission.
        /// </summary>
        public bool? IsAdmin { get; set; }
    }
}