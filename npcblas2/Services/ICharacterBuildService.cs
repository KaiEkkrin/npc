using System.Collections.Generic;
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
        /// The build output won't be populated.
        /// </summary>
        Task<List<CharacterBuild>> GetAllAsync(ClaimsPrincipal user);

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
        Task<bool> RemoveAsync(ClaimsPrincipal user, CharacterBuild model);
    }
}