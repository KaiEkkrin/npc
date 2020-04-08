using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace npcblas2.Services
{
    /// <summary>
    /// Helpful extension methods for the character build service.
    /// </summary>
    public static class CharacterBuildServiceExtensions
    {
        public static async Task ExportToFileAsync(this ICharacterBuildService characterBuildService, ClaimsPrincipal user, IFileStorageService fileStorageService, string fileName)
        {
            // TODO With the MemoryStream we'll end up holding everything in memory at once.
            // For larger exports, instead connect some pipes together to do this?
            // Also emit status at regular intervals :)
            using (var ms = new MemoryStream())
            {
                await characterBuildService.ExportJsonAsync(user, ms);
                ms.Seek(0, SeekOrigin.Begin);
                await fileStorageService.WriteAsync(fileName, ms);
            }
        }

        public static async Task ImportFromFileAsync(this ICharacterBuildService characterBuildService, ClaimsPrincipal user, IFileStorageService fileStorageService, string fileName)
        {
            // TODO as above.
            using (var ms = new MemoryStream())
            {
                await fileStorageService.ReadAsync(fileName, ms);
                ms.Seek(0, SeekOrigin.Begin);
                await characterBuildService.ImportJsonAsync(user, ms);
            }
        }
    }
}