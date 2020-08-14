using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieLand.Api.Models;
using MovieLand.BLL;
using MovieLand.BLL.Configurations;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Services;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Repositories;

namespace MovieLand.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
            swwwRootPath = @"C:\Users\E7450\source\repos\MovieLand\MovieLand\wwwroot\";
        }

        public IConfiguration Configuration { get; }
        public string swwwRootPath { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // Setup custom configurations
            var moviePosterFileConfSection = Configuration.GetSection("MoviePosterFileConfiguration");
            services.Configure<MoviePosterFileConfiguration>(moviePosterFileConfSection);

            var apiConfSection = Configuration.GetSection("ApiConfiguration");
            services.Configure<ApiConfiguration>(apiConfSection);

            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            // Setup db connection
            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => {
                options.UseSqlServer(connection);
            });

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connection);

            var apiConf = apiConfSection.Get<ApiConfiguration>();

            // Setup IdentityServer4
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options => {
                    options.Authority = apiConf.Authority;
                    options.RequireHttpsMetadata = false;

                    options.Audience = apiConf.Audience;
                });


            // Setup entity services
            services.AddTransient<IMovieService, MovieService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IFileClient, LocalFileClient>(client => {
                return new LocalFileClient(swwwRootPath);
            });

            // Setup mapping profiles
            var mappingConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new MappingProfileBLL());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddCors(options => {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy => {
                    policy.WithOrigins("http://localhost:5003")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app) {
            app.UseCors("default");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
