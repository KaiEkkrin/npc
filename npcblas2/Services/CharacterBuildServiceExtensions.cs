using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using npcblas2.Models;

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
                await fileStorageService.WriteAsync(user, fileName, ms);
            }
        }

        public static async Task<ImportResult> ImportFromFileAsync(this ICharacterBuildService characterBuildService, ClaimsPrincipal user, IFileStorageService fileStorageService, StoredFile file)
        {
            var stream = await fileStorageService.ReadAsync(user, file);
            if (stream == null)
            {
                return null; // Error should already have been reported via toast service
            }

            using (stream)
            {
                return await characterBuildService.ImportJsonAsync(user, stream);
            }
        }
    }
}