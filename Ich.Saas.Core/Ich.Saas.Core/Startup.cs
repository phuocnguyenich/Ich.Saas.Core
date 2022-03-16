using Ich.Saas.Core.Code.Caching;
using Ich.Saas.Core.Code.Identity;
using Ich.Saas.Core.Code.Infrastructure;
using Ich.Saas.Core.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core
{
    public class Startup
    {
        public IConfiguration _config { get; }
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

            // Identity support
            services.AddSingleton<ICurrentTenant, CurrentTenant>();
            services.AddSingleton<ICurrentUser, CurrentUser>();
            services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, ClaimsPrincipalFactory>();
            services.AddScoped<IIdentityService, IdentityService>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddHttpContextAccessor();

            // Database context: For localdb connectionString's path is calculated
            var connectionString = _config.GetConnectionString("Saas").Replace("{Path}", _env.ContentRootPath);
            
            services.AddDbContext<SaaSContext>(options => options.UseSqlServer(connectionString));

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            services.AddControllersWithViews(options => {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
