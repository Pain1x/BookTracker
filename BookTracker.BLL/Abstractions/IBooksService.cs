using BookTracker.BLL.Models;

namespace BookTracker.BLL.Abstractions
{
    public interface IBooksService
    {
        /// <summary>
        /// Adds the book.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <returns></returns>
        public Task AddBook(BookModel book);

        /// <summary>
        /// Edits the book.
        /// </summary>
        /// <param name="updatedBook">The updated book.</param>
        /// <returns></returns>
        public Task UpdateBook(BookModel updatedBook);

        /// <summary>
        /// Deletes the book.
        /// </summary>
        /// <param name="bookPk">The book pk.</param>
        /// <returns></returns>
        public Task DeleteBook(Guid bookPk);

        /// <summary>
        /// Gets all books.
        /// </summary>
        /// <returns></returns>
        public Task<List<BookModel>> GetAllBooks();

        /// <summary>
        /// Finds the book by identifier.
        /// </summary>
        /// <param name="bookPk">The book pk.</param>
        /// <returns></returns>
        public Task<BookModel?> FindBookByPk(Guid bookPk);

        /// <summary>
        /// Counts the books by year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public Task<int> CountBooksByYear(int year);

        /// <summary>
        /// Counts the books by author.
        /// </summary>
        /// <param name="authorName">Name of the author.</param>
        /// <returns></returns>
        public Task<int> CountBooksByAuthor(string authorName);

        /// <summary>
        /// Counts the books by genre.
        /// </summary>
        /// <param name="genreName">Name of the genre.</param>
        /// <returns></returns>
        public Task<int> CountBooksByGenre(string genreName);
    }
}
