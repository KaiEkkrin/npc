using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly IToastService toastService;
        private readonly UserManager<IdentityUser> userManager;

        public CharacterBuildService(IBuildDriver buildDriver, ApplicationDbContext context, ILogger<CharacterBuildService> logger, IToastService toastService, UserManager<IdentityUser> userManager)
            => (this.buildDriver, this.context, this.logger, this.toastService, this.userManager) = (buildDriver, context, logger, toastService, userManager);

        /// <inheritdoc />
        public async Task<CharacterBuildModel> AddAsync(ClaimsPrincipal user, NewCharacterModel model)
        {
            try
            {
                var userId = GetUserId(user);
                await EnsureNotOverCharacterCountCap(userId);

                var buildOutput = buildDriver.Create(model.Name, model.Level);
                var build = new CharacterBuild
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreationDateTime = DateTime.UtcNow,
                    Name = model.Name,
                    Level = model.Level,
                    Summary = buildDriver.Summarise(buildOutput),
                    Version = CharacterBuild.CurrentVersion,
                    Choices = new List<Choice>()
                };

                await context.CharacterBuilds.AddAsync(build);
                await context.SaveChangesAsync();
                return new CharacterBuildModel { Build = build, BuildOutput = buildOutput, CanEdit = true };
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
                model.BuildOutput = buildDriver.Continue(model.BuildOutput, choice);

                if (model.Build.UserId != GetUserId(user))
                {
                    throw new InvalidOperationException("User id doesn't match");
                }

                var lastChoice = model.Build.Choices.OrderByDescending(ch => ch.Order).FirstOrDefault();
                var thisChoice = new Choice { CharacterBuildId = model.Build.Id, Order = lastChoice?.Order + 1 ?? 0, Value = choice };
                model.Build.Choices.Add(thisChoice);
                model.Build.Summary = buildDriver.Summarise(model.BuildOutput);
                await context.SaveChangesAsync();
                return model;
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
                var userId = GetUserId(user);
                return await context.CharacterBuilds.Where(b => b.UserId == userId)
                    .Select(b => new CharacterBuildSummary { Build = b, UserName = string.Empty })
                    .ToListAsync();
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

                // Associate each build with the username that created it -- we can't do this
                // via relations, because we're not actually using a relational database :)
                // We need to be careful here to not write in any null values
                string GetUserId(CharacterBuild build) => build.UserId ?? string.Empty;
                var userNameById = new Dictionary<string, string>();
                foreach (var g in builds.GroupBy(b => GetUserId(b)))
                {
                    var user = await userManager.FindByIdAsync(g.Key);
                    userNameById[g.Key] = user?.UserName ?? string.Empty;
                }

                return builds.Select(b => new CharacterBuildSummary { Build = b, UserName = userNameById[GetUserId(b)] }).ToList();
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
                var userId = GetUserId(user);
                var build = await context.CharacterBuilds.Where(b => b.Id == guid && (b.UserId == userId || b.IsPublic == true))
                    .Include(b => b.Choices)
                    .FirstOrDefaultAsync();
                if (build == null)
                {
                    return null;
                }

                var start = buildDriver.Create(build.Name, build.Level);
                var buildOutput = buildDriver.Construct(start, build.Choices.OrderBy(ch => ch.Order).Select(ch => ch.Value));
                return new CharacterBuildModel { Build = build, BuildOutput = buildOutput, CanEdit = build.UserId == userId };
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
                var userId = GetUserId(user);
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
        public async Task<bool> RemoveAsync(ClaimsPrincipal user, CharacterBuild build)
        {
            try
            {
                var userId = GetUserId(user);
                if (userId != build.UserId)
                {
                    return false;
                }

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
                logger.LogError(ex, $"Failed to remove {build?.Name} ({build?.Id}) for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateAsync(ClaimsPrincipal user, CharacterBuild build)
        {
            try
            {
                var userId = GetUserId(user);
                if (userId != build.UserId)
                {
                    return false;
                }

                return (await context.SaveChangesAsync()) > 0;
            }
            catch (CharacterBuildException cbe)
            {
                toastService.ShowError(cbe.Message);
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to update {build?.Name} ({build?.Id}) for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return false;
            }
        }

        private static string GetUserId(ClaimsPrincipal user) => user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

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
