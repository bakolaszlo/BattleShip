using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueCliMiddleware;
using BattleShip.Data;
using BattleShip.SignalR;
using System.Diagnostics;

namespace BattleShip
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
            services.AddControllers();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp";
            });
			services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    GetConnectionString()));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddSignalR();

            if (Debugger.IsAttached)
            {
                services.AddSpaStaticFiles(configuration =>
                {
                    configuration.RootPath = "ClientApp";
                });
            }
            else
            {
                services.AddSpaStaticFiles(configuration =>
                {
                    configuration.RootPath = "ClientApp/dist";
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
				app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();
            app.UseSpaStaticFiles();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MessageHub>("/messagehub");
            });

            app.UseSpa(spa =>
            {
                if (env.IsDevelopment())
                    spa.Options.SourcePath = "ClientApp/";
                else
                    spa.Options.SourcePath = "dist";

                if (env.IsDevelopment())
                {
                    spa.UseVueCli(npmScript: "serve");
                }

            });
        }

        private string GetConnectionString()
        {
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (connectionString != null)
            {
                return ConvertConnectionString(connectionString);
            }

            return Configuration.GetConnectionString("LocalDB");
        }

        private string ConvertConnectionString(string connectionString)
        {
            Uri uri = new(connectionString);
            string userId = uri.UserInfo.Split(':')[0];
            string password = uri.UserInfo.Split(':')[1];
            string database = uri.AbsolutePath.TrimStart('/');
            return $"Database={database}; Host={uri.Host}; Port={uri.Port}; User Id={userId}; Password={password}; SSL Mode=Require;Trust Server Certificate=true";
        }
    }
}
