using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using npcblas2.Models;

namespace npcblas2.Services
{
    /// <summary>
    /// Helps access a file storage service.
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Gets the files currently in the service.
        /// Also use this method to check whether the user is authorized to use file storage services.
        /// </summary>
        Task<(bool isAuthorized, IEnumerable<StoredFile> files)> GetFilesAsync(ClaimsPrincipal user);

        /// <summary>
        /// Reads a file from the service.
        /// </summary>
        Task<Stream> ReadAsync(ClaimsPrincipal user, StoredFile file);

        /// <summary>
        /// Writes a file to the service.
        /// </summary>
        Task<bool> WriteAsync(ClaimsPrincipal user, string fileName, Stream contents);
    }
}