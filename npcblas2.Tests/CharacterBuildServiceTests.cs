using System.Threading.Tasks.Dataflow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Blazored.Toast.Services;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    /// Tests the character build service.
    /// We assume that the Cosmos DB emulator is running; just like with the real
    /// application, you'll need to configure user-secrets "Cosmos:Uri" and
    /// "Cosmos:Key" in order for us to connect successfully.  We'll create a
    /// random database to test into.
    /// TODO: test the public flag, and any more edge cases of builds/exports/imports
    /// that I can think of.
    /// </summary>
    public sealed class CharacterBuildServiceTests : IDisposable
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
            // Configure and load our user secrets
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<CharacterBuildServiceTests>();
            Configuration = builder.Build();

            // Create a temporary database to test against
            dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseCosmos(
                    accountEndpoint: Configuration["Cosmos:Uri"],
                    accountKey: Configuration["Cosmos:Key"],
                    databaseName: $"Test-{Guid.NewGuid()}"
                )
                .Options;

            using (var context = new ApplicationDbContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

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

        private IConfiguration Configuration { get; }

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

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task TestBuildThenGetCharacters(bool exportAsAlice, bool importAsAlice)
        {
            // Build some characters:
            await TestServiceAsync(async service =>
            {
                await AddAndBuildAsync(service, User1.Principal, "Thrud", 1);
                await AddAndBuildAsync(service, User1.Principal, "Grug", 1);
                await AddAndBuildAsync(service, User2.Principal, "Grunt", 1);
                await AddAndBuildAsync(service, User1.Principal, "Ug", 2, "Barbarian");
                await AddAndBuildAsync(service, User2.Principal, "Frunk", 3, "Dwarf", "Barbarian", "Fighter");
            });

            // Each user should see their own builds:
            var aliceAll = await TestServiceAsync(service => service.GetAllAsync(User1.Principal));
            var bobAll = await TestServiceAsync(service => service.GetAllAsync(User2.Principal));

            aliceAll.Should().HaveCount(3);
            aliceAll.Should().Contain(m => m.Build.Name == "Thrud" && m.Build.Level == 1 && m.Build.Summary == string.Empty);
            aliceAll.Should().Contain(m => m.Build.Name == "Grug" && m.Build.Level == 1 && m.Build.Summary == string.Empty);
            aliceAll.Should().Contain(m => m.Build.Name == "Ug" && m.Build.Level == 2 && m.Build.Summary == "Barbarian");

            bobAll.Should().HaveCount(2);
            bobAll.Should().Contain(m => m.Build.Name == "Grunt" && m.Build.Level == 1 && m.Build.Summary == string.Empty);
            bobAll.Should().Contain(m => m.Build.Name == "Frunk" && m.Build.Level == 3 && m.Build.Summary == "Dwarf Barbarian Fighter");

            // ...with the correct counts too:
            var aliceCount = await TestServiceAsync(service => service.GetCountAsync(User1.Principal));
            var bobCount = await TestServiceAsync(service => service.GetCountAsync(User2.Principal));

            aliceCount.Should().Be(3);
            bobCount.Should().Be(2);

            // We can take a deep dive on a build and make some changes, extracting an
            // export before the changes happened:
            var thrudId = aliceAll.First(m => m.Build.Name == "Thrud").Build.Id;
            var grugId = aliceAll.First(m => m.Build.Name == "Grug").Build.Id;
            var frunkId = bobAll.First(m => m.Build.Name == "Frunk").Build.Id;
            var gruntId = bobAll.First(m => m.Build.Name == "Grunt").Build.Id;
            var exportStream = new MemoryStream();
            await TestServiceAsync(async service =>
            {
                var thrud = await service.GetAsync(User1.Principal, thrudId.ToString());
                VerifyBuild(thrud, "Thrud", 1);

                // Extending Thrud will only be exported if Alice does the exporting
                await BuildAsync(service, User1.Principal, thrud, "Orc");

                var frunk = await service.GetAsync(User2.Principal, frunkId.ToString());
                VerifyBuild(frunk, "Frunk", 3, "Dwarf", "Barbarian", "Fighter");

                await service.ExportJsonAsync(exportAsAlice ? User1.Principal : User2.Principal, exportStream);

                // We extend Frunk and Grug, and delete Grunt and Thrud:
                await service.RemoveAsync(User1.Principal, thrud.Build);

                var grunt = await service.GetAsync(User2.Principal, gruntId.ToString());
                VerifyBuild(grunt, "Grunt", 1);
                await service.RemoveAsync(User2.Principal, grunt.Build);

                frunk = await BuildAsync(service, User2.Principal, frunk, "Fast", "Angry");
                VerifyBuild(frunk, "Frunk", 3, "Dwarf", "Barbarian", "Fighter", "Fast", "Angry");

                var grug = await service.GetAsync(User1.Principal, grugId.ToString());
                grug = await BuildAsync(service, User1.Principal, grug, "Goblin");
                VerifyBuild(grug, "Grug", 1, "Goblin");
            });

            // If we re-import that stream, things should go back to how they were --
            // excepting import permission; Alice is an admin and so can import all.
            // Bob is not and can only import his characters.
            await TestServiceAsync(async service =>
            {
                var grunt = await service.GetAsync(User2.Principal, gruntId.ToString());
                grunt.Should().BeNull();

                var frunk = await service.GetAsync(User2.Principal, frunkId.ToString());
                VerifyBuild(frunk, "Frunk", 3, "Dwarf", "Barbarian", "Fighter", "Fast", "Angry");

                exportStream.Seek(0, SeekOrigin.Begin);
                var result = await service.ImportJsonAsync(importAsAlice ? User1.Principal : User2.Principal, exportStream);
                result.NumberAdded.Should().Be(exportAsAlice ? 2 : 1);
                result.NumberRejected.Should().Be(exportAsAlice && !importAsAlice ? 2 : 0);

                // If we exported and imported as Alice, then Alice should have a
                // record for "Thrud":
                var thrud = await service.GetAsync(User1.Principal, thrudId.ToString());
                if (importAsAlice)
                {
                    VerifyBuild(thrud, "Thrud", 1, "Orc");
                }
                else
                {
                    thrud.Should().BeNull();
                }

                // If we exported as Alice and imported as Bob, then because there is no
                // "Thrud" in the database before import, the user id should be squashed
                // and *Bob* should have a record for "Thrud":
                thrud = await service.GetAsync(User2.Principal, thrudId.ToString());
                if (exportAsAlice && !importAsAlice)
                {
                    VerifyBuild(thrud, "Thrud", 1, "Orc");
                }
                else
                {
                    thrud.Should().BeNull();
                }

                grunt = await service.GetAsync(User2.Principal, gruntId.ToString());
                VerifyBuild(grunt, "Grunt", 1);

                frunk = await service.GetAsync(User2.Principal, frunkId.ToString());
                VerifyBuild(frunk, "Frunk", 3, "Dwarf", "Barbarian", "Fighter");

                // If we imported as Alice, Grug should be back to having no choices;
                // otherwise it should retain the Goblin choice, there should be no
                // squashed Grug for Bob after import as Bob
                var grug = await service.GetAsync(User1.Principal, grugId.ToString());
                if (exportAsAlice && importAsAlice)
                {
                    VerifyBuild(grug, "Grug", 1);
                }
                else
                {
                    VerifyBuild(grug, "Grug", 1, "Goblin");
                }

                grug = await service.GetAsync(User2.Principal, grugId.ToString());
                grug.Should().BeNull();
            });
        }

        public void Dispose()
        {
            using (var context = new ApplicationDbContext(dbOptions))
            {
                context.Database.EnsureDeleted();
            }
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

        /// <summary>
        /// Checks the build matches what would be emitted by our test build driver
        /// </summary>
        private void VerifyBuild(CharacterBuildModel build, string name, int level, params string[] choices)
        {
            build.Build.Name.Should().Be(name);
            build.Build.Level.Should().Be(level);
            build.Build.Summary.Should().Be(string.Join(" ", choices));

            var sheet = build.BuildOutput.CreateCharacterSheet().ToList();
            sheet.Should().HaveCount(choices.Length + 1);

            var basics = sheet[0].Items.ToList();
            basics.Should().Contain(l => l.Item1 == "Name" && l.Item2 == name);
            basics.Should().Contain(l => l.Item1 == "Level" && l.Item2 == $"{level}");

            for (int i = 0; i < choices.Length; ++i)
            {
                sheet[i + 1].Items.ToList().Should().HaveCount(1);
                var entry = sheet[i + 1].Items[0];
                entry.Item1.Should().Be("Option");
                entry.Item2.Should().Be(choices[i]);
            }
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