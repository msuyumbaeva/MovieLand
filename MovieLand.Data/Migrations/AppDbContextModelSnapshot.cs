﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieLand.Data.ApplicationDbContext;

namespace MovieLand.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("MovieLand.Data.Models.AppRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("MovieLand.Data.Models.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("MovieLand.Data.Models.Artist", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("MovieLand.Data.Models.Career", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Careers");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Director"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Actor"
                        });
                });

            modelBuilder.Entity("MovieLand.Data.Models.Country", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("MovieLand.Data.Models.Genre", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("MovieLand.Data.Models.Movie", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<short>("Duration");

                    b.Property<byte>("MinAge");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("OriginalName")
                        .HasMaxLength(100);

                    b.Property<string>("Poster")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("ReleaseYear");

                    b.HasKey("Id");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("MovieLand.Data.Models.MovieArtist", b =>
                {
                    b.Property<Guid>("MovieId");

                    b.Property<Guid>("ArtistId");

                    b.Property<int>("CareerId");

                    b.Property<byte>("Priority");

                    b.HasKey("MovieId", "ArtistId", "CareerId");

                    b.HasIndex("ArtistId");

                    b.HasIndex("CareerId");

                    b.ToTable("MovieArtists");
                });

            modelBuilder.Entity("MovieLand.Data.Models.MovieCountry", b =>
                {
                    b.Property<Guid>("MovieId");

                    b.Property<Guid>("CountryId");

                    b.HasKey("MovieId", "CountryId");

                    b.HasIndex("CountryId");

                    b.ToTable("MovieContries");
                });

            modelBuilder.Entity("MovieLand.Data.Models.MovieGenre", b =>
                {
                    b.Property<Guid>("MovieId");

                    b.Property<Guid>("GenreId");

                    b.HasKey("MovieId", "GenreId");

                    b.HasIndex("GenreId");

                    b.ToTable("MovieGenres");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("MovieLand.Data.Models.AppRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("MovieLand.Data.Models.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("MovieLand.Data.Models.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("MovieLand.Data.Models.AppRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MovieLand.Data.Models.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("MovieLand.Data.Models.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MovieLand.Data.Models.MovieArtist", b =>
                {
                    b.HasOne("MovieLand.Data.Models.Artist", "Artist")
                        .WithMany()
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MovieLand.Data.Models.Career", "Career")
                        .WithMany()
                        .HasForeignKey("CareerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MovieLand.Data.Models.Movie", "Movie")
                        .WithMany()
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MovieLand.Data.Models.MovieCountry", b =>
                {
                    b.HasOne("MovieLand.Data.Models.Country", "Country")
                        .WithMany("MovieCountries")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MovieLand.Data.Models.Movie", "Movie")
                        .WithMany()
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MovieLand.Data.Models.MovieGenre", b =>
                {
                    b.HasOne("MovieLand.Data.Models.Genre", "Genre")
                        .WithMany("MovieGenres")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MovieLand.Data.Models.Movie", "Movie")
                        .WithMany()
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
