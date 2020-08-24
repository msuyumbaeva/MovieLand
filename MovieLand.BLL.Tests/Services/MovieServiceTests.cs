using AutoMapper;
using ExpectedObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Options;
using Moq;
using MovieLand.BLL.Configurations;
using MovieLand.BLL.Contracts;
using MovieLand.BLL.Dtos.Movie;
using MovieLand.BLL.Services;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
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
                Poster = "moviePoster.jpeg"
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
                .ReturnsAsync(newMovie);

            mockUOW.Setup(x => x.CompleteAsync());

            _sut = new MovieService(_mapper, mockUOW.Object, mockFileConfigOptions.Object, mockFileClient.Object);

            var sourceImg = File.OpenRead(@"C:\Users\E7450\source\repos\MovieLand\MovieLand\wwwroot\posters\no-movie-poster.jpg");
            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.SetupGet(x => x.ContentType).Returns("image/jpeg");
            mockFormFile.SetupGet(x => x.FileName).Returns("no-movie-poster.jpg");
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
    }
}
