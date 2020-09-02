using AutoMapper;
using ExpectedObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using MovieLand.BLL.Configurations;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Artist;
using MovieLand.BLL.Dtos.Country;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.BLL.Dtos.Genre;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.BLL.Services;
using MovieLand.Data.Builders;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Enums;
using MovieLand.Data.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Tests.Services
{
    [TestFixture]
    class MovieServiceTests
    {
        private MovieService _sut;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            // Setup mapping profile
            var mappingConfig = new MapperConfiguration(mc =>
                mc.AddProfile(new MappingProfileBLL())
            );
            _mapper = mappingConfig.CreateMapper();
        }

        private static readonly Artist[] _testArtists = new Artist[5] {
            new Artist() { Id = Guid.NewGuid(), Name = "Daniel Jacob Radcliffe" },
            new Artist() { Id = Guid.NewGuid(), Name = "Leonardo DiCaprio" },
            new Artist() { Id = Guid.NewGuid(), Name = "Keira Knightley" },
            new Artist() { Id = Guid.NewGuid(), Name = "Robert John Downey Jr." },
            new Artist() { Id = Guid.NewGuid(), Name = "Natalie Portman" }
        };

        private static readonly Country[] _testCountries = new Country[5] {
            new Country() { Id = Guid.NewGuid(), Name = "USA" },
            new Country() { Id = Guid.NewGuid(), Name = "UK" },
            new Country() { Id = Guid.NewGuid(), Name = "France" },
            new Country() { Id = Guid.NewGuid(), Name = "Mexico" },
            new Country() { Id = Guid.NewGuid(), Name = "Canada" }
        };

        private static readonly Genre[] _testGenres = new Genre[5]
        {
            new Genre() { Id = Guid.NewGuid(), Name = "Drama" },
            new Genre() { Id = Guid.NewGuid(), Name = "Romance" },
            new Genre() { Id = Guid.NewGuid(), Name = "Action" },
            new Genre() { Id = Guid.NewGuid(), Name = "Crime" },
            new Genre() { Id = Guid.NewGuid(), Name = "Adventure" }
        };

        private readonly Movie[] _testMovies = new Movie[5]
        {
            new Movie() { Id = Guid.NewGuid(), Name = "Titanic",  Duration = 194, ReleaseYear = 1997, Poster = "titanicPoster.jpg",
                MovieArtists = new MovieArtist[1] { new MovieArtist() { Artist = _testArtists[1], CareerId = CareerEnum.Actor } },
                MovieCountries = new MovieCountry[3] { new MovieCountry() { Country = _testCountries[0] }, new MovieCountry() { Country = _testCountries[3] }, new MovieCountry() { Country = _testCountries[4] } },
                MovieGenres = new MovieGenre[2] { new MovieGenre() { Genre = _testGenres[0] }, new MovieGenre() { Genre = _testGenres[1] } }
            },
            new Movie() { Id = Guid.NewGuid(), Name = "Leon", Duration = 133, ReleaseYear = 1994, Poster = "leonPoster.png",
                MovieArtists = new MovieArtist[1] { new MovieArtist() { Artist = _testArtists[4], CareerId = CareerEnum.Actor } },
                MovieCountries = new MovieCountry[2] { new MovieCountry() { Country = _testCountries[2] }, new MovieCountry() { Country = _testCountries[0] } },
                MovieGenres = new MovieGenre[2] { new MovieGenre() { Genre = _testGenres[2] }, new MovieGenre() { Genre = _testGenres[3] } }
            },
            new Movie() { Id = Guid.NewGuid(), Name = "Pride & Prejudice", Duration = 129, ReleaseYear = 2005, Poster = "prideAndPrejudicePoster.jpg",
                MovieArtists = new MovieArtist[1] { new MovieArtist() { Artist = _testArtists[2], CareerId = CareerEnum.Actor } },
                MovieCountries = new MovieCountry[3] { new MovieCountry() { Country = _testCountries[2] }, new MovieCountry() { Country = _testCountries[1] }, new MovieCountry() { Country = _testCountries[0] } },
                MovieGenres = new MovieGenre[2] { new MovieGenre() { Genre = _testGenres[0] }, new MovieGenre() { Genre = _testGenres[1] } }
            },
            new Movie() { Id = Guid.NewGuid(), Name = "Iron Man", Duration = 121, ReleaseYear = 2008, Poster = "ironManPoster.png",
                MovieArtists = new MovieArtist[1] { new MovieArtist() { Artist = _testArtists[3], CareerId = CareerEnum.Actor } },
                MovieCountries = new MovieCountry[2] { new MovieCountry() { Country = _testCountries[0] }, new MovieCountry() { Country = _testCountries[4] } },
                MovieGenres = new MovieGenre[2] { new MovieGenre() { Genre = _testGenres[2] }, new MovieGenre() { Genre = _testGenres[4] } }
            },
            new Movie() { Id = Guid.NewGuid(), Name = "Harry Potter and the Sorcerer's Stone", Duration = 152, ReleaseYear = 2001, Poster = "harryPotterPoster.jpg",
                MovieArtists = new MovieArtist[1] { new MovieArtist() { Artist = _testArtists[0], CareerId = CareerEnum.Actor } },
                MovieCountries = new MovieCountry[2] { new MovieCountry() { Country = _testCountries[1] }, new MovieCountry() { Country = _testCountries[0] } },
                MovieGenres = new MovieGenre[1] { new MovieGenre() { Genre = _testGenres[4] } }
            }
        };

        #region SaveAsync tests

        [Test]
        public async Task SaveAsync_EmptyIdAndNotNullPoster_CreatesNew()
        {
            // Arrange
            var newMovie = new Movie()
            {
                Id = Guid.NewGuid(),
                Name = "New Movie name",
                ReleaseYear = 2020,
                MinAge = 16,
                Duration = 100,
                Description = "Movie description",
                Poster = "moviePoster.jpg"
            };

            var expectedValue = new MovieDto()
            {
                Id = newMovie.Id,
                Name = newMovie.Name,
                ReleaseYear = newMovie.ReleaseYear,
                MinAge = newMovie.MinAge,
                Duration = newMovie.Duration,
                Description = newMovie.Description,
                Poster = newMovie.Poster
            }.ToExpectedObject();

            var mockFileClient = new Mock<IFileClient>();
            mockFileClient
                .Setup(x => x.SaveFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()));

            var fileConfiguration = new MoviePosterFileConfiguration()
            {
                Directory = "posters",
                Width = 300,
                Height = 450
            };

            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(fileConfiguration);

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.AddAsync(It.IsAny<Movie>()))
                .ReturnsAsync((Movie movie) => {
                    movie.Poster = movie.Poster.Substring(20);
                    movie.Id = newMovie.Id;
                    return movie;
                });

            mockUOW.Setup(x => x.CompleteAsync());

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            var sourceImg = File.OpenRead(@"C:\Users\E7450\source\repos\MovieLand\MovieLand\wwwroot\posters\no-movie-poster.jpg");
            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(x => x.ContentType).Returns("image/jpeg");
            mockFormFile.SetupGet(x => x.FileName).Returns(newMovie.Poster);
            mockFormFile.SetupGet(x => x.Name).Returns("poster");
            mockFormFile.SetupGet(x => x.Length).Returns(1024);
            mockFormFile
                .Setup(x => x.OpenReadStream())
                .Returns(sourceImg);

            // Act
            var request = new MovieCreateDto()
            {
                Id = Guid.Empty,
                Name = newMovie.Name,
                ReleaseYear = newMovie.ReleaseYear,
                MinAge = newMovie.MinAge,
                Duration = newMovie.Duration,
                Description = newMovie.Description,
                Poster = mockFormFile.Object
            };

            var result = await _sut.SaveAsync(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            expectedValue.ShouldEqual(result.Entity);
        }

        [Test]
        public async Task SaveAsync_EmptyIdAndNullPoster_CreatesNewWithDefaultPoster()
        {
            // Arrange
            var newMovie = new Movie()
            {
                Id = Guid.NewGuid(),
                Name = "New Movie name",
                ReleaseYear = 2020,
                MinAge = 16,
                Duration = 100,
                Description = "Movie description",
                Poster = "no-movie-poster.jpg"
            };

            var expectedValue = new MovieDto()
            {
                Id = newMovie.Id,
                Name = newMovie.Name,
                ReleaseYear = newMovie.ReleaseYear,
                MinAge = newMovie.MinAge,
                Duration = newMovie.Duration,
                Description = newMovie.Description,
                Poster = newMovie.Poster
            }.ToExpectedObject();

            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.AddAsync(It.IsAny<Movie>()))
                .ReturnsAsync((Movie movie) => {
                    movie.Id = newMovie.Id;
                    return movie;
                });

            mockUOW.Setup(x => x.CompleteAsync());

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var request = new MovieCreateDto()
            {
                Id = Guid.Empty,
                Name = newMovie.Name,
                ReleaseYear = newMovie.ReleaseYear,
                MinAge = newMovie.MinAge,
                Duration = newMovie.Duration,
                Description = newMovie.Description,
                Poster = null
            };

            var result = await _sut.SaveAsync(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            expectedValue.ShouldEqual(result.Entity);
        }

        [Test]
        public async Task SaveAsync_NotEmptyIdAndNotNullPoster_UpdatesWithNewPoster()
        {
            // Arrange
            var existingMovie = _testMovies[0];
            var newPoster = "newPoster.jpg";
            var newName = "new name";

            var expectedValue = new MovieDto()
            {
                Id = existingMovie.Id,
                Name = newName,
                ReleaseYear = existingMovie.ReleaseYear,
                MinAge = existingMovie.MinAge,
                Duration = existingMovie.Duration,
                Description = existingMovie.Description,
                Poster = newPoster
            };

            var mockFileClient = new Mock<IFileClient>();
            mockFileClient
                .Setup(x => x.SaveFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()));

            var fileConfiguration = new MoviePosterFileConfiguration()
            {
                Directory = "posters",
                Width = 300,
                Height = 450
            };

            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(fileConfiguration);

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => _testMovies.Where(m => m.Id == id).FirstOrDefault());

            mockUOW.Setup(x => x.Movies.Update(It.IsAny<Movie>()));
            mockUOW.Setup(x => x.CompleteAsync());

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            var sourceImg = File.OpenRead(@"C:\Users\E7450\source\repos\MovieLand\MovieLand\wwwroot\posters\no-movie-poster.jpg");
            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(x => x.ContentType).Returns("image/jpeg");
            mockFormFile.SetupGet(x => x.FileName).Returns(newPoster);
            mockFormFile.SetupGet(x => x.Name).Returns("poster");
            mockFormFile.SetupGet(x => x.Length).Returns(1024);
            mockFormFile
                .Setup(x => x.OpenReadStream())
                .Returns(sourceImg);

            // Act
            var request = new MovieCreateDto()
            {
                Id = existingMovie.Id,
                Name = newName,
                ReleaseYear = existingMovie.ReleaseYear,
                MinAge = existingMovie.MinAge,
                Duration = existingMovie.Duration,
                Description = existingMovie.Description,
                Poster = mockFormFile.Object
            };

            var result = await _sut.SaveAsync(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedValue.Name, result.Entity.Name);
            Assert.AreEqual(expectedValue.Poster, result.Entity.Poster.Substring(20));
        }

        [Test]
        public async Task SaveAsync_NotEmptyIdAndNullPoster_UpdatesWithOldPoster()
        {
            // Arrange
            var existingMovie = _testMovies[0];

            var expectedValue = new MovieDto()
            {
                Id = existingMovie.Id,
                Name = existingMovie.Name,
                ReleaseYear = existingMovie.ReleaseYear,
                MinAge = existingMovie.MinAge,
                Duration = existingMovie.Duration,
                Description = existingMovie.Description,
                Poster = existingMovie.Poster
            }.ToExpectedObject();

            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => _testMovies.Where(m=>m.Id == id).FirstOrDefault());

            mockUOW.Setup(x => x.Movies.Update(It.IsAny<Movie>()));
            mockUOW.Setup(x => x.CompleteAsync());

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var request = new MovieCreateDto()
            {
                Id = existingMovie.Id,
                Name = existingMovie.Name,
                ReleaseYear = existingMovie.ReleaseYear,
                MinAge = existingMovie.MinAge,
                Duration = existingMovie.Duration,
                Description = existingMovie.Description,
                Poster = null
            };

            var result = await _sut.SaveAsync(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            expectedValue.ShouldEqual(result.Entity);
        }

        [Test]
        public async Task SaveAsync_ExceptionInRepoCompleteAsync_Error()
        {
            // Arrange
            var expectedException = "An error occurred while updating the entries.";

            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.AddAsync(It.IsAny<Movie>()))
                .ReturnsAsync((Movie movie) => movie);

            mockUOW.Setup(x => x.CompleteAsync())
                .ThrowsAsync(new Exception(expectedException));

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var request = new MovieCreateDto();
            var result = await _sut.SaveAsync(request);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(expectedException, result.Errors.First());
        }

        [Test]
        public async Task SaveAsync_EmptyIdAndNameLengthMoreThan100_ValidationFailedError() {
            // Arrange
            var mockFileClient = new Mock<IFileClient>();
            mockFileClient
                .Setup(x => x.SaveFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()));

            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var request = new MovieCreateDto() {
                Id = Guid.Empty,
                Name = new string('a', 101),
                ReleaseYear = 2020,
                MinAge = 16,
                Duration = 100,
                Description = "Movie description",
                Poster = null
            };

            var result = await _sut.SaveAsync(request);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Errors[0].Contains("Validation failed"));
        }

        #endregion SaveAsync tests

        #region GetAsync tests

        [Test]
        public async Task GetAsync_FullDataTableParameters_FiveMovieListItems()
        {
            // Arrange
            var expectedValue = new MovieListItemDto()
            {
                Id = _testMovies[0].Id,
                Name = _testMovies[0].Name,
                OriginalName = _testMovies[0].OriginalName,
                Duration = _testMovies[0].Duration,
                ReleaseYear = _testMovies[0].ReleaseYear,
                Poster = _testMovies[0].Poster
            }.ToExpectedObject();

            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.GetAsync(It.IsAny<EntityQueryBuilder<Movie>>()))
                .ReturnsAsync(_testMovies);

            mockUOW.Setup(x => x.Movies.CountAsync())
                .ReturnsAsync(_testMovies.Length);

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var request = new MovieDataTablesParameters()
            {
                Columns = new DTColumn[1] { new DTColumn() { Data = "Name", Name = "Name" } },
                Search = new DTSearch() { Value = "Leo" },
                Order = new DTOrder[1] { new DTOrder() { Column = 0, Dir = DTOrderDir.ASC } },
                Length = 3,
                Start = 1,
                Genre = Guid.NewGuid(),
                Artist = Guid.NewGuid(),
                Country = Guid.NewGuid()
            };
            var result = await _sut.GetAsync(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(_testMovies.Length, result.Entity.TotalSize);
            expectedValue.ShouldEqual(result.Entity.Items.First());
        }

        [Test]
        public async Task GetAsync_EmptyDataTableParameters_FiveMovieListItems()
        {
            // Arrange
            var expectedValue = new MovieListItemDto()
            {
                Id = _testMovies[0].Id,
                Name = _testMovies[0].Name,
                OriginalName = _testMovies[0].OriginalName,
                Duration = _testMovies[0].Duration,
                ReleaseYear = _testMovies[0].ReleaseYear,
                Poster = _testMovies[0].Poster
            }.ToExpectedObject();

            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.GetAsync(It.IsAny<EntityQueryBuilder<Movie>>()))
                .ReturnsAsync(_testMovies);

            mockUOW.Setup(x => x.Movies.CountAsync())
                .ReturnsAsync(_testMovies.Length);

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var request = new MovieDataTablesParameters();
            var result = await _sut.GetAsync(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(_testMovies.Length, result.Entity.TotalSize);
            expectedValue.ShouldEqual(result.Entity.Items.First());
        }

        [Test]
        public async Task GetAsync_NullDataTableParameters_ArgumentNullExceptionError()
        {
            // Arrange
            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.GetAsync(null);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Value cannot be null. (Parameter 'table')", result.Errors.First());
        }

        #endregion GetAsync tests

        #region GetById tests

        [Test]
        public async Task GetById_MovieExists_Dto()
        {
            // Assert
            var expectedValue = new MovieDto()
            {
                Id = _testMovies[0].Id,
                Name = _testMovies[0].Name,
                OriginalName = _testMovies[0].OriginalName,
                Duration = _testMovies[0].Duration,
                ReleaseYear = _testMovies[0].ReleaseYear,
                Poster = _testMovies[0].Poster,
                Actors = new List<ArtistDto> { new ArtistDto() { Id = _testMovies[0].MovieArtists.First().Artist.Id, Name = _testMovies[0].MovieArtists.First().Artist.Name } },
                Countries = new List<CountryDto> { new CountryDto() { Id = _testMovies[0].MovieCountries.First().Country.Id, Name = _testMovies[0].MovieCountries.First().Country.Name }, new CountryDto() { Id = _testMovies[0].MovieCountries.ElementAt(1).Country.Id, Name = _testMovies[0].MovieCountries.ElementAt(1).Country.Name }, new CountryDto() { Id = _testMovies[0].MovieCountries.ElementAt(2).Country.Id, Name = _testMovies[0].MovieCountries.ElementAt(2).Country.Name } },
                Genres = new List<GenreDto> { new GenreDto() { Id = _testMovies[0].MovieGenres.First().Genre.Id, Name = _testMovies[0].MovieGenres.First().Genre.Name }, new GenreDto() { Id = _testMovies[0].MovieGenres.ElementAt(1).Genre.Id, Name = _testMovies[0].MovieGenres.ElementAt(1).Genre.Name } }
            }.ToExpectedObject();

            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.GetFullByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => _testMovies.Where(m => m.Id == id).FirstOrDefault());

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.GetByIdAsync(_testMovies[0].Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            expectedValue.ShouldEqual(result.Entity);
        }

        [Test]
        public async Task GetById_MovieNotExists_Null()
        {
            // Arrange
            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.GetFullByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => _testMovies.Where(m => m.Id == id).FirstOrDefault());

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.GetByIdAsync(Guid.Empty);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.Entity);
        }

        [Test]
        public async Task GetById_ExceptionInRepoGetFullByIdAsync_Error()
        {
            // Arrange
            var expectedException = "An error occurred while executing query.";

            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.GetFullByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception(expectedException));

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.GetByIdAsync(Guid.Empty);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(expectedException, result.Errors.First());
        }

        #endregion GetById tests

        #region GetGenresOfMovieAsync tests

        [Test]
        public async Task GetGenresOfMovieAsync_GenresList()
        {
            // Assert
            var expectedValue = _testMovies[0].MovieGenres.Count;

            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.GetGenresByMovieAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => _testMovies.Where(m => m.Id == id).FirstOrDefault().MovieGenres.Select(m=>m.Genre));

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.GetGenresOfMovieAsync(_testMovies[0].Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedValue, result.Entity.Count());
        }

        #endregion GetGenresOfMovieAsync tests

        #region GetCountriesOfMovieAsync tests

        [Test]
        public async Task GetCountriesOfMovieAsync_CountriesList()
        {
            // Assert
            var expectedValue = _testMovies[0].MovieCountries.Count;

            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.GetCountriesByMovieAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => _testMovies.Where(m => m.Id == id).FirstOrDefault().MovieCountries.Select(m => m.Country));

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.GetCountriesOfMovieAsync(_testMovies[0].Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedValue, result.Entity.Count());
        }

        #endregion GetCountriesOfMovieAsync tests

        #region GetArtistsByCareerOfMovieAsync tests

        [Test]
        public async Task GetArtistsByCareerOfMovieAsync_CountriesList()
        {
            // Assert
            var expectedValue = _testMovies[0].MovieArtists.Count;

            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.GetArtistsByMovieAndCareerAsync(It.IsAny<Guid>(), It.IsAny<CareerEnum>()))
                .ReturnsAsync((Guid id, CareerEnum career) => 
                    _testMovies.Where(m => m.Id == id).FirstOrDefault()
                    .MovieArtists.Where(m => m.CareerId == career).Select(m => m.Artist));

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.GetArtistsByCareerOfMovieAsync(_testMovies[0].Id, CareerEnum.Actor);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedValue, result.Entity.Count());
        }

        #endregion GetArtistsByCareerOfMovieAsync tests

        #region AddGenreAsync tests

        [Test]
        public async Task AddGenreAsync_MovieNotInGenre_CreatesNew() {
            // Assert
            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(x => x.Movies.AddToGenreAsync(It.IsAny<MovieGenre>())).Verifiable();
            mockUOW
                .Setup(x => x.Movies.IsInGenreAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.AddGenreAsync(_testMovies[0].Id, _testGenres[0].Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            mockUOW.Verify();
        }

        [Test]
        public async Task AddGenreAsync_MovieInGenre_DoesNotCreateNew() {
            // Assert
            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.AddToGenreAsync(It.IsAny<MovieGenre>()))
                .ThrowsAsync(new Exception("Method shouldn't be called"));
            mockUOW
                .Setup(x => x.Movies.IsInGenreAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.AddGenreAsync(_testMovies[0].Id, _testGenres[0].Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion AddGenreAsync tests

        #region AddCountryAsync tests

        [Test]
        public async Task AddCountryAsync_MovieNotInCountry_CreatesNew() {
            // Assert
            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(x => x.Movies.AddToCountryAsync(It.IsAny<MovieCountry>())).Verifiable();
            mockUOW
                .Setup(x => x.Movies.IsInCountryAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.AddCountryAsync(_testMovies[0].Id, _testCountries[0].Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            mockUOW.Verify();
        }

        [Test]
        public async Task AddCountryAsync_MovieInCountry_DoesNotCreateNew() {
            // Assert
            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.AddToCountryAsync(It.IsAny<MovieCountry>()))
                .ThrowsAsync(new Exception("Method shouldn't be called"));
            mockUOW
                .Setup(x => x.Movies.IsInCountryAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.AddCountryAsync(_testMovies[0].Id, _testCountries[0].Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion AddCountryAsync tests

        #region SaveArtistAsync tests

        [Test]
        public async Task SaveArtistAsync_MovieArtistNull_CreatesNew() {
            // Assert
            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(x => x.Movies.AddToArtistAndCareerAsync(It.IsAny<MovieArtist>())).Verifiable();
            mockUOW
                .Setup(x => x.Movies.GetByMovieAndArtistAndCareerAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CareerEnum>()))
                .ReturnsAsync((MovieArtist)null);

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.SaveArtistAsync(_testMovies[0].Id, new MovieArtistDto(_testArtists[0].Id, CareerEnum.Director, 1));

            // Assert
            Assert.IsTrue(result.IsSuccess);
            mockUOW.Verify();
        }

        [Test]
        public async Task SaveArtistAsync_MovieArtistNotNull_UpdatesExisting() {
            // Assert
            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(x => x.Movies.UpdateArtist(It.IsAny<MovieArtist>())).Verifiable();
            mockUOW
                .Setup(x => x.Movies.GetByMovieAndArtistAndCareerAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CareerEnum>()))
                .ReturnsAsync(_testMovies[0].MovieArtists.First());

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.SaveArtistAsync(_testMovies[0].Id, new MovieArtistDto(_testArtists[0].Id, CareerEnum.Actor, 2));

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion SaveArtistAsync tests

        #region RemoveGenreAsync tests

        [Test]
        public async Task RemoveGenreAsync_MovieInGenre_Deletes() {
            // Assert
            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(x => x.Movies.RemoveFromGenre(It.IsAny<MovieGenre>())).Verifiable();
            mockUOW
                .Setup(x => x.Movies.IsInGenreAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.RemoveGenreAsync(_testMovies[0].Id, _testGenres[0].Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            mockUOW.Verify();
        }

        [Test]
        public async Task RemoveGenreAsync_MovieNotInGenre_DoesNotDelete() {
            // Assert
            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.RemoveFromGenre(It.IsAny<MovieGenre>()))
                .Throws(new Exception("Method shouldn't be called"));
            mockUOW
                .Setup(x => x.Movies.IsInGenreAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.RemoveGenreAsync(_testMovies[0].Id, _testGenres[0].Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion RemoveGenreAsync tests

        #region RemoveCountryAsync tests

        [Test]
        public async Task RemoveCountryAsync_MovieInCountry_Deletes() {
            // Assert
            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(x => x.Movies.RemoveFromCountry(It.IsAny<MovieCountry>())).Verifiable();
            mockUOW
                .Setup(x => x.Movies.IsInCountryAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.RemoveCountryAsync(_testMovies[0].Id, _testCountries[0].Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            mockUOW.Verify();
        }

        [Test]
        public async Task RemoveCountryAsync_MovieNotInCountry_DoesNotDelete() {
            // Assert
            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.RemoveFromCountry(It.IsAny<MovieCountry>()))
                .Throws(new Exception("Method shouldn't be called"));
            mockUOW
                .Setup(x => x.Movies.IsInCountryAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.RemoveGenreAsync(_testMovies[0].Id, _testCountries[0].Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion RemoveCountryAsync tests

        #region RemoveArtistAsync tests

        [Test]
        public async Task RemoveArtistAsync_MovieArtistNotNull_Deletes() {
            // Assert
            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(x => x.Movies.RemoveFromArtistAndCareer(It.IsAny<MovieArtist>())).Verifiable();
            mockUOW
                .Setup(x => x.Movies.GetByMovieAndArtistAndCareerAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CareerEnum>()))
                .ReturnsAsync(_testMovies[0].MovieArtists.First());

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.RemoveArtistAsync(_testMovies[0].Id, new MovieArtistDto(_testArtists[0].Id, CareerEnum.Actor, 1));

            // Assert
            Assert.IsTrue(result.IsSuccess);
            mockUOW.Verify();
        }

        [Test]
        public async Task RemoveArtistAsync_MovieArtistNull_DoesNotDelete() {
            // Assert
            var mockFileClient = new Mock<IFileClient>();
            var mockFileConfigOptions = new Mock<IOptions<MoviePosterFileConfiguration>>();
            mockFileConfigOptions
                .SetupGet(x => x.Value)
                .Returns(new MoviePosterFileConfiguration());

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Movies.RemoveFromArtistAndCareer(It.IsAny<MovieArtist>()))
                .Throws(new Exception("Method shouldn't be called"));
            mockUOW
                .Setup(x => x.Movies.GetByMovieAndArtistAndCareerAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CareerEnum>()))
                .ReturnsAsync((MovieArtist)null);

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            // Act
            var result = await _sut.RemoveArtistAsync(_testMovies[0].Id, new MovieArtistDto(_testArtists[0].Id, CareerEnum.Actor, 1));

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        #endregion RemoveArtistAsync tests
    }
}
