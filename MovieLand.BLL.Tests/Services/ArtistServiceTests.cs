using AutoMapper;
using ExpectedObjects;
using Moq;
using MovieLand.BLL.Dtos.Artist;
using MovieLand.BLL.Dtos.DataTables;
using MovieLand.BLL.Services;
using MovieLand.Data.Builders;
using MovieLand.Data.Contracts.Repositories;
using MovieLand.Data.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieLand.BLL.Tests.Services
{
    [TestFixture]
    class ArtistServiceTests
    {
        private ArtistService _sut;
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

        private readonly Artist[] _testArtists = new Artist[5] {
            new Artist() { Id = Guid.NewGuid(), Name = "Johnny Depp" },
            new Artist() { Id = Guid.NewGuid(), Name = "Leonardo DiCaprio" },
            new Artist() { Id = Guid.NewGuid(), Name = "Angelina Jolie" },
            new Artist() { Id = Guid.NewGuid(), Name = "Charlize Theron" },
            new Artist() { Id = Guid.NewGuid(), Name = "Natalie Portman" }
        };

        #region SaveAsync tests

        [Test]
        public async Task SaveAsync_EmptyIdAndUniqueName_CreatesNew()
        {
            // Arrange
            var newArtist = new Artist()
            {
                Id = Guid.NewGuid(),
                Name = "New Artist"
            };
            var expectedValue = new ArtistDto()
            {
                Id = newArtist.Id,
                Name = newArtist.Name
            }.ToExpectedObject();

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Artists.GetAsync(It.IsAny<EntityQueryBuilder<Artist>>()))
                .ReturnsAsync(new Artist[0]);

            mockUOW
                .Setup(x => x.Artists.AddAsync(It.IsAny<Artist>()))
                .ReturnsAsync(newArtist);

            mockUOW.Setup(x => x.CompleteAsync());

            _sut = new ArtistService(_mapper, mockUOW.Object);

            // Act
            var request = new ArtistDto() { Name = newArtist.Name };
            var result = await _sut.SaveAsync(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            expectedValue.ShouldEqual(result.Entity);
        }

        [Test]
        public async Task SaveAsync_NotUniqueName_ReturnsExistingArtist()
        {
            // Arrange
            var existingArtist = _testArtists[0];
            var expectedValue = new ArtistDto()
            {
                Id = existingArtist.Id,
                Name = existingArtist.Name
            }.ToExpectedObject();

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Artists.GetAsync(It.IsAny<EntityQueryBuilder<Artist>>()))
                .ReturnsAsync(new Artist[1] { existingArtist });

            mockUOW
                .Setup(x => x.Artists.AddAsync(It.IsAny<Artist>()))
                .ReturnsAsync(existingArtist);

            _sut = new ArtistService(_mapper, mockUOW.Object);

            // Act
            var request = new ArtistDto() { Name = existingArtist.Name };
            var result = await _sut.SaveAsync(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            expectedValue.ShouldEqual(result.Entity);
        }

        [Test]
        public async Task SaveAsync_NotEmptyIdAndUniqueName_UpdatesArtist()
        {
            // Arrange
            var existingArtist = _testArtists[0];
            var newName = "New Artist name";

            var expectedValue = new ArtistDto()
            {
                Id = existingArtist.Id,
                Name = newName
            }.ToExpectedObject();

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Artists.GetAsync(It.IsAny<EntityQueryBuilder<Artist>>()))
                .ReturnsAsync(new Artist[0]);

            mockUOW.Setup(x => x.Artists.Update(It.IsAny<Artist>()));
            mockUOW.Setup(x => x.CompleteAsync());

            _sut = new ArtistService(_mapper, mockUOW.Object);

            // Act
            var request = new ArtistDto() { Id = existingArtist.Id, Name = newName };
            var result = await _sut.SaveAsync(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            expectedValue.ShouldEqual(result.Entity);
        }

        [Test]
        public async Task SaveAsync_ExceptionInRepoCompleteAsync_Error()
        {
            var expectedException = "An error occurred while updating the entries.";

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Artists.GetAsync(It.IsAny<EntityQueryBuilder<Artist>>()))
                .ReturnsAsync(new Artist[0]);

            mockUOW.Setup(x => x.Artists.Update(It.IsAny<Artist>()));
            mockUOW.Setup(x => x.CompleteAsync()).ThrowsAsync(new Exception(expectedException));

            _sut = new ArtistService(_mapper, mockUOW.Object);

            // Act
            var request = new ArtistDto() { Id = Guid.Empty, Name = "" };
            var result = await _sut.SaveAsync(request);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(expectedException, result.Errors.First());
        }

        #endregion SaveAsync tests

        #region GetByIdAsync tests

        [Test]
        public async Task GetByIdAsync_ArtistExists_Dto()
        {
            // Arrange
            var expectedValue = new ArtistDto() { 
                Id = _testArtists[0].Id, 
                Name = _testArtists[0].Name 
            }.ToExpectedObject(); 

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Artists.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(_testArtists[0]);

            _sut = new ArtistService(_mapper, mockUOW.Object);

            // Action
            var result = await _sut.GetByIdAsync(_testArtists[0].Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            expectedValue.ShouldEqual(result.Entity);
        }

        [Test]
        public async Task GetByIdAsync_ArtistNotExists_Null()
        {
            // Arrange
            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Artists.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Artist)null);

            _sut = new ArtistService(_mapper, mockUOW.Object);

            // Action
            var result = await _sut.GetByIdAsync(Guid.Empty);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(null, result.Entity);
        }

        [Test]
        public async Task GetById_ExceptionInRepoGetByIdAsync_Error()
        {
            // Arrange
            var expectedException = "An error occurred while executing query.";

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Artists.GetByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception(expectedException));

            _sut = new ArtistService(_mapper, mockUOW.Object);

            // Action
            var result = await _sut.GetByIdAsync(Guid.Empty);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(expectedException, result.Errors.First());
        }

        #endregion GetByIdAsync tests      

        #region GetAsync tests

        [Test]
        public async Task GetAsync_FullDataTableParameters_FiveArtistListItems()
        {
            // Arrange
            var expectedItems = _mapper.Map<ArtistDto[]>(_testArtists);
            var expectedResult = new DataTablesPagedResults<ArtistDto>
            {
                Items = expectedItems,
                TotalSize = expectedItems.Length
            };

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Artists.GetAsync(It.IsAny<EntityQueryBuilder<Artist>>()))
                .ReturnsAsync(_testArtists);

            mockUOW.Setup(x => x.Artists.CountAsync())
                .ReturnsAsync(_testArtists.Length);

            _sut = new ArtistService(_mapper, mockUOW.Object);

            // Act
            var request = new DataTablesParameters()
            {
                Columns = new DTColumn[1] { new DTColumn() { Data = "Name", Name = "Name" } },
                Search = new DTSearch() { Value = "Leo" },
                Order = new DTOrder[1] { new DTOrder() { Column = 0, Dir = DTOrderDir.ASC } },
                Length = 3,
                Start = 1
            };
            var result = await _sut.GetAsync(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedResult.TotalSize, result.Entity.TotalSize);
            expectedResult.Items.First().ToExpectedObject().ShouldEqual(result.Entity.Items.First());
        }

        [Test]
        public async Task GetAsync_ExceptionInRepoCountAsync_Error()
        {
            // Arrange
            var expectedException = "An error occurred while executing query.";

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW.Setup(x => x.Artists.CountAsync())
                .ThrowsAsync(new Exception(expectedException));

            _sut = new ArtistService(_mapper, mockUOW.Object);

            // Act
            var request = new DataTablesParameters();
            var result = await _sut.GetAsync(request);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(expectedException, result.Errors.First());
        }

        [Test]
        public async Task GetAsync_EmptyDataTableParameters_FiveArtistListItems()
        {
            // Arrange
            var expectedItems = _mapper.Map<ArtistDto[]>(_testArtists);
            var expectedResult = new DataTablesPagedResults<ArtistDto>
            {
                Items = expectedItems,
                TotalSize = expectedItems.Length
            };

            var mockUOW = new Mock<IUnitOfWork>();
            mockUOW
                .Setup(x => x.Artists.GetAsync(It.IsAny<EntityQueryBuilder<Artist>>()))
                .ReturnsAsync(_testArtists);

            mockUOW.Setup(x => x.Artists.CountAsync())
                .ReturnsAsync(_testArtists.Length);

            _sut = new ArtistService(_mapper, mockUOW.Object);

            // Act
            var request = new DataTablesParameters();
            var result = await _sut.GetAsync(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedResult.TotalSize, result.Entity.TotalSize);
            expectedResult.Items.First().ToExpectedObject().ShouldEqual(result.Entity.Items.First());
        }

        [Test]
        public async Task GetAsync_NullDataTableParameters_ArgumentNullException()
        {
            // Arrange
            var mockUOW = new Mock<IUnitOfWork>();
            _sut = new ArtistService(_mapper, mockUOW.Object);

            // Act
            var result = await _sut.GetAsync(null);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Value cannot be null. (Parameter 'table')", result.Errors.First());
        }

        #endregion GetAsync tests
    }
}
