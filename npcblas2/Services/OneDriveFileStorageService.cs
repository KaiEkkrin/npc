using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Graph;
using npcblas2.Models;

namespace npcblas2.Services
{
    public class OneDriveFileStorageService : IFileStorageService
    {
        private readonly GraphServiceClient graphServiceClient;

        public OneDriveFileStorageService(IHttpContextAccessor httpContextAccessor)
        {
            graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider(async requestMessage =>
            {
                var accessToken = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<StoredFile>> GetFilesAsync()
        {
            var result = await graphServiceClient.Me.Drive.Root.Children.Request()
                .Select("Id,Name,File")
                .GetAsync();
            return result.Where(i => i.File != null)
                .Select(i => new StoredFile { Id = i.Id, Name = i.Name });
        }

        /// <inheritdoc />
        public async Task ReadAsync(string fileName, Stream contents)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task WriteAsync(string fileName, Stream contents)
        {
            var uploadSession = await graphServiceClient.Me.Drive.Root.ItemWithPath(fileName).CreateUploadSession().Request().PostAsync();

            // TODO Provide progress details here?
            var uploadProvider = new ChunkedUploadProvider(uploadSession, graphServiceClient, contents);
            await uploadProvider.UploadAsync();
        }
    }
}