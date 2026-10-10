using Moq;
using BookTracker.Common;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.DBManagers;
using BookTracker.DAL.Entities.Authors;
using BookTracker.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Tests.Features.AuthorDropdown
{
    [Trait("Category", "Accept_AuthorDropdown")]
    public class AuthorDropdownTests
    {
        private readonly Mock<IDbContextFactory<BooksDbContext>> _mockContextFactory;
        private readonly AuthorDbManager _manager;
        private readonly string _dbName = "TestDb_" + Guid.NewGuid();
        private readonly DbContextOptions<BooksDbContext> _options;

        public AuthorDropdownTests()
        {
            _options = new DbContextOptionsBuilder<BooksDbContext>()
                .UseInMemoryDatabase(databaseName: _dbName)
                .Options;

            // The factory will return a context with these options
            _mockContextFactory = new Mock<IDbContextFactory<BooksDbContext>>();
            _mockContextFactory.Setup(f => f.CreateDbContext()).Returns(() => new BooksDbContext(_options));
            _mockContextFactory.Setup(f => f.CreateDbContextAsync(CancellationToken.None))
                .ReturnsAsync(new BooksDbContext(_options));

            _manager = new AuthorDbManager(_mockContextFactory.Object);
        }

        [Fact]
        public async Task GetAuthorsForSearchableDropdown_ReturnsSortedList()
        {
            // Arrange
            var authors = new List<Author>
            {
                new() { Name = "Zebra" },
                new() { Name = "Apple" },
                new() { Name = "Banana" },
                new() { Name = "Author 1" },
                new() { Name = "Author 2" }
            };

            await using (var context = new BooksDbContext(_options))
            {
                context.Authors.AddRange(authors);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _manager.GetAuthorsForSearchableDropdown(null);

            // Assert
            Assert.Equal("Apple", result.First().Name);
            Assert.Equal("Author 1", result.Skip(1).First().Name);
            Assert.Equal("Author 2", result.Skip(2).First().Name);
            Assert.Equal("Banana", result.Skip(3).First().Name);
            Assert.Equal("Zebra", result.Last().Name);
        }

        [Fact]
        public async Task GetAuthorsForSearchableDropdown_FiltersBySearchTerm()
        {
            // Arrange
            var authors = new List<Author>
            {
                new() { Name = "Apple" },
                new() { Name = "Banana" },
                new() { Name = "Cherry" }
            };

            await using (var context = new BooksDbContext(_options))
            {
                context.Authors.AddRange(authors);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _manager.GetAuthorsForSearchableDropdown("Banana");

            // Assert
            Assert.Single(result);
            Assert.Equal("Banana", result.First().Name);
        }

        [Fact]
        public async Task GetAuthorsForSearchableDropdown_AppliesDisplayLimit()
        {
            // Arrange
            await using (var context = new BooksDbContext(_options))
            {
                var authors = new List<Author>();
                for (int i = 0; i < 20; i++)
                {
                    authors.Add(new Author { Name = $"Author {i}" });
                }

                context.Authors.AddRange(authors);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _manager.GetAuthorsForSearchableDropdown(null);

            // Assert
            Assert.Equal(DropdownConstants.InitialDisplayLimit, result.Count());
        }

        [Fact]
        public async Task GetAuthorsForSearchableDropdown_ReturnsEmptyWhenNoMatch()
        {
            // Arrange
            var authors = new List<Author>
            {
                new() { Name = "Apple" },
                new() { Name = "Banana" },
                new() { Name = "Cherry" }
            };

            await using (var context = new BooksDbContext(_options))
            {
                context.Authors.AddRange(authors);
                await context.SaveChangesAsync();
            }
            
            // Act
            var result = await _manager.GetAuthorsForSearchableDropdown("NonExistentAuthor");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAuthorsForSearchableDropdown_CaseSensitiveSearch()
        {
            // Arrange
            var authors = new List<Author>
            {
                new() { Name = "Apple" },
                new() { Name = "Banana" },
                new() { Name = "Cherry" }
            };

            await using (var context = new BooksDbContext(_options))
            {
                context.Authors.AddRange(authors);
                await context.SaveChangesAsync();
            }
            
            // Act
            var result = await _manager.GetAuthorsForSearchableDropdown("apple");

            // Assert
            Assert.Empty(result); // Search is case-sensitive
        }

        [Fact]
        public async Task GetAuthorsForSearchableDropdown_MatchesMultipleResults()
        {
            // Arrange
            var authors = new List<Author>
            {
                new() { Name = "Author" },
                new() { Name = "Author 2" },
                new() { Name = "Genre" },
                new() { Name = "Genre 2" }
            };

            await using (var context = new BooksDbContext(_options))
            {
                context.Authors.AddRange(authors);
                await context.SaveChangesAsync();
            }
            
            // Act
            var result = await _manager.GetAuthorsForSearchableDropdown("Author");

            // Assert
            Assert.Equal(2, result.Count()); // Author 1, Author 2
        }

        [Fact]
        public async Task GetAuthorsForSearchableDropdown_WhitespacesNotTrimmed()
        {
            // Arrange
            var authors = new List<Author>
            {
                new() { Name = "Apple" }
            };

            await using (var context = new BooksDbContext(_options))
            {
                context.Authors.AddRange(authors);
                await context.SaveChangesAsync();
            }
            // Act
            var result = await _manager.GetAuthorsForSearchableDropdown("  Apple  ");

            // Assert
            Assert.Empty(result); // Whitespace is not trimmed
        }

        [Fact]
        public async Task GetAuthorsForSearchableDropdown_EmptyStringTreatedAsNull()
        {
            // Arrange
            var authors = new List<Author>
            {
                new() { Name = "Author" },
                new() { Name = "Author 2" },
                new() { Name = "Genre" },
                new() { Name = "Genre 2" }
            };

            await using (var context = new BooksDbContext(_options))
            {
                context.Authors.AddRange(authors);
                await context.SaveChangesAsync();
            }
            // Act
            var result = await _manager.GetAuthorsForSearchableDropdown("");

            // Assert
            Assert.Equal(4, result.Count()); // All authors should be returned
        }

        [Fact]
        public async Task GetAuthorsForSearchableDropdown_WithSpecialCharacters()
        {
            // Arrange
            await using (var context = new BooksDbContext(_options))
            {
                context.Authors.Add(new Author { Name = "Author O'Brien" });
                context.Authors.Add(new Author { Name = "Author García" });
                context.SaveChangesAsync();
            }

            // Act
            var result = await _manager.GetAuthorsForSearchableDropdown("O'Brien");

            // Assert
            Assert.Single(result);
            Assert.Equal("Author O'Brien", result.First().Name);
        }
    }
}