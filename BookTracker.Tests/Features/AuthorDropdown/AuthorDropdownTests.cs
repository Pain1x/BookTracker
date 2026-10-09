using Moq;
using BookTracker.Common;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.DBManagers;
using BookTracker.DAL.Entities.Authors;
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

            // Seed the database with sample data
            using (var context = new BooksDbContext(_options))
            {
                context.Authors.AddRange(new List<Author>
                {
                    new() { Name = "Zebra"},
                    new() { Name = "Apple"},
                    new() { Name = "Banana"},
                    new() { Name = "Author 1"},
                    new() { Name = "Author 2",}
                });
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task T01_GetAuthorsForSearchableDropdown_ReturnsSortedList()
        {
            // Arrange
            var authors = new List<Author>
            {
                new() { Name = "Zebra"},
                new() { Name = "Apple"}
            };

            // Act
            var result = await _manager.GetAuthorsForSearchableDropdown(null);

            // Assert
            Assert.Equal("Apple", result.First().Name);
        }

        [Fact]
        public async Task T02_GetAuthorsForSearchableDropdown_FiltersByTerm()
        {
            // Arrange
            var authors = new List<Author>
            {
                new() { Name = "Apple" },
                new() { Name = "Banana" }
            };

            // Act
            var result = await _manager.GetAuthorsForSearchableDropdown("Banana");

            // Assert
            Assert.Single(result);
            Assert.Equal("Banana", result.First().Name);
        }

        [Fact]
        public async Task T03_GetAuthorsForSearchableDropdown_AppliesLimit()
        {
            // Arrange
            using (var context = new BooksDbContext(_options))
            {
                var authors = new List<Author>();
                for (int i = 0; i < 20; i++) authors.Add(new Author { Name = $"Author {i}" });
                context.Authors.AddRange(authors);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _manager.GetAuthorsForSearchableDropdown(null);

            // Assert
            Assert.Equal(DropdownConstants.InitialDisplayLimit, result.Count());
        }
    }
}
