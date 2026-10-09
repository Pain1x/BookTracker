using Moq;
using BookTracker.Common;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.DBManagers;
using BookTracker.DAL.Entities.Genres;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Tests.Features.GenreDropdown
{
    [Trait("Category", "Accept_GenreDropdown")]
    public class GenreDropdownTests
    {
        private readonly Mock<IDbContextFactory<BooksDbContext>> _mockContextFactory;
        private readonly GenreDbManager _manager;
        private readonly string _dbName = "TestDb_" + Guid.NewGuid();
        private readonly DbContextOptions<BooksDbContext> _options;

        public GenreDropdownTests()
        {
            _options = new DbContextOptionsBuilder<BooksDbContext>()
                .UseInMemoryDatabase(databaseName: _dbName)
                .Options;

            // The factory will return a context with these options
            _mockContextFactory = new Mock<IDbContextFactory<BooksDbContext>>();
            _mockContextFactory.Setup(f => f.CreateDbContext()).Returns(() => new BooksDbContext(_options));
            _mockContextFactory.Setup(f => f.CreateDbContextAsync(CancellationToken.None))
                .ReturnsAsync(new BooksDbContext(_options));

            _manager = new GenreDbManager(_mockContextFactory.Object);

            // Seed the database with sample data
            using (var context = new BooksDbContext(_options))
            {
                context.Genres.AddRange(new List<Genre>
                {
                    new() { Name = "Z"},
                    new() { Name = "A"},
                    new() { Name = "Action"},
                    new() { Name = "Drama"}
                });
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task T01_GetGenresForSearchableDropdown_ReturnsSortedList()
        {
            // Arrange
            // The constructor already seeds "Z" and "A"

            // Act
            var result = await _manager.GetGenresForSearchableDropdown(null);

            // Assert
            Assert.Equal("A", result.First().Name);
        }

        [Fact]
        public async Task T02_GetGenresForSearchableDropdown_FiltersByTerm()
        {
            // Arrange
            // The constructor already seeds "Drama"

            // Act
            var result = await _manager.GetGenresForSearchableDropdown("Drama");

            // Assert
            Assert.Single(result);
            Assert.Equal("Drama", result.First().Name);
        }

        [Fact]
        public async Task T03_GetGenresForSearchableDropdown_AppliesLimit()
        {
            // Arrange
            using (var context = new BooksDbContext(_options))
            {
                var genres = new List<Genre>();
                for (int i = 0; i < 20; i++) genres.Add(new Genre { Name = $"Genre {i}" });
                context.Genres.AddRange(genres);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _manager.GetGenresForSearchableDropdown(null);

            // Assert
            Assert.Equal(DropdownConstants.InitialDisplayLimit, result.Count());
        }
    }
}
