using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npc;
using npcblas2.Data;
using npcblas2.Models;

namespace npcblas2.Services
{
    public class CharacterBuildService : ICharacterBuildService
    {
        private readonly IBuildDriver buildDriver;
        private readonly ApplicationDbContext context;
        private readonly ILogger<CharacterBuildService> logger;
        private readonly IToastService toastService;

        public CharacterBuildService(IBuildDriver buildDriver, ApplicationDbContext context, ILogger<CharacterBuildService> logger, IToastService toastService)
            => (this.buildDriver, this.context, this.logger, this.toastService) = (buildDriver, context, logger, toastService);

        /// <inheritdoc />
        public async Task<CharacterBuildModel> AddAsync(ClaimsPrincipal user, NewCharacterModel model)
        {
            try
            {
                var buildOutput = buildDriver.Create(model.Name, model.Level);
                var build = new CharacterBuild
                {
                    Id = Guid.NewGuid(),
                    UserId = GetUserId(user),
                    CreationDateTime = DateTime.UtcNow,
                    Name = model.Name,
                    Level = model.Level,
                    Version = CharacterBuild.CurrentVersion,
                    Choices = new List<Choice>()
                };

                await context.CharacterBuilds.AddAsync(build);
                await context.SaveChangesAsync();
                return new CharacterBuildModel { Build = build, BuildOutput = buildOutput };
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
                // TODO Cache this build output here
                model.BuildOutput = buildDriver.Continue(model.BuildOutput, choice);

                if (model.Build.UserId != GetUserId(user))
                {
                    throw new InvalidOperationException("User id doesn't match");
                }

                var lastChoice = model.Build.Choices.OrderByDescending(ch => ch.Order).FirstOrDefault();
                var thisChoice = new Choice { CharacterBuildId = model.Build.Id, Order = lastChoice?.Order + 1 ?? 0, Value = choice };
                model.Build.Choices.Add(thisChoice);
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to build {model?.Build?.Name} ({model?.Build?.Id}) for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CharacterBuild>> GetAllAsync(ClaimsPrincipal user)
        {
            try
            {
                var userId = GetUserId(user);
                return await context.CharacterBuilds.Where(b => b.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to get all for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return Enumerable.Empty<CharacterBuild>();
            }
        }

        /// <inheritdoc />
        public async Task<CharacterBuildModel> GetAsync(ClaimsPrincipal user, string id)
        {
            try
            {
                var guid = Guid.Parse(id);
                var userId = GetUserId(user);
                var build = await context.CharacterBuilds.Where(b => b.UserId == userId && b.Id == guid)
                    .Include(b => b.Choices)
                    .FirstAsync();

                // TODO Retrieve the build output from a cache here if possible
                var start = buildDriver.Create(build.Name, build.Level);
                var buildOutput = buildDriver.Construct(start, build.Choices.OrderBy(ch => ch.Order).Select(ch => ch.Value));
                return new CharacterBuildModel { Build = build, BuildOutput = buildOutput };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to get {id} for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task RemoveAsync(ClaimsPrincipal user, CharacterBuild model)
        {
            try
            {
                var userId = GetUserId(user);
                var build = new CharacterBuild { Id = model.Id, UserId = userId };
                context.Remove(build);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to remove {model?.Name} ({model?.Id}) for {user?.Identity?.Name} : {ex.Message}");
                toastService.ShowError(ex.Message);
            }
        }

        private static string GetUserId(ClaimsPrincipal user) => user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}
