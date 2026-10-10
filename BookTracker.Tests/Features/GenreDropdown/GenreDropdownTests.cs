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
        }

        [Fact]
        public async Task GetGenresForSearchableDropdown_ReturnsSortedList()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new() { Name = "Zombie" },
                new() { Name = "Action" },
                new() { Name = "Comedy" },
                new() { Name = "Drama" }
            };

            using (var context = new BooksDbContext(_options))
            {
                context.Genres.AddRange(genres);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _manager.GetGenresForSearchableDropdown(null);

            // Assert
            Assert.Equal("Action", result.First().Name);
            Assert.Equal("Comedy", result.Skip(1).First().Name);
            Assert.Equal("Drama", result.Skip(2).First().Name);
            Assert.Equal("Zombie", result.Last().Name);
        }

        [Fact]
        public async Task GetGenresForSearchableDropdown_FiltersBySearchTerm()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new() { Name = "Fiction" },
                new() { Name = "Non-Fiction" },
                new() { Name = "Science" }
            };

            await using (var context = new BooksDbContext(_options))
            {
                context.Genres.AddRange(genres);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _manager.GetGenresForSearchableDropdown("F");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // "Fiction" and "Non-Fiction" contain "F"
            Assert.Contains("Fiction", result.FirstOrDefault().Name);
        }

        [Fact]
        public async Task GetGenresForSearchableDropdown_ReturnsEmptyWhenNoMatch()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new() { Name = "Fiction" },
                new() { Name = "Non-Fiction" },
                new() { Name = "Science" }
            };

            await using (var context = new BooksDbContext(_options))
            {
                context.Genres.AddRange(genres);
                await context.SaveChangesAsync();
            }
            // Act
            var result = await _manager.GetGenresForSearchableDropdown("NonExistent");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGenresForSearchableDropdown_CaseSensitiveSearch()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new() { Name = "Fiction" },
                new() { Name = "Non-Fiction" },
                new() { Name = "Action" }
            };

            await using (var context = new BooksDbContext(_options))
            {
                context.Genres.AddRange(genres);
                await context.SaveChangesAsync();
            }
            // Act
            var result = await _manager.GetGenresForSearchableDropdown("ACTION");

            // Assert
            Assert.Empty(result); // Search is case-sensitive
        }

        [Fact]
        public async Task GetGenresForSearchableDropdown_PartialMatch()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new() { Name = "Action" }
            };

            await using (var context = new BooksDbContext(_options))
            {
                context.Genres.AddRange(genres);
                await context.SaveChangesAsync();
            }
            
            // Act
            var result = await _manager.GetGenresForSearchableDropdown("Act");

            // Assert
            Assert.Single(result);
            Assert.Equal("Action", result.First().Name);
        }

        [Fact]
        public async Task GetGenresForSearchableDropdown_MatchesMultipleResults()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new() { Name = "Action" },
                new() { Name = "Action 2" },
                new() { Name = "Non-fiction" },
                new() { Name = "Fiction" }
            };

            await using (var context = new BooksDbContext(_options))
            {
                context.Genres.AddRange(genres);
                await context.SaveChangesAsync();
            }
            
            // Act
            var result = await _manager.GetGenresForSearchableDropdown("Action");

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetGenresForSearchableDropdown_WhitespacesNotTrimmed()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new() { Name = "Action" },
                new() { Name = "Action 2" },
                new() { Name = "Non-fiction" },
                new() { Name = "Fiction" }
            };

            await using (var context = new BooksDbContext(_options))
            {
                context.Genres.AddRange(genres);
                await context.SaveChangesAsync();
            }
            
            // Act
            var result = await _manager.GetGenresForSearchableDropdown("  Act  ");

            // Assert
            Assert.Empty(result); // Whitespace is not trimmed, so "  Act  " doesn't match "Action"
        }

        [Fact]
        public async Task GetGenresForSearchableDropdown_EmptyStringTreatedAsNull()
        {
            // Arrange
            var genres = new List<Genre>
            {
                new() { Name = "Action" },
                new() { Name = "Action 2" },
                new() { Name = "Non-fiction" },
                new() { Name = "Fiction" }
            };

            await using (var context = new BooksDbContext(_options))
            {
                context.Genres.AddRange(genres);
                await context.SaveChangesAsync();
            }
            // Act
            var result = await _manager.GetGenresForSearchableDropdown("");

            // Assert
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public async Task GetGenresForSearchableDropdown_LimitApplied()
        {
            // Arrange
            using (var context = new BooksDbContext(_options))
            {
                var genres = new List<Genre>();
                for (int i = 0; i < 20; i++)
                {
                    genres.Add(new Genre { Name = $"Genre {i}" });
                }

                context.Genres.AddRange(genres);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _manager.GetGenresForSearchableDropdown(null);

            // Assert
            Assert.Equal(DropdownConstants.InitialDisplayLimit, result.Count());
        }

        [Fact]
        public async Task GetGenresForSearchableDropdown_WithSpecialCharacters()
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