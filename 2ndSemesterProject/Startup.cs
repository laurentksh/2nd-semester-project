using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using _2ndSemesterProject.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using _2ndSemesterProject.Models;

namespace _2ndSemesterProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Load the DB connection string.
            services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")), 128);

            // Anti-Forgery
            var options = new AntiforgeryOptions
            {
                FormFieldName = "ASP-AF",
                HeaderName = "ASP-AF-HEADER",
                SuppressXFrameOptionsHeader = false
            };

            services.AddAntiforgery(o1 => o1 = options);

            // Localization
            var options2 = new LocalizationOptions();

            services.AddLocalization(o2 => o2 = options2);

            // API Versioning
            var options3 = new ApiVersioningOptions
            {
                
            };

            services.AddApiVersioning(o3 => o3 = options3);

            // Identity

            var identityOpt = new IdentityOptions();
            identityOpt.SignIn.RequireConfirmedEmail = true;
            identityOpt.Password.RequiredLength = 8;
            identityOpt.Password.RequiredUniqueChars = 4;
            identityOpt.Password.RequireNonAlphanumeric = false;
            identityOpt.User.RequireUniqueEmail = true;

            services.AddIdentity<AppUser, AppRole>(o4 => o4 = identityOpt)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /*using (ApplicationDbContext dbContext = app.ApplicationServices.GetRequiredService<ApplicationDbContext>())
            {
                dbContext.Database.EnsureCreated();
            }*/

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            } else {
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
