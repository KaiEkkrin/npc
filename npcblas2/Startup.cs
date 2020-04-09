using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using Blazored.Modal;
using Blazored.Toast;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npc;
using npcblas2.Areas.Identity;
using npcblas2.Data;
using npcblas2.Services;

namespace npcblas2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseCosmos(
                    accountEndpoint: Configuration["Cosmos:Uri"],
                    accountKey: Configuration["Cosmos:Key"],
                    databaseName: Configuration["Cosmos:DatabaseName"]
                ));

            var authBuilder = services.AddAuthentication();

            var googleClientId = Configuration["Authentication:Google:ClientId"];
            var googleClientSecret = Configuration["Authentication:Google:ClientSecret"];
            if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
            {
                authBuilder = authBuilder.AddGoogle(options =>
                {
                    options.ClientId = googleClientId;
                    options.ClientSecret = googleClientSecret;
                });
            }

            var microsoftClientId = Configuration["Authentication:Microsoft:ClientId"];
            var microsoftClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
            if (!string.IsNullOrWhiteSpace(microsoftClientId) && !string.IsNullOrWhiteSpace(microsoftClientSecret))
            {
                authBuilder = authBuilder.AddMicrosoftAccount(options =>
                {
                    options.ClientId = microsoftClientId;
                    options.ClientSecret = microsoftClientSecret;

                    // For OneDrive access:
                    options.SaveTokens = true;
                    options.Scope.Add("Files.ReadWrite");
                    options.Scope.Add("offline_access");
                });
            }

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();
            services.AddRazorPages();
            services.AddServerSideBlazor();

            var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
            services.AddSingleton<IMapper>(_ => mappingConfig.CreateMapper());

            services.AddBlazoredModal();
            services.AddBlazoredToast();

            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
            services.AddSingleton<IBuildDriver, BuildDriver>();
            services.AddScoped<ICharacterBuildService, CharacterBuildService>();
            services.AddScoped<IFileStorageService, OneDriveFileStorageService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<RandomNumberGenerator, RNGCryptoServiceProvider>();
            services.AddScoped<IUserManager, UserManagerWrapper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> log)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                // Note that the Cosmos DB connection doesn't support Migrate()
                // We will have to be careful to support old format records :)
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
                SeedAdminPermission(context);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private void SeedAdminPermission(ApplicationDbContext context)
        {
            // Set this on a temporary basis only to replace the admin user, if you're sure
            // you have the correct name!
            var forceAdminUser = Configuration["Authentication:ForceAdminUser"];
            if (!string.IsNullOrWhiteSpace(forceAdminUser))
            {
                var toChange = context.Users.Where(u => u.IsAdmin == true || u.UserName == forceAdminUser).ToList();
                foreach (var u in toChange)
                {
                    u.IsAdmin = u.UserName == forceAdminUser;
                }

                context.SaveChanges();
                return;
            }

            // This code creates a default admin user if there isn't one already and is safe to
            // leave enabled all the time
            var adminUser = Configuration["Authentication:AdminUser"];
            if (string.IsNullOrWhiteSpace(adminUser))
            {
                return;
            }

            var possibleAdmins = context.Users.Where(u => u.IsAdmin == true || u.UserName == adminUser).ToList();
            if (possibleAdmins.Count == 1 && possibleAdmins[0].IsAdmin != true)
            {
                possibleAdmins[0].IsAdmin = true;
                context.SaveChanges();
            }
        }
    }
}
