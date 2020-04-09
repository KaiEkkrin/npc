using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Blazored.Toast.Services;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Logging;
using Npc;
using Npc.Tests;
using npcblas2;
using npcblas2.Data;
using npcblas2.Models;
using npcblas2.Services;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace npcblas2.Tests
{
    /// <summary>
    /// Tests the character build service, using an in-memory database.
    /// </summary>
    public sealed class CharacterBuildServiceTests
    {
        private const string ValidUserName1 = "alice@example.com";
        private const string ValidUserName2 = "bob@example.com";
        private const string ValidUserHandle1 = "Alice";
        private const string ValidUserHandle2 = "Bob";

        private readonly string databaseId = Guid.NewGuid().ToString();
        private readonly string validUserId1 = Guid.NewGuid().ToString();
        private readonly string validUserId2 = Guid.NewGuid().ToString();

        private readonly DbContextOptions<ApplicationDbContext> dbOptions;
        private readonly IBuildDriver buildDriver;
        private readonly ILogger<CharacterBuildService> logger;
        private readonly IMapper mapper;
        private readonly IToastService toastService = Substitute.For<IToastService>();
        private readonly IUserManager userManager = Substitute.For<IUserManager>();

        public CharacterBuildServiceTests(ITestOutputHelper output)
        {
            // Create an in-memory database to test against
            dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseId)
                .Options;

            // Set up AutoMapper like our Startup does
            var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
            mapper = mappingConfig.CreateMapper();

            // Set up our valid users.  User 1 is admin.
            User1 = CreateTestUser(validUserId1, ValidUserName1, ValidUserHandle1, true);
            User2 = CreateTestUser(validUserId2, ValidUserName2, ValidUserHandle2, false);

            // This build driver creates build abstractions that accept any choice and
            // summarise by concatenating the choices.
            buildDriver = new TestBuildDriver(output);

            // This logger should log to the test output:
            logger = output.BuildLoggerFor<CharacterBuildService>();
        }

        private TestUser User1 { get; }

        private TestUser User2 { get; }

        [Fact]
        public Task TestGetNoCharacters() => TestServiceAsync(async service =>
        {
            var all = await service.GetAllAsync(User1.Principal);
            all.Should().BeEmpty();

            all = await service.GetAllAsync(User2.Principal);
            all.Should().BeEmpty();

            all = await service.GetPublicAsync();
            all.Should().BeEmpty();

            var one = await service.GetAsync(User1.Principal, Guid.NewGuid().ToString());
            one.Should().BeNull();

            var count = await service.GetCountAsync(User1.Principal);
            count.Should().Be(0);

            count = await service.GetCountAsync(User2.Principal);
            count.Should().Be(0);
        });

        [Fact]
        public async Task TestBuildThenGetCharacters()
        {
            // Build some characters:
            await TestServiceAsync(async service =>
            {
                await AddAndBuildAsync(service, User1.Principal, "Thrud", 1);
                await AddAndBuildAsync(service, User2.Principal, "Grunt", 1);
                await AddAndBuildAsync(service, User1.Principal, "Ug", 2, "Barbarian");
                await AddAndBuildAsync(service, User2.Principal, "Frunk", 3, "Dwarf", "Barbarian", "Fighter");
            });

            // Each user should see their own builds:
            var aliceAll = await TestServiceAsync(service => service.GetAllAsync(User1.Principal));
            var bobAll = await TestServiceAsync(service => service.GetAllAsync(User2.Principal));

            aliceAll.Should().HaveCount(2);
            aliceAll.Should().Contain(m => m.Build.Name == "Thrud" && m.Build.Level == 1);
            aliceAll.Should().Contain(m => m.Build.Name == "Ug" && m.Build.Level == 2);

            bobAll.Should().HaveCount(2);
            bobAll.Should().Contain(m => m.Build.Name == "Grunt" && m.Build.Level == 1);
            bobAll.Should().Contain(m => m.Build.Name == "Frunk" && m.Build.Level == 3);

            // We can take a deep dive on a build:
            await TestServiceAsync(async service =>
            {
                var frunkId = bobAll.First(m => m.Build.Name == "Frunk").Build.Id;
                var frunkModel = await service.GetAsync(User2.Principal, frunkId.ToString());

                frunkModel.Build.Id.Should().Be(frunkId);
                frunkModel.Build.Name.Should().Be("Frunk");
                frunkModel.Build.Level.Should().Be(3);
                frunkModel.Build.Summary.Should().Be("Dwarf Barbarian Fighter");
            });
        }

        private TestUser CreateTestUser(string userId, string userName, string handle, bool? isAdmin)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.AuthenticationMethod, "amr"),
                    new Claim(ApplicationClaimType.Handle, handle),
                };
            if (isAdmin == true)
            {
                claims.Add(new Claim(ApplicationClaimType.Permission, ApplicationClaimValue.Admin));
            }

            var testUser = new TestUser { Principal = new ClaimsPrincipal(new ClaimsIdentity(
                claims, ClaimTypes.AuthenticationMethod, ClaimTypes.Name, ClaimTypes.Role
            )), Record = Substitute.For<IApplicationUser>() };

            testUser.Record.Handle.Returns(handle);
            testUser.Record.IsAdmin.Returns(isAdmin);
            userManager.FindByIdAsync(userId).Returns(testUser.Record);
            return testUser;
        }

        // Note that add and build operations need to be done in the same context to preserve
        // tracking:
        private async Task<CharacterBuildModel> AddAndBuildAsync(CharacterBuildService service, ClaimsPrincipal user, string name, int level, params string[] choices)
        {
            var model = await service.AddAsync(user, new NewCharacterModel { Name = name, Level = level });
            model.Should().NotBeNull();
            return await BuildAsync(service, user, model, choices);
        }

        private async Task<CharacterBuildModel> BuildAsync(CharacterBuildService service, ClaimsPrincipal user, CharacterBuildModel model, params string[] choices)
        {
            if (choices.Length == 0)
            {
                return model;
            }

            var newModel = await service.BuildAsync(user, model, choices[0]);
            newModel.Should().NotBeNull();
            return await BuildAsync(service, user, newModel, choices.Skip(1).ToArray());
        }

        private async Task TestServiceAsync(Func<CharacterBuildService, Task> action)
        {
            using (var context = new ApplicationDbContext(dbOptions))
            {
                var service = new CharacterBuildService(buildDriver, context, logger, mapper, toastService, userManager);
                await action(service);
            }
        }

        private async Task<T> TestServiceAsync<T>(Func<CharacterBuildService, Task<T>> func)
        {
            using (var context = new ApplicationDbContext(dbOptions))
            {
                var service = new CharacterBuildService(buildDriver, context, logger, mapper, toastService, userManager);
                return await func(service);
            }
        }

        private class TestUser
        {
            public ClaimsPrincipal Principal { get; set; }

            public IApplicationUser Record { get; set; }
        }
    }
}