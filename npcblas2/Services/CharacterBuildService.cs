using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public async Task<CharacterBuildModel> BuildAsync(ClaimsPrincipal user, CharacterBuildModel model, string choice)
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

        public async Task<IEnumerable<CharacterBuild>> GetAllAsync(ClaimsPrincipal user)
        {
            var userId = GetUserId(user);
            return await context.CharacterBuilds.Where(b => b.UserId == userId).ToListAsync();
        }

        public async Task<CharacterBuildModel> GetAsync(ClaimsPrincipal user, string id)
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

        public async Task RemoveAsync(ClaimsPrincipal user, CharacterBuild model)
        {
            var userId = GetUserId(user);
            var build = new CharacterBuild { Id = model.Id, UserId = userId };
            context.Remove(build);
            await context.SaveChangesAsync();
        }

        private static string GetUserId(ClaimsPrincipal user) => user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}
