using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using npcblas2.Data;
using npcblas2.Models;

namespace npcblas2.Services
{
    /// <summary>
    /// Describes how to handle character builds.
    /// </summary>
    public interface ICharacterBuildService
    {
        /// <summary>
        /// Adds a new character.
        /// </summary>
        Task<CharacterBuildModel> AddAsync(ClaimsPrincipal user, NewCharacterModel model);

        /// <summary>
        /// Continues the build of the given character by choosing the given option.
        /// </summary>
        Task<CharacterBuildModel> BuildAsync(ClaimsPrincipal user, CharacterBuildModel model, string choice);

        /// <summary>
        /// Gets all builds for the given user.
        /// The user name fields will be empty.
        /// </summary>
        Task<List<CharacterBuildSummary>> GetAllAsync(ClaimsPrincipal user);

        /// <summary>
        /// Gets the public builds.
        /// </summary>
        Task<List<CharacterBuildSummary>> GetPublicAsync();

        /// <summary>
        /// Gets a character build.
        /// </summary>
        Task<CharacterBuildModel> GetAsync(ClaimsPrincipal user, string id);

        /// <summary>
        /// Gets the number of characters a user has.
        /// </summary>
        Task<int> GetCountAsync(ClaimsPrincipal user);

        /// <summary>
        /// Gets the maximum number of characters you're allowed.
        /// </summary>
        int GetMaximumCount();

        /// <summary>
        /// Removes the given character build.
        /// </summary>
        Task<bool> RemoveAsync(ClaimsPrincipal user, CharacterBuild build);

        /// <summary>
        /// Updates the database with any changes to the build record.  (For setting the public flag, etc.)
        /// </summary>
        Task<bool> UpdateAsync(ClaimsPrincipal user, CharacterBuild build);

        /// <summary>
        /// Exports the user's characters (or all characters for an admin) as JSON to the given stream.
        /// </summary>
        Task ExportJsonAsync(ClaimsPrincipal user, Stream stream);

        /// <summary>
        /// Imports characters from the given stream as JSON.
        /// </summary>
        /// <returns>An object describing what was imported or null if the import failed.</returns>
        Task<ImportResult> ImportJsonAsync(ClaimsPrincipal user, Stream stream);
    }
}