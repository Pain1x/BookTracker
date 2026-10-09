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
        public async Task T03_GetGenresForSearchableDropdown_ReturnsEmptyWhenNoMatch()
        {
            // Arrange
            // The constructor already seeds "Action" and "Drama"

            // Act
            var result = await _manager.GetGenresForSearchableDropdown("NonExistent");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task T04_GetGenresForSearchableDropdown_CaseSensitiveSearch()
        {
            // Arrange
            // The constructor already seeds "Action" (with capital A)

            // Act
            var result = await _manager.GetGenresForSearchableDropdown("ACTION");

            // Assert
            Assert.Empty(result); // Search is case-sensitive
        }

        [Fact]
        public async Task T05_GetGenresForSearchableDropdown_PartialMatch()
        {
            // Arrange
            // The constructor already seeds "Action"

            // Act
            var result = await _manager.GetGenresForSearchableDropdown("Act");

            // Assert
            Assert.Single(result);
            Assert.Equal("Action", result.First().Name);
        }

        [Fact]
        public async Task T06_GetGenresForSearchableDropdown_MatchesMultipleResults()
        {
            // Arrange
            // The constructor already seeds "Action" and "Drama"

            // Act
            var result = await _manager.GetGenresForSearchableDropdown("Action");

            // Assert
            Assert.Single(result);
            Assert.Equal("Action", result.First().Name);
        }

        [Fact]
        public async Task T07_GetGenresForSearchableDropdown_WhitespacesNotTrimmed()
        {
            // Arrange
            // The constructor already seeds "Action"

            // Act
            var result = await _manager.GetGenresForSearchableDropdown("  Act  ");

            // Assert
            Assert.Empty(result); // Whitespace is not trimmed, so "  Act  " doesn't match "Action"
        }

        [Fact]
        public async Task T08_GetGenresForSearchableDropdown_EmptyStringTreatedAsNull()
        {
            // Arrange
            // The constructor already seeds all genres

            // Act
            var result = await _manager.GetGenresForSearchableDropdown("");

            // Assert
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public async Task T09_GetGenresForSearchableDropdown_LimitApplied()
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

        [Fact]
        public async Task T10_GetGenresForSearchableDropdown_WithSpecialCharacters()
        {
            // Arrange
            using (var context = new BooksDbContext(_options))
            {
                context.Genres.Add(new Genre { Name = "Sci-Fi" });
                context.Genres.Add(new Genre { Name = "Fantasy" });
                context.SaveChanges();
            }

            // Act
            var result = await _manager.GetGenresForSearchableDropdown("Sci-Fi");

            // Assert
            Assert.Single(result);
            Assert.Equal("Sci-Fi", result.First().Name);
        }
    }
}