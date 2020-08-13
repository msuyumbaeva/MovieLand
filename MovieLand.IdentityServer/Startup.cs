using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Models;
using IdentityServer4;
using System;
using Microsoft.AspNetCore.Http;

namespace MovieLand.IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {

            // Setup db connection
            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => {
                options.UseSqlServer(connection);
            });

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connection);

            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options => {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
               .AddInMemoryIdentityResources(Config.GetIdentityResources())
               .AddInMemoryApiResources(Config.GetApis())
               .AddInMemoryClients(Config.GetClients())
               .AddAspNetIdentity<AppUser>();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseIdentityServer();

            app.Run(async (context) => {
                await context.Response.WriteAsync("IdSrv4");
            });
        }
    }
}
