using Moq;
using Microsoft.EntityFrameworkCore;
using BookTracker.DAL.DBManagers;
using BookTracker.DAL.Entities.Books;
using BookTracker.DAL.Entities.Authors;
using BookTracker.DAL.Entities.Genres;
using BookTracker.DAL.DBContexts;
using BookTracker.DAL.Entities.Translations;

namespace BookTracker.Tests
{
    public class BookDbManagerTests
    {
        private readonly Mock<IDbContextFactory<BooksDbContext>> _mockContextFactory;
        private readonly BookDbManager _bookDbManager;
        private readonly string _dbName = "TestDb_" + Guid.NewGuid().ToString();

        public BookDbManagerTests()
        {
            var options = new DbContextOptionsBuilder<BooksDbContext>()
                .UseInMemoryDatabase(databaseName: _dbName)
                .Options;

            // The factory will return a context with these options
            _mockContextFactory = new Mock<IDbContextFactory<BooksDbContext>>();
            _mockContextFactory.Setup(f => f.CreateDbContext()).Returns(() => new BooksDbContext(options));
            _mockContextFactory.Setup(f => f.CreateDbContextAsync(CancellationToken.None))
                .ReturnsAsync(new BooksDbContext(options));

            _bookDbManager = new BookDbManager(_mockContextFactory.Object);
        }

        private async Task SeedDataAsync(Action<BooksDbContext> seedAction)
        {
            await using var context = new BooksDbContext(new DbContextOptionsBuilder<BooksDbContext>()
                .UseInMemoryDatabase(databaseName: _dbName)
                .Options);
            seedAction(context);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task AddBook_WhenAuthorAndGenreExist_AddsNewBookSuccessfully()
        {
            // Arrange
            var authorPk = Guid.NewGuid();
            var genrePk = Guid.NewGuid();

            await SeedDataAsync(ctx =>
            {
                ctx.Authors.AddAsync(new Author { AuthorPk = authorPk, Name = "Existing Author" });
                ctx.Genres.AddAsync(new Genre { GenrePk = genrePk, Name = "Fiction" });
            });

            var bookToSave = new Book
            {
                Title = "New Test Book",
                Author = new Author { AuthorPk = authorPk },
                Genre = new Genre { GenrePk = genrePk }
            };

            // Act
            var resultBook = await _bookDbManager.AddBook(bookToSave);

            // Assert
            Assert.NotNull(resultBook);
            Assert.Equal("New Test Book", resultBook.Title);

            await using var context = new BooksDbContext(new DbContextOptionsBuilder<BooksDbContext>()
                .UseInMemoryDatabase(databaseName: _dbName)
                .Options);

            var savedBook = await context.Books.FirstOrDefaultAsync(b => b.BookPk == resultBook.BookPk);
            Assert.NotNull(savedBook);
        }

        [Fact]
        public async Task AddBook_WhenAuthorIsMissingButGenreExists_CreatesAndAddsNewAuthor()
        {
            // Arrange
            var genrePk = Guid.NewGuid();
            await SeedDataAsync(ctx => { ctx.Genres.AddAsync(new Genre { GenrePk = genrePk, Name = "Fiction" }); });

            var bookToSave = new Book
            {
                Title = "Test Book",
                Author = new Author { Name = "New Author" },
                Genre = new Genre { GenrePk = genrePk }
            };

            // Act
            var resultBook = await _bookDbManager.AddBook(bookToSave);

            // Assert
            Assert.NotNull(resultBook);
            await using var context = new BooksDbContext(new DbContextOptionsBuilder<BooksDbContext>()
                .UseInMemoryDatabase(databaseName: _dbName)
                .Options);

            var savedAuthor = await context.Authors.FirstOrDefaultAsync(a => a.Name == "New Author");
            Assert.NotNull(savedAuthor);
        }

        [Fact]
        public async Task UpdateBook_WhenInputMatchesExisting_UpdatesSuccessfully()
        {
            // Arrange
            var bookPk = Guid.NewGuid();
            var authorPk = Guid.NewGuid();
            var genrePk = Guid.NewGuid();

            await SeedDataAsync(ctx =>
            {
                var author = new Author { AuthorPk = authorPk, Name = "Author" };
                var genre = new Genre { GenrePk = genrePk, Name = "Genre" };
                ctx.Authors.Add(author);
                ctx.Genres.Add(genre);
                ctx.Books.Add(new Book
                {
                    BookPk = bookPk,
                    Title = "Original Title",
                    Rating = 5,
                    Author = author,
                    Genre = genre
                });
            });

            var updatedBook = new Book
            {
                BookPk = bookPk,
                Title = "Updated Title",
                Rating = 4,
                Author = new Author { AuthorPk = authorPk },
                Genre = new Genre { GenrePk = genrePk }
            };

            // Act
            await _bookDbManager.UpdateBook(updatedBook);

            // Assert
            await using var context = new BooksDbContext(new DbContextOptionsBuilder<BooksDbContext>()
                .UseInMemoryDatabase(databaseName: _dbName)
                .Options);

            var result = await context.Books.FindAsync(bookPk);
            Assert.Equal("Updated Title", result?.Title);
            Assert.Equal(4, result?.Rating);
        }

        [Fact]
        public async Task GetAllBooksLocalized_WhenBooksExist_ReturnsCorrectList()
        {
            // Arrange
            byte languagePk = 1;
            var book1 = new Book
            {
                Title = "Book A", Author = new Author(), Genre = new Genre(), DateRead = DateTime.UtcNow, Rating = 5
            };
            var book2 = new Book
            {
                Title = "Book B", Author = new Author(), Genre = new Genre(), DateRead = DateTime.UtcNow.AddDays(1),
                Rating = 4
            };

            await SeedDataAsync(ctx =>
            {
                ctx.Books.AddRange(book1, book2);
                // Add translations for languagePk = 1
                ctx.BookTranslations.AddAsync(new BookTranslation
                    { BookPk = book1.BookPk, LanguagePk = languagePk, Title = "Book A (L1)" });
                ctx.BookTranslations.AddAsync(new BookTranslation
                    { BookPk = book2.BookPk, LanguagePk = languagePk, Title = "Book B (L1)" });
            });

            // Act
            var result = await _bookDbManager.GetAllBooksLocalized(languagePk);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, b => b.Title == "Book A (L1)");
            Assert.Contains(result, b => b.Title == "Book B (L1)");
        }

        [Fact]
        public async Task FindBookByPkLocalized_WhenBookExists_ReturnsCorrectBook()
        {
            // Arrange
            var bookPk = Guid.NewGuid();
            var languagePk = (byte)1;
            var author = new Author { AuthorPk = Guid.NewGuid(), Name = "Author" };
            var genre = new Genre { GenrePk = Guid.NewGuid(), Name = "Genre" };

            await SeedDataAsync(ctx =>
            {
                ctx.Books.Add(new Book
                {
                    BookPk = bookPk,
                    Title = "Original Title",
                    Author = author,
                    Genre = genre,
                    DateRead = DateTime.UtcNow,
                    Rating = 5
                });
                ctx.BookTranslations.Add(new BookTranslation
                    { BookPk = bookPk, LanguagePk = languagePk, Title = "Localized Title" });
            });

            // Act
            var result = await _bookDbManager.FindBookByPkLocalized(bookPk, languagePk);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Localized Title", result.Title);
        }

        [Fact]
        public async Task FindBookByPkLocalized_WhenBookDoesNotExist_ReturnsNull()
        {
            // Arrange
            var bookPk = Guid.NewGuid();
            var languagePk = (byte)1;

            // Act
            var result = await _bookDbManager.FindBookByPkLocalized(bookPk, languagePk);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddBook_WhenAuthorExistsButGenreIsMissing_CreatesAndAddsNewGenre()
        {
            // Arrange
            var authorPk = Guid.NewGuid();
            var genrePk = Guid.NewGuid();

            await SeedDataAsync(ctx =>
            {
                ctx.Authors.AddAsync(new Author { AuthorPk = authorPk, Name = "Existing Author" });
                ctx.Genres.AddAsync(new Genre { GenrePk = genrePk, Name = "Fiction" });
            });

            var bookToSave = new Book
            {
                Title = "New Test Book",
                Author = new Author { AuthorPk = authorPk },
                Genre = new Genre { GenrePk = genrePk }
            };

            // Act
            var resultBook = await _bookDbManager.AddBook(bookToSave);

            // Assert
            Assert.NotNull(resultBook);
            await using var context = new BooksDbContext(new DbContextOptionsBuilder<BooksDbContext>()
                .UseInMemoryDatabase(databaseName: _dbName)
                .Options);

            var savedGenre = await context.Genres.FirstOrDefaultAsync(g => g.GenrePk == genrePk);
            Assert.NotNull(savedGenre);
        }

        [Fact]
        public async Task AddBook_WhenBothAuthorAndGenreAreMissing_CreatesAndAddsBoth()
        {
            // Arrange
            var bookToSave = new Book
            {
                Title = "New Test Book",
                Author = new Author { Name = "New Author" },
                Genre = new Genre { Name = "New Genre" }
            };

            // Act
            var resultBook = await _bookDbManager.AddBook(bookToSave);

            // Assert
            Assert.NotNull(resultBook);
            await using var context = new BooksDbContext(new DbContextOptionsBuilder<BooksDbContext>()
                .UseInMemoryDatabase(databaseName: _dbName)
                .Options);

            var savedAuthor = await context.Authors.FirstOrDefaultAsync(a => a.Name == "New Author");
            var savedGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "New Genre");
            Assert.NotNull(savedAuthor);
            Assert.NotNull(savedGenre);
        }

        [Fact]
        public async Task AddBook_WithNullDateRead_SavesSuccessfully()
        {
            // Arrange
            var authorPk = Guid.NewGuid();
            var genrePk = Guid.NewGuid();

            await SeedDataAsync(ctx =>
            {
                ctx.Authors.AddAsync(new Author { AuthorPk = authorPk, Name = "Existing Author" });
                ctx.Genres.AddAsync(new Genre { GenrePk = genrePk, Name = "Fiction" });
            });

            var bookToSave = new Book
            {
                Title = "New Test Book",
                Author = new Author { AuthorPk = authorPk },
                Genre = new Genre { GenrePk = genrePk },
                DateRead = null
            };

            // Act
            var resultBook = await _bookDbManager.AddBook(bookToSave);

            // Assert
            Assert.NotNull(resultBook);
            Assert.Null(resultBook.DateRead);
        }

        [Fact]
        public async Task GetAllBooksLocalized_WhenNoBooksExist_ReturnsEmptyList()
        {
            // Act
            var result = await _bookDbManager.GetAllBooksLocalized(1);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllBooksLocalized_WhenTranslationsAreMissing_ReturnsOriginalValues()
        {
            // Arrange
            var book = new Book
            {
                Title = "Original Title",
                Author = new Author { Name = "Original Author" },
                Genre = new Genre { Name = "Original Genre" },
                DateRead = DateTime.UtcNow,
                Rating = 5
            };

            await SeedDataAsync(ctx => ctx.Books.Add(book));

            // Act
            var result = await _bookDbManager.GetAllBooksLocalized(1);

            // Assert
            Assert.Single(result);
            Assert.Equal("Original Title", result[0].Title);
            Assert.Equal("Original Author", result[0].Author.Name);
            Assert.Equal("Original Genre", result[0].Genre.Name);
        }
        
        [Fact]
        public async Task GetAllBooksLocalized_ForDifferentLanguagePk_ReturnsCorrectData()
        {
            // Arrange
            var book = new Book { Title = "Base Title", Author = new Author(), Genre = new Genre() };
            byte langPk = 2;

            await SeedDataAsync(ctx =>
            {
                ctx.Books.Add(book);
                ctx.BookTranslations.Add(new BookTranslation { BookPk = book.BookPk, LanguagePk = langPk, Title = "Translated Title" });
            });

            // Act
            var result = await _bookDbManager.GetAllBooksLocalized(langPk);

            // Assert
            Assert.Single(result);
            Assert.Equal("Translated Title", result[0].Title);
        }

        [Fact]
        public async Task FindBookByPkLocalized_WhenTranslationIsMissing_ReturnsOriginalValues()
        {
            // Arrange
            var book = new Book { Title = "Original Title", Author = new Author { Name = "Author" }, Genre = new Genre { Name = "Genre" } };
            await SeedDataAsync(ctx => ctx.Books.Add(book));

            // Act
            var result = await _bookDbManager.FindBookByPkLocalized(book.BookPk, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Original Title", result.Title);
        }

        [Fact]
        public async Task CountBooksByYears_WhenNoBooksExist_ReturnsAllZeros()
        {
            // Act
            var result = await _bookDbManager.CountBooksByYears();

            // Assert
            var currentYear = DateTime.UtcNow.Year;
            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(0, result[currentYear - i]);
            }
        }

        [Fact]
        public async Task CountBooksByYears_WithMultipleBooksInSameYear_ReturnsCorrectCount()
        {
            // Arrange
            var year = DateTime.UtcNow.Year;
            await SeedDataAsync(ctx =>
            {
                ctx.Books.Add(new Book { DateRead = new DateTime(year, 1, 1), Author = new Author(), Genre = new Genre() });
                ctx.Books.Add(new Book { DateRead = new DateTime(year, 1, 1), Author = new Author(), Genre = new Genre() });
            });

            // Act
            var result = await _bookDbManager.CountBooksByYears();

            // Assert
            Assert.Equal(2, result[year]);
        }

        [Fact]
        public async Task CountBooksByYears_WithBooksInDifferentYears_ReturnsCorrectCounts()
        {
            // Arrange
            var year1 = DateTime.UtcNow.Year;
            var year2 = DateTime.UtcNow.Year - 1;
            await SeedDataAsync(ctx =>
            {
                ctx.Books.Add(new Book { DateRead = new DateTime(year1, 1, 1) , Author = new Author(), Genre = new Genre()});
                ctx.Books.Add(new Book { DateRead = new DateTime(year2, 1, 1) , Author = new Author(), Genre = new Genre()});
            });

            // Act
            var result = await _bookDbManager.CountBooksByYears();

            // Assert
            Assert.Equal(1, result[year1]);
            Assert.Equal(1, result[year2]);
        }

        [Fact]
        public async Task UpdateBook_UpdatesRatingAndNotesSuccessfully()
        {
            // Arrange
            var bookPk = Guid.NewGuid();
            await SeedDataAsync(ctx =>
            {
                ctx.Books.Add(new Book { BookPk = bookPk, Rating = 1, Notes = "Old", Author = new Author(), Genre = new Genre() });
            });

            var updatedBook = new Book
            {
                BookPk = bookPk,
                Rating = 5,
                Notes = "New",
                Author = new Author(),
                Genre = new Genre()
            };

            // Act
            await _bookDbManager.UpdateBook(updatedBook);

            // Assert
            await using var context = new BooksDbContext(new DbContextOptionsBuilder<BooksDbContext>()
                .UseInMemoryDatabase(databaseName: _dbName)
                .Options);
            var result = await context.Books.FindAsync(bookPk);
            Assert.Equal(5, result?.Rating);
            Assert.Equal("New", result?.Notes);
        }

        [Fact]
        public async Task CountBooksByYears_WithBooksInCurrentYear_ReturnsCorrectCounts()
        {
            // Arrange
            var currentYear = DateTime.UtcNow.Year;
            await SeedDataAsync(ctx =>
            {
                ctx.Books.Add(new Book { DateRead = new DateTime(currentYear, 1, 1) , Author = new Author(), Genre = new Genre()});
            });

            // Act
            var result = await _bookDbManager.CountBooksByYears();

            // Assert
            Assert.Equal(1, result[currentYear]);
        }

        [Fact]
        public async Task UpdateBook_UpdatesTitleSuccessfully()
        {
            // Arrange
            var bookPk = Guid.NewGuid();
            await SeedDataAsync(ctx =>
            {
                ctx.Books.Add(new Book { BookPk = bookPk, Title = "Old Title" , Author = new Author(), Genre = new Genre()});
            });

            var updatedBook = new Book
            {
                BookPk = bookPk,
                Title = "New Title",
                Author = new Author(),
                Genre = new Genre()
            };

            // Act
            await _bookDbManager.UpdateBook(updatedBook);

            // Assert
            await using var context = new BooksDbContext(new DbContextOptionsBuilder<BooksDbContext>()
                .UseInMemoryDatabase(databaseName: _dbName)
                .Options);
            var result = await context.Books.FindAsync(bookPk);
            Assert.Equal("New Title", result?.Title);
        }
    }
}