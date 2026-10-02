using Moq;
using Microsoft.EntityFrameworkCore;
using BookTracker.DAL.DBManagers;
using BookTracker.DAL.Entities.Books;
using BookTracker.DAL.Entities.Authors;
using BookTracker.DAL.Entities.Genres;
using BookTracker.DAL.DBContexts;

namespace BookTracker.Tests
{
    public class BookDbManagerTests
    {
        private readonly Mock<IDbContextFactory<BooksDbContext>> _mockContextFactory;
        private readonly Mock<BooksDbContext> _mockContextMock;
        private readonly BookDbManager _bookDbManager;

        public BookDbManagerTests()
        {
            _mockContextMock = new Mock<BooksDbContext>();
            
            // --- Simplified Mocking Strategy: Use in-memory lists instead of complex async providers ---
            
            // Setup mock DbSet for Books to return a list when queried (simulating synchronous behavior)
            var mockBookSet = new Mock<DbSet<Book>>();
            _mockContextMock.Setup(c => c.Books).Returns(mockBookSet.Object);

            // Setup mock DbSet for Authors and Genres similarly, returning empty lists by default
            var mockAuthorSet = new Mock<DbSet<Author>>();
            _mockContextMock.Setup(c => c.Authors).Returns(mockAuthorSet.Object);
            
            var mockGenreSet = new Mock<DbSet<Genre>>();
            _mockContextMock.Setup(c => c.Genres).Returns(mockGenreSet.Object);

            // Setup the mock factory to return our mocked context when CreateDbContextAsync is called
            _mockContextFactory = new Mock<IDbContextFactory<BooksDbContext>>();
            _mockContextFactory.Setup(f => f.CreateDbContextAsync()).ReturnsAsync(_mockContextMock.Object);

            // Instantiate the manager under test
            _bookDbManager = new BookDbManager(_mockContextFactory.Object);
        }

        [Fact]
        public async Task AddBook_WhenAuthorAndGenreExist_AddsNewBookSuccessfully()
        {
            // Arrange: Setup existing Author and Genre entities
            var authorEntity = new Author { AuthorPk = Guid.NewGuid(), Name = "Existing Author" };
            var genreEntity = new Genre { GenrePk = Guid.NewGuid(), Name = "Fiction" };

            // Mock the FindAsync calls to return existing entities
            _mockContextMock.Setup(c => c.Authors.FindAsync(It.IsAny<Guid>())).ReturnsAsync(authorEntity);
            _mockContextMock.Setup(c => c.Genres.FindAsync(It.IsAny<Guid>())).ReturnsAsync(genreEntity);

            var bookToSave = new Book { Title = "New Test Book", Author = new Author { AuthorPk = authorEntity.AuthorPk, Genre = new Genre() }, Genre = new Genre { GenrePk = genreEntity.GenrePk } };

            // Act
            var resultBook = await _bookDbManager.AddBook(bookToSave);

            // Assert
            Assert.NotNull(resultBook);
            Assert.Equal("New Test Book", resultBook.Title);
            _mockContextMock.Verify(c => c.Books.AddAsync(It.IsAny<Book>()), Times.Once());
            _mockContextMock.Verify(c => c.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task AddBook_WhenAuthorIsMissingButGenreExists_CreatesAndAddsNewAuthor()
        {
            // Arrange: Setup non-existent Author and existing Genre
            var genreEntity = new Genre { GenrePk = Guid.NewGuid(), Name = "Fiction" };
            var bookToSave = new Book { Title = "Test Book", Author = new Author { Name = "New Author" }, Genre = new Genre { GenrePk = genreEntity.GenrePk } };

            // Mock the FindAsync calls: Author returns null, Genre returns existing entity
            _mockContextMock.Setup(c => c.Authors.FindAsync(It.IsAny<Guid>())).ReturnsAsync((Author)null);
            _mockContextMock.Setup(c => c.Genres.FindAsync(It.IsAny<Guid>())).ReturnsAsync(genreEntity);

            // Act
            var resultBook = await _bookDbManager.AddBook(bookToSave);

            // Assert
            Assert.NotNull(resultBook);
            _mockContextMock.Verify(c => c.Authors.AddAsync(It.Is<Author>(a => a.Name == "New Author")), Times.Once());
            _mockContextMock.Verify(c => c.Genres.AddAsync(It.IsAny<Genre>()), Times.Never()); // Genre already existed
            _mockContextMock.Verify(c => c.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task AddBook_WhenAuthorAndGenreExistButTitleIsEmpty_AddsWithEmptyTitle()
        {
            // Arrange: Setup existing Author and Genre entities
            var authorEntity = new Author { AuthorPk = Guid.NewGuid(), Name = "Existing Author" };
            var genreEntity = new Genre { GenrePk = Guid.NewGuid(), Name = "Fiction" };

            _mockContextMock.Setup(c => c.Authors.FindAsync(It.IsAny<Guid>())).ReturnsAsync(authorEntity);
            _mockContextMock.Setup(c => c.Genres.FindAsync(It.IsAny<Guid>())).ReturnsAsync(genreEntity);

            // Book with an empty title
            var bookToSave = new Book { Title = "", Author = new Author { AuthorPk = authorEntity.AuthorPk}, Genre = new Genre { GenrePk = genreEntity.GenrePk } };

            // Act
            var resultBook = await _bookDbManager.AddBook(bookToSave);

            // Assert
            Assert.NotNull(resultBook);
            Assert.Equal("", resultBook.Title); // Should save the empty title
            _mockContextMock.Verify(c => c.Books.AddAsync(It.IsAny<Book>()), Times.Once());
            _mockContextMock.Verify(c => c.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task AddBook_WhenDatabaseFails_ThrowsException()
        {
            // Arrange: Setup existing Author and Genre entities
            var authorEntity = new Author { AuthorPk = Guid.NewGuid(), Name = "Existing Author" };
            var genreEntity = new Genre { GenrePk = Guid.NewGuid(), Name = "Fiction" };

            _mockContextMock.Setup(c => c.Authors.FindAsync(It.IsAny<Guid>())).ReturnsAsync(authorEntity);
            _mockContextMock.Setup(c => c.Genres.FindAsync(It.IsAny<Guid>())).ReturnsAsync(genreEntity);

            var bookToSave = new Book { Title = "Failing Test Book", Author = new Author { AuthorPk = authorEntity.AuthorPk}, Genre = new Genre { GenrePk = genreEntity.GenrePk } };

            // Setup SaveChangesAsync to throw an exception
            _mockContextMock.Setup(c => c.SaveChangesAsync()).ThrowsAsync(new DbUpdateException("Database connection failed"));

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => _bookDbManager.AddBook(bookToSave));
        }

        [Fact]
        public async Task UpdateBook_WhenInputMatchesExisting_UpdatesSuccessfullyButNoChangesAreMade()
        {
            // Arrange
            var bookPk = Guid.NewGuid();
            var existingBook = new Book { BookPk = bookPk, Title = "Original Title", Rating = 5 };
            // The updated book is identical to the existing one
            var inputBook = new Book { BookPk = bookPk, Title = "Original Title", Rating = 5, Author = new Author(), Genre = new Genre() };

            // Mock the FindAsync call to return the existing entity
            _mockContextMock.Setup(c => c.Books.FindAsync(bookPk)).ReturnsAsync(existingBook);

            // Act
            await _bookDbManager.UpdateBook(inputBook);

            // Assert
            // Verify that SetValues was called, but since values are identical, SaveChanges should still be called once to persist state/track changes correctly in EF Core context lifecycle.
            _mockContextMock.Verify(c => c.Entry(It.Is<Book>(b => b.BookPk == bookPk)).CurrentValues.SetValues(inputBook), Times.Once());
            _mockContextMock.Verify(c => c.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task UpdateBook_WhenBookDoesNotExist_DoesNothing()
        {
            // Arrange
            var nonExistentBook = new Book { BookPk = Guid.NewGuid(), Title = "Ghost", Author = new Author(), Genre = new Genre() };

            // Mock the FindAsync call to return null
            _mockContextMock.Setup(c => c.Books.FindAsync(nonExistentBook.BookPk)).ReturnsAsync((Book)null);

            // Act
            await _bookDbManager.UpdateBook(nonExistentBook);

            // Assert
            // Verify that SaveChangesAsync was never called if the book wasn't found
            _mockContextMock.Verify(c => c.SaveChangesAsync(), Times.Never());
        }

        [Fact]
        public async Task UpdateBook_WhenDatabaseFailsDuringSetValues_ThrowsException()
        {
            // Arrange
            var bookPk = Guid.NewGuid();
            var existingBook = new Book { BookPk = bookPk, Title = "Old Title" };
            var updatedBook = new Book { BookPk = bookPk, Title = "New Updated Title", Author = new Author(), Genre = new Genre() };

            _mockContextMock.Setup(c => c.Books.FindAsync(bookPk)).ReturnsAsync(existingBook);

            // Setup SaveChangesAsync to throw an exception during the update process
            _mockContextMock.Setup(c => c.SaveChangesAsync()).ThrowsAsync(new DbUpdateException("Constraint violation during update"));

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => _bookDbManager.UpdateBook(updatedBook));
        }

        [Fact]
        public async Task GetAllBooksLocalized_WhenBooksExist_ReturnsCorrectList()
        {
            // Arrange
            byte languagePk = 1;
            var book1 = new Book { Title = "Book A", Author = new Author(), Genre = new Genre(), DateRead = DateTime.UtcNow, Rating = 5 };
            var book2 = new Book { Title = "Book B", Author = new Author(), Genre = new Genre(), DateRead = DateTime.UtcNow.AddDays(1), Rating = 4 };

            // Mock the query to return a list of books (using AsQueryable() on an in-memory list)
            _mockContextMock.Setup(c => c.Books).Returns((DbSet<Book>)new List<Book> { book1, book2 }.AsQueryable());

            // Act
            var result = await _bookDbManager.GetAllBooksLocalized(languagePk);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllBooksLocalized_WhenSingleBookExists_ReturnsCorrectList()
        {
            // Arrange
            byte languagePk = 1;
            var book1 = new Book { Title = "Single Book", Author = new Author(), Genre = new Genre(), DateRead = DateTime.UtcNow, Rating = 5 };

            // Mock the query to return a single book
            _mockContextMock.Setup(c => c.Books).Returns((DbSet<Book>)new List<Book> { book1 }.AsQueryable());

            // Act
            var result = await _bookDbManager.GetAllBooksLocalized(languagePk);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllBooksLocalized_WhenNoTranslationsMatchLanguagePk_ReturnsFallbackTitles()
        {
            // Arrange: Book exists, but its translations do not match the requested language (languagePk = 2)
            byte targetLanguagePk = 2;
            var bookWithOnlyLang1Translation = new Book { Title = "Default Title", Author = new Author(), Genre = new Genre(), DateRead = DateTime.UtcNow, Rating = 5 };

            // Mock the query to return a list of books where translations are missing for targetLanguagePk
            _mockContextMock.Setup(c => c.Books).Returns((DbSet<Book>)new List<Book> { bookWithOnlyLang1Translation }.AsQueryable());

            // Act
            var result = await _bookDbManager.GetAllBooksLocalized(targetLanguagePk);

            // Assert: The title should fall back to the default/base title stored on the Book entity
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Default Title", result[0].Title);
        }

        [Fact]
        public async Task GetAllBooksLocalized_WhenDatabaseFails_ThrowsException()
        {
            // Arrange
            byte languagePk = 1;

            // Setup the query to throw an exception (this will now be caught by synchronous execution path)
            _mockContextMock.Setup(c => c.Books).Throws(new DbUpdateException("Query failed"));

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => _bookDbManager.GetAllBooksLocalized(languagePk));
        }

        [Fact]
        public async Task FindBookByPkLocalized_WhenBookExists_ReturnsCorrectBook()
        {
            // Arrange
            Guid bookPk = Guid.NewGuid();
            var expectedBook = new Book { BookPk = bookPk, Title = "Found Book", Author = new Author(), Genre = new Genre() };

            // Mock the query to return a single book matching the PK
            _mockContextMock.Setup(c => c.Books.Where(b => b.BookPk == bookPk)).Returns(new List<Book> { expectedBook }.AsQueryable());

            // Act
            var result = await _bookDbManager.FindBookByPkLocalized(bookPk, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Found Book", result.Title);
        }

        [Fact]
        public async Task FindBookByPkLocalized_WhenBookDoesNotExist_ReturnsNull()
        {
            // Arrange
            Guid nonExistentPk = Guid.NewGuid();

            // Mock the query to return an empty list (FirstOrDefaultAsync will return null)
            _mockContextMock.Setup(c => c.Books.Where(b => b.BookPk == nonExistentPk)).Returns(Enumerable.Empty<Book>().AsQueryable());

            // Act
            var result = await _bookDbManager.FindBookByPkLocalized(nonExistentPk, 1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindBookByPkLocalized_WhenNoTranslationForLanguage_ReturnsFallbackTitle()
        {
            // Arrange: Book exists, but no translation for the requested language (languagePk = 2)
            Guid bookPk = Guid.NewGuid();
            var existingBook = new Book { BookPk = bookPk, Title = "Default Title", Author = new Author(), Genre = new Genre() };

            // Mock the query to return a single book where translation lookup will fail/return null
            _mockContextMock.Setup(c => c.Books.Where(b => b.BookPk == bookPk)).Returns(new List<Book> { existingBook }.AsQueryable());

            // Act
            var result = await _bookDbManager.FindBookByPkLocalized(bookPk, 2); // Requesting language 2

            // Assert: The title should fall back to the default/base title stored on the Book entity
            Assert.NotNull(result);
            Assert.Equal("Default Title", result.Title);
        }

        [Fact]
        public async Task FindBookByPkLocalized_WhenDatabaseFails_ThrowsException()
        {
            // Arrange
            Guid bookPk = Guid.NewGuid();

            // Setup the query to throw an exception
            _mockContextMock.Setup(c => c.Books.Where(b => b.BookPk == bookPk)).Throws(new DbUpdateException("Query failed"));

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => _bookDbManager.FindBookByPkLocalized(bookPk, 1));
        }

        [Fact]
        public async Task CountBooksByYears_WhenDataExistsForMultipleYears_ReturnsCorrectCounts()
        {
            // Arrange: Simulate data for years 2023 and 2024
            var book2023 = new Book { DateRead = DateTime.UtcNow.AddYears(-1) };
            var book2023b = new Book { DateRead = DateTime.UtcNow.AddYears(-1) };
            var book2024 = new Book { DateRead = DateTime.UtcNow };

            // Mock the query to return these books
            _mockContextMock.Setup(c => c.Books).Returns((DbSet<Book>)new List<Book> { book2023, book2023b, book2024 }.AsQueryable());

            // Act
            var result = await _bookDbManager.CountBooksByYears();

            // Assert: Check if the counts are correct for the years present in the mock data (assuming current year is 2025)
            Assert.Equal(2, result[2023]);
            Assert.Equal(1, result[2024]);
        }

        [Fact]
        public async Task CountBooksByYears_WhenNoDataExists_ReturnsAllZeroCounts()
        {
            // Arrange: Simulate no books found in the mock data
            _mockContextMock.Setup(c => c.Books).Returns((DbSet<Book>)Enumerable.Empty<Book>().AsQueryable());

            // Act
            var result = await _bookDbManager.CountBooksByYears();

            // Assert: Check if all expected years have a count of 0
            Assert.True(result.All(kvp => kvp.Value == 0));
        }

        [Fact]
        public async Task CountBooksByYears_WhenDataExistsOutsideTargetRange_ReturnsCorrectCounts()
        {
            // Arrange: Simulate books from a year outside the target range (e.g., 5 years ago, if current is 2025)
            var bookOld = new Book { DateRead = DateTime.UtcNow.AddYears(-5), Author = new Author(), Genre = new Genre() };

            // Mock the query to return this old book and one in the target range
            _mockContextMock.Setup(c => c.Books).Returns(new List<Book> { bookOld, new Book { DateRead = DateTime.UtcNow } }.AsQueryable());

            // Act
            var result = await _bookDbManager.CountBooksByYears();

            // Assert: The count for the target year should be 1, and the old book should be ignored.
            Assert.Equal(0, result[2023]); // Assuming current year is 2025
            Assert.Equal(1, result[2024]);
        }

        [Fact]
        public async Task AddBook_WhenAuthorIsNull_ThrowsNullReferenceException()
        {
            // Arrange
            var bookToSave = new Book { Title = "Test", Author = null, Genre = new Genre { GenrePk = Guid.NewGuid(), Name = "Fiction" } };

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _bookDbManager.AddBook(bookToSave));
        }

        [Fact]
        public async Task CountBooksByYears_WhenDatabaseFails_ThrowsException()
        {
            // Arrange: Simulate the query failing
            _mockContextMock.Setup(c => c.Books).Throws(new DbUpdateException("Counting failed"));

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => _bookDbManager.CountBooksByYears());
        }
    }
}