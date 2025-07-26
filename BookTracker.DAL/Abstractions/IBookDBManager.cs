using BookTracker.DAL.Entities;


namespace BookTracker.DAL.Abstractions
{
    public interface IBookDBManager
    {
        /// <summary>
        /// Adds the book.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <returns></returns>
        public Task AddBook(Book book);

        /// <summary>
        /// Edits the book.
        /// </summary>
        /// <param name="updatedBook">The updated book.</param>
        /// <returns></returns>
        public Task EditBook(Book updatedBook);

        /// <summary>
        /// Deletes the book.
        /// </summary>
        /// <param name="bookPK">The book pk.</param>
        /// <returns></returns>
        public Task DeleteBook(Guid bookPK);

        /// <summary>
        /// Gets all books.
        /// </summary>
        /// <returns></returns>
        public Task<List<Book>> GetAllBooks();

        /// <summary>
        /// Finds the book by identifier.
        /// </summary>
        /// <param name="bookPK">The book pk.</param>
        /// <returns></returns>
        public Task<Book?> FindBookById(Guid bookPK);

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
