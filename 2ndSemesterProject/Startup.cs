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
using _2ndSemesterProject.Models.Database;
using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using _2ndSemesterProject.Services;
using System.Text.Json;

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
            services.AddAntiforgery();
            services.Configure<AntiforgeryOptions>(options =>
            {
                options.FormFieldName = "ASP-AF";
                options.HeaderName = "ASP-AF-HEADER";
                options.SuppressXFrameOptionsHeader = false;
            });

            // Localization
            services.AddLocalization();
            services.Configure<LocalizationOptions>(options =>
            {

            });

            // API Versioning
            services.AddApiVersioning();
            services.Configure<ApiVersioningOptions>(options =>
            {

            });

            services.AddTransient<IEmailSender, SendGridEmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);

            // Identity
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
                //.AddUserManager<UserManager<AppUser>>();


            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.MaxDepth = 128;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
            
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (IServiceScope scope = app.ApplicationServices.CreateScope()) {
                //Get services here

                using (ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>()) {
                    dbContext.Database.EnsureCreated();
                    //dbContext.Database.Migrate();

                    //If there are exceptions here, ensure the database is properly created
                    //(using Update-Database <Migration>

                    //WARNING: Do not use this on large tables
                    //https://stackoverflow.com/a/24520014
                    //UPDATE: This is fucking up the foreign keys
                    /*dbContext.AccountPlans.RemoveRange(dbContext.AccountPlans);
                    dbContext.SaveChanges();*/

                    //Make sure the tiers are created.
                    if (dbContext.AccountPlans.SingleOrDefault(x => x.Name == "Free") == null) {
                        dbContext.AccountPlans.Add(AccountPlan.GetFreeTier());
                    } else {
                        var target = AccountPlan.GetFreeTier();
                        var origin = dbContext.AccountPlans.Single(a => a.Name == target.Name);

                        origin.PricePerMonth = target.PricePerMonth;
                        origin.ReductionPerMonth = target.ReductionPerMonth;
                        origin.PricePerYear = target.PricePerYear;
                        origin.ReductionPerYear = target.ReductionPerYear;

                        origin.State = target.State;

                        origin.FileSizeLimit = target.FileSizeLimit;
                        origin.FileTransferSize = target.FileTransferSize;
                        origin.GlobalStorageLimit = target.GlobalStorageLimit;
                    }

                    if (dbContext.AccountPlans.SingleOrDefault(x => x.Name == "Plus") == null) {
                        dbContext.AccountPlans.Add(AccountPlan.GetPlusTier());
                    } else {
                        var target = AccountPlan.GetPlusTier();
                        var origin = dbContext.AccountPlans.Single(a => a.Name == target.Name);

                        origin.PricePerMonth = target.PricePerMonth;
                        origin.ReductionPerMonth = target.ReductionPerMonth;
                        origin.PricePerYear = target.PricePerYear;
                        origin.ReductionPerYear = target.ReductionPerYear;

                        origin.State = target.State;

                        origin.FileSizeLimit = target.FileSizeLimit;
                        origin.FileTransferSize = target.FileTransferSize;
                        origin.GlobalStorageLimit = target.GlobalStorageLimit;
                    }

                    if (dbContext.AccountPlans.SingleOrDefault(x => x.Name == "Pro") == null) {
                        dbContext.AccountPlans.Add(AccountPlan.GetProTier());
                    } else {
                        var target = AccountPlan.GetProTier();
                        var origin = dbContext.AccountPlans.Single(a => a.Name == target.Name);

                        origin.PricePerMonth = target.PricePerMonth;
                        origin.ReductionPerMonth = target.ReductionPerMonth;
                        origin.PricePerYear = target.PricePerYear;
                        origin.ReductionPerYear = target.ReductionPerYear;

                        origin.State = target.State;

                        origin.FileSizeLimit = target.FileSizeLimit;
                        origin.FileTransferSize = target.FileTransferSize;
                        origin.GlobalStorageLimit = target.GlobalStorageLimit;
                    }

                    dbContext.SaveChanges();
                }
            }

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                app.UseResponseCompression();
                app.UseResponseCaching();
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
