using AutoMapper;
using Ich.Saas.Core.Code.Caching;
using Ich.Saas.Core.Code.Identity;
using Ich.Saas.Core.Code.Infrastructure;
using Ich.Saas.Core.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
using System.Reflection;
using Ich.Saas.Core.Code.FlatAreas;
using Ich.Saas.Core.Code.Localization;
using Microsoft.AspNetCore.Localization;
using RequestCultureProvider = Ich.Saas.Core.Code.Localization.RequestCultureProvider;

namespace Ich.Saas.Core
{
    public class Startup
    {
        private IConfiguration _config { get; }
        private IWebHostEnvironment _env { get; }
        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Caching
            services.AddMemoryCache(); // if already added AddControllersWithViews, don't need to AddMemoryCache
            services.AddScoped<ICache, Cache>();
            services.AddScoped<IFilter, Filter>();
            services.AddScoped<ILookup, Lookup>();
            
            // Identity support
            services.AddSingleton<ICurrentTenant, CurrentTenant>();
            services.AddSingleton<ICurrentUser, CurrentUser>();
            services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, ClaimsPrincipalFactory>();
            services.AddScoped<IIdentityService, IdentityService>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();
            
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/login");
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });
            
            services.AddHttpContextAccessor();
            
            // Configure localization, i.e cultures and uicultures
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("nl-NL") };

                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Insert(0, new RequestCultureProvider());
            });
            
            // Custom string localization from database
            services.AddStringLocalization();
            
            // Database context: For localdb connectionString's path is calculated
            var connectionString = _config.GetConnectionString("Saas").Replace("{Path}", _env.ContentRootPath);
            
            services.AddDbContext<SaaSContext>(options => options.UseSqlServer(connectionString));

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            services.AddControllersWithViews(options => {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            })
                .AddDataAnnotationsLocalization()
                .AddFlatAreas(new FlatAreaOptions())
                .AddRazorRuntimeCompilation();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            ServiceLocator.Register(httpContextAccessor);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
            });
        }
    }
}
