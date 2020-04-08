using System.Collections.Generic;
using System.IO;
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
        /// </summary>
        Task<IEnumerable<StoredFile>> GetFilesAsync();

        /// <summary>
        /// Reads a file from the service.
        /// </summary>
        Task ReadAsync(string fileName, Stream contents);

        /// <summary>
        /// Writes a file to the service.
        /// </summary>
        Task WriteAsync(string fileName, Stream contents);
    }
}