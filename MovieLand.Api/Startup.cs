using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MovieLand.Api.HyperMedia;
using MovieLand.Api.Models;
using MovieLand.Api.ResponseEnrichers;
using MovieLand.BLL;
using MovieLand.BLL.Configurations;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Services;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Models;
using MovieLand.Data.Repositories;

namespace MovieLand.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // Setup custom configurations
            var moviePosterFileConfSection = Configuration.GetSection("MoviePosterFileConfiguration");
            services.Configure<MoviePosterFileConfiguration>(moviePosterFileConfSection);

            var apiConfSection = Configuration.GetSection("ApiConfiguration");
            services.Configure<ApiConfiguration>(apiConfSection);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest); 

            // Setup db connection
            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => {
                options.UseSqlServer(connection);
            });

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connection);

            services.AddIdentity<AppUser, AppRole>(o => {
                // configure identity options
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
            })
                .AddUserManager<AppUserManager>()
                .AddRoleManager<AppRoleManager>()
                .AddEntityFrameworkStores<AppDbContext>();

            var apiConf = apiConfSection.Get<ApiConfiguration>();

            // Setup IdentityServer4
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer("Bearer", options => {
                    options.Authority = apiConf.Authority;
                    options.RequireHttpsMetadata = false;

                    options.Audience = apiConf.Audience;
                });

            // Setup entity services
            services.AddTransient<IMovieService, MovieService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<IStarRatingService, StarRatingService>();
            services.AddTransient<IGenreService, GenreService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            var filePath = Configuration["LocalFilesRoot"];
            services.AddScoped<IFileClient, LocalFileClient>(client => {
                return new LocalFileClient(filePath);
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

            // Setup swagger
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "MovieLand API docs",
                    Version = "v1"
                });
            });

            var filtertOptions = new HyperMediaFilterOptions();
            filtertOptions.ObjectContentResponseEnricherList.Add(new GenreDtoEnricher());
            services.AddSingleton(filtertOptions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app) {
            app.UseCors("default");
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            //app.UseMvc();
        }
    }
}
