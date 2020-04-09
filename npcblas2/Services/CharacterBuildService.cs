using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Blazored.Toast.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npc;
using npcblas2.Data;
using npcblas2.Models;

namespace npcblas2.Services
{
    public class CharacterBuildService : ICharacterBuildService
    {
        private const int MaximumNumberOfCharactersPerUser = 100;

        private readonly IBuildDriver buildDriver;
        private readonly ApplicationDbContext context;
        private readonly ILogger<CharacterBuildService> logger;
        private readonly IMapper mapper;
        private readonly IToastService toastService;
        private readonly IUserManager userManager;

        public CharacterBuildService(IBuildDriver buildDriver, ApplicationDbContext context, ILogger<CharacterBuildService> logger, IMapper mapper, IToastService toastService, IUserManager userManager)
            => (this.buildDriver, this.context, this.logger, this.mapper, this.toastService, this.userManager) = (buildDriver, context, logger, mapper, toastService, userManager);

        /// <inheritdoc />
        public async Task<CharacterBuildModel> AddAsync(ClaimsPrincipal user, NewCharacterModel model)
        {
            try
            {
                var userId = user.GetUserId();
                await EnsureNotOverCharacterCountCap(userId);

                var buildOutput = buildDriver.Create(model.Name, model.Level);
                var build = new CharacterBuild
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreationDateTime = DateTime.UtcNow,
                    Name = model.Name,
                    Level = model.Level,
                    Summary = buildOutput.Summarise(),
                    Version = CharacterBuild.CurrentVersion,
                    Choices = new List<Choice>()
                };

                await context.CharacterBuilds.AddAsync(build);
                await context.SaveChangesAsync();
                return new CharacterBuildModel { Build = mapper.Map<CharacterBuild, CharacterBuildDto>(build), BuildOutput = buildOutput, CanEdit = true };
            }
            catch (CharacterBuildException cbe)
            {
                toastService.ShowError(cbe.Message);
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to add {model?.Name} for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<CharacterBuildModel> BuildAsync(ClaimsPrincipal user, CharacterBuildModel model, string choice)
        {
            try
            {
                var build = await GetBuildEntityAsync(user, model.Build.Id);
                var buildOutput = model.BuildOutput.Continue(choice);
                var lastChoice = build.Choices.OrderByDescending(ch => ch.Order).FirstOrDefault();
                var thisChoice = new Choice { CharacterBuildId = build.Id, Order = lastChoice?.Order + 1 ?? 0, Value = choice };
                build.Choices.Add(thisChoice);
                build.Summary = model.BuildOutput.Summarise();
                await context.SaveChangesAsync();
                return new CharacterBuildModel { Build = mapper.Map<CharacterBuild, CharacterBuildDto>(build), BuildOutput = buildOutput, CanEdit = true };
            }
            catch (CharacterBuildException cbe)
            {
                toastService.ShowError(cbe.Message);
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to build {model?.Build?.Name} ({model?.Build?.Id}) for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<List<CharacterBuildSummary>> GetAllAsync(ClaimsPrincipal user)
        {
            try
            {
                var userId = user.GetUserId();
                var handle = user.GetHandle();
                var builds = await context.CharacterBuilds.Where(b => b.UserId == userId)
                    .ToListAsync();
                return builds.Select(b => new CharacterBuildSummary
                {
                    Build = mapper.Map<CharacterBuild, CharacterBuildDto>(b),
                    Handle = handle
                }).ToList();
            }
            catch (CharacterBuildException cbe)
            {
                toastService.ShowError(cbe.Message);
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to get all for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<List<CharacterBuildSummary>> GetPublicAsync()
        {
            try
            {
                var builds = await context.CharacterBuilds.Where(b => b.IsPublic == true).ToListAsync();

                // Associate each build with the handle that created it -- we can't do this
                // via relations, because we're not actually using a relational database :)
                // We need to be careful here to not write in any null values
                string GetUserId(CharacterBuild build) => build.UserId ?? string.Empty;
                var handleById = new Dictionary<string, string>();
                foreach (var g in builds.GroupBy(b => GetUserId(b)))
                {
                    var user = await userManager.FindByIdAsync(g.Key);
                    handleById[g.Key] = user?.Handle ?? string.Empty;
                }

                return builds.Select(b => new CharacterBuildSummary
                {
                    Build = mapper.Map<CharacterBuild, CharacterBuildDto>(b),
                    Handle = handleById[GetUserId(b)]
                }).ToList();
            }
            catch (CharacterBuildException cbe)
            {
                toastService.ShowError(cbe.Message);
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to get public : {ex.Message}");
                toastService.ShowError(ex.Message);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<CharacterBuildModel> GetAsync(ClaimsPrincipal user, string id)
        {
            try
            {
                var guid = Guid.Parse(id);
                var userId = user.GetUserId();
                var build = await context.CharacterBuilds.Where(b => b.Id == guid && (b.UserId == userId || b.IsPublic == true))
                    .Include(b => b.Choices)
                    .FirstOrDefaultAsync();
                if (build == null)
                {
                    return null;
                }

                var buildOutput = build.Choices.OrderBy(ch => ch.Order)
                    .Aggregate(buildDriver.Create(build.Name, build.Level), (b, ch) => b.Continue(ch.Value));
                return new CharacterBuildModel
                {
                    Build = mapper.Map<CharacterBuild, CharacterBuildDto>(build),
                    BuildOutput = buildOutput,
                    CanEdit = build.UserId == userId
                };
            }
            catch (CharacterBuildException cbe)
            {
                toastService.ShowError(cbe.Message);
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to get {id} for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<int> GetCountAsync(ClaimsPrincipal user)
        {
            try
            {
                var userId = user.GetUserId();
                return await GetCountAsync(userId);
            }
            catch (CharacterBuildException cbe)
            {
                toastService.ShowError(cbe.Message);
                return 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to get character count for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return 0;
            }
        }

        /// <inheritdoc />
        public int GetMaximumCount() => MaximumNumberOfCharactersPerUser;

        /// <inheritdoc />
        public async Task<bool> RemoveAsync(ClaimsPrincipal user, Guid id)
        {
            try
            {
                var build = await GetBuildEntityAsync(user, id);
                context.Remove(build);
                return (await context.SaveChangesAsync()) > 0;
            }
            catch (CharacterBuildException cbe)
            {
                toastService.ShowError(cbe.Message);
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to remove {id} for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> SetPublicAsync(ClaimsPrincipal user, Guid id, bool isPublic)
        {
            try
            {
                var build = await GetBuildEntityAsync(user, id);
                build.IsPublic = isPublic;
                return (await context.SaveChangesAsync()) > 0;
            }
            catch (CharacterBuildException cbe)
            {
                toastService.ShowError(cbe.Message);
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to update {id} for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task ExportJsonAsync(ClaimsPrincipal user, Stream stream)
        {
            try
            {
                var userId = user.GetUserId();
                var query = user.IsAdmin() ? context.CharacterBuilds : context.CharacterBuilds.Where(b => b.UserId == userId);

                // TODO This will get too large.  Stream them in batches, rather than fetching them
                // all in one go.
                var all = await query.Include(b => b.Choices).AsNoTracking().ToListAsync();
                var serializer = new JsonSerializer();
                var writer = new JsonTextWriter(new StreamWriter(stream));
                serializer.Serialize(writer, all.Select(mapper.Map<CharacterBuild, CharacterBuildDto>));
                await writer.FlushAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to export charcters for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
            }
        }

        /// <inheritdoc />
        public async Task<ImportResult> ImportJsonAsync(ClaimsPrincipal user, Stream stream)
        {
            try
            {
                var result = new ImportResult();

                var userId = user.GetUserId();
                var isAdmin = user.IsAdmin();
                var serializer = new JsonSerializer();
                using (var sr = new StreamReader(stream))
                using (var reader = new JsonTextReader(sr))
                {
                    // TODO Can I make it read these in batches rather than in one go?
                    // (maybe use the Utf8JsonReader from System.Text.Json instead?  It's more complicated though)
                    var all = serializer.Deserialize<IEnumerable<CharacterBuildDto>>(reader);
                    foreach (var build in all)
                    {
                        if (build.UserId != userId && !isAdmin)
                        {
                            // Squash non-admin user IDs to the ID of the user doing the importing
                            build.UserId = userId;
                        }

                        // Fetch any existing record to see if we need to update it:
                        var existing = await context.CharacterBuilds.Where(b => b.Id == build.Id)
                            .Include(b => b.Choices)
                            .FirstOrDefaultAsync();
                        if (existing != null)
                        {
                            if (existing.UserId != userId && !isAdmin)
                            {
                                // We can't update this record, it's not ours
                                ++result.NumberRejected;
                            }
                            else
                            {
                                mapper.Map(build, existing);
                                ++result.NumberUpdated;
                            }
                        }
                        else
                        {
                            await context.CharacterBuilds.AddAsync(mapper.Map<CharacterBuildDto, CharacterBuild>(build));
                            ++result.NumberAdded;
                        }
                    }
                }

                await context.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to import charcters for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return null;
            }
        }

        private Task<CharacterBuild> GetBuildEntityAsync(ClaimsPrincipal user, Guid id)
        {
            var userId = user.GetUserId();
            return context.CharacterBuilds.Where(b => b.Id == id && b.UserId == userId)
                .Include(b => b.Choices)
                .FirstAsync();
        }

        private Task<int> GetCountAsync(string userId) => context.CharacterBuilds.Where(b => b.UserId == userId).CountAsync();

        private async Task EnsureNotOverCharacterCountCap(string userId)
        {
            var characterCount = await GetCountAsync(userId);
            if (characterCount >= MaximumNumberOfCharactersPerUser)
            {
                throw new CharacterBuildException("You have too many characters.");
            }
        }
    }
}
