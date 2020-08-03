using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieLand.Data.Enums;
using MovieLand.Data.Extensions;
using MovieLand.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieLand.Data.ApplicationDbContext
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDbContext(DbContextOptions options) : base(options) {  }

        #region DbSets
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<MovieCountry> MovieContries { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Career> Careers { get; set; }
        public DbSet<Artist> Artists { get; set; }
        #endregion DbSets

        #region Overrides
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            //Set composite keys to entities
            modelBuilder.Entity<MovieGenre>().HasKey(sc => new { sc.MovieId, sc.GenreId });
            modelBuilder.Entity<MovieCountry>().HasKey(sc => new { sc.MovieId, sc.CountryId });

            modelBuilder.SeedEnumValues<Career, CareerEnum>(e => new Career(e));
        }
        #endregion Overrides
    }
}
