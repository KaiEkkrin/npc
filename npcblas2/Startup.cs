using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Toast;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
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

            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });
            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddBlazoredModal();
            services.AddBlazoredToast();

            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
            services.AddSingleton<IBuildDriver, BuildDriver>();
            services.AddScoped<ICharacterBuildService, CharacterBuildService>();
            services.AddSingleton<RandomNumberGenerator, RNGCryptoServiceProvider>();
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
