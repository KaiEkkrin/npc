using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        private readonly IBuildDriver buildDriver;
        private readonly ApplicationDbContext context;
        private readonly ILogger<CharacterBuildService> logger;

        public CharacterBuildService(IBuildDriver buildDriver, ApplicationDbContext context, ILogger<CharacterBuildService> logger)
            => (this.buildDriver, this.context, this.logger) = (buildDriver, context, logger);

        public async Task<CharacterBuildModel> AddAsync(ClaimsPrincipal user, NewCharacterModel model)
        {
            var buildOutput = buildDriver.Create(model.Name, model.Level);
            var buildModel = new CharacterBuildModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = GetUserId(user),
                CreationDateTime = DateTime.UtcNow,
                Name = model.Name,
                Level = model.Level,
                Version = CharacterBuild.CurrentVersion,
                BuildOutput = buildOutput
            };

            await context.CharacterBuilds.AddAsync(FromModel(buildModel));
            await context.SaveChangesAsync();
            return buildModel;
        }

        public async Task<CharacterBuildModel> BuildAsync(ClaimsPrincipal user, CharacterBuildModel model, int choice)
        {
            var buildOutput = buildDriver.Continue(model.BuildOutput, choice);
            model.BuildOutput = buildOutput;

            var userId = GetUserId(user);
            var build = await context.CharacterBuilds.Where(b => b.Id == model.Id && b.UserId == userId).FirstAsync();
            build.BuildOutput = buildDriver.Serialize(buildOutput);
            await context.SaveChangesAsync();
            return model;
        }

        public async Task<IEnumerable<CharacterBuildModel>> GetAllAsync(ClaimsPrincipal user)
        {
            var userId = GetUserId(user);
            var builds = await context.CharacterBuilds.Where(b => b.UserId == userId)
                .Select(b => new // hopefully this trick will stop it from fetching the serialized data
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    CreationDateTime = b.CreationDateTime,
                    Name = b.Name,
                    Level = b.Level,
                    Version = b.Version
                })
                .ToListAsync();

            return builds
                .Select(b => new CharacterBuildModel
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    CreationDateTime = b.CreationDateTime,
                    Name = b.Name,
                    Level = b.Level,
                    Version = b.Version
                });
        }

        public async Task<CharacterBuildModel> GetAsync(ClaimsPrincipal user, string id)
        {
            var userId = GetUserId(user);
            var build = await context.CharacterBuilds.Where(b => b.UserId == userId && b.Id == id)
                .FirstAsync();
            return ToModel(build);
        }

        public async Task RemoveAsync(ClaimsPrincipal user, CharacterBuildModel model)
        {
            var userId = GetUserId(user);
            var build = new CharacterBuild { Id = model.Id, UserId = userId };
            context.Remove(build);
            await context.SaveChangesAsync();
        }

        private static string GetUserId(ClaimsPrincipal user) => user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        private CharacterBuild FromModel(CharacterBuildModel model) => new CharacterBuild
        {
            Id = model.Id,
            UserId = model.UserId,
            CreationDateTime = model.CreationDateTime,
            Name = model.Name,
            Level = model.Level,
            Version = model.Version,
            BuildOutput = model.BuildOutput != null ? buildDriver.Serialize(model.BuildOutput) : null
        };

        private CharacterBuildModel ToModel(CharacterBuild build) => new CharacterBuildModel
        {
            Id = build.Id,
            UserId = build.UserId,
            CreationDateTime = build.CreationDateTime,
            Name = build.Name,
            Level = build.Level,
            Version = build.Version,
            BuildOutput = DeserializeBuildOutput(build)
        };

        private BuildOutput DeserializeBuildOutput(CharacterBuild build)
        {
            try
            {
                return buildDriver.Deserialize(build.BuildOutput);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to deserialize build output for {build.Name} ({build.Id}) : {ex.Message}");
                return null;
            }
        }
    }
}
