using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using _2ndSemesterProject.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Anti-Forgery
            var options = new Microsoft.AspNetCore.Antiforgery.AntiforgeryOptions
            {
                FormFieldName = "ASP-AF",
                HeaderName = "ASP-AF-HEADER",
                SuppressXFrameOptionsHeader = false
            };

            services.AddAntiforgery(new Action<Microsoft.AspNetCore.Antiforgery.AntiforgeryOptions>((x) => x = options));

            // Localization
            var options2 = new Microsoft.Extensions.Localization.LocalizationOptions
            {

            };

            services.AddLocalization(new Action<Microsoft.Extensions.Localization.LocalizationOptions>((y) => y = options2));

            // API Versioning
            var options3 = new Microsoft.AspNetCore.Mvc.Versioning.ApiVersioningOptions
            {
                
            };

            services.AddApiVersioning(new Action<Microsoft.AspNetCore.Mvc.Versioning.ApiVersioningOptions>((z) => z = options3));

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
