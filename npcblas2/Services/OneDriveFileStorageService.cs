using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using npcblas2.Models;

namespace npcblas2.Services
{
    public class OneDriveFileStorageService : IFileStorageService
    {
        private readonly GraphServiceClient graphServiceClient;
        private readonly ILogger<OneDriveFileStorageService> logger;
        private readonly IToastService toastService;

        public OneDriveFileStorageService(IHttpContextAccessor httpContextAccessor, ILogger<OneDriveFileStorageService> logger, IToastService toastService)
        {
            graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider(async requestMessage =>
            {
                var accessToken = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }));
            
            this.logger = logger;
            this.toastService = toastService;
        }

        /// <inheritdoc />
        public async Task<(bool isAuthorized, IEnumerable<StoredFile> files)> GetFilesAsync(ClaimsPrincipal user)
        {
            try
            {
                var result = await graphServiceClient.Me.Drive.Root.Children.Request()
                    .Select("Id,Name,File")
                    .GetAsync();
                return (true, result.Where(i => i.File != null)
                    .Select(i => new StoredFile { Id = i.Id, Name = i.Name }));
            }
            catch (ServiceException ex)
            {
                // We expect this, if the user isn't a OneDrive user.
                logger.LogError($"Failed to get OneDrive files for {user?.Identity?.Name} : {ex.Message}");
                return (false, null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to get OneDrive files for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return (true, null);
            }
        }

        /// <inheritdoc />
        public async Task<Stream> ReadAsync(ClaimsPrincipal user, StoredFile file)
        {
            try
            {
                return await graphServiceClient.Me.Drive.Items[file.Id].Content.Request().GetAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to read OneDrive file {file.Name} for user {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<bool> WriteAsync(ClaimsPrincipal user, string fileName, Stream contents)
        {
            try
            {
                var uploadSession = await graphServiceClient.Me.Drive.Root.ItemWithPath(fileName).CreateUploadSession().Request().PostAsync();

                // TODO Provide progress details here?
                var uploadProvider = new ChunkedUploadProvider(uploadSession, graphServiceClient, contents);
                await uploadProvider.UploadAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to write OneDrive file {fileName} for user {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return false;
            }
        }
    }
}