using BookTracker.DAL.Entities.Books;


namespace BookTracker.DAL.Abstractions
{
	public interface IBookDbManager
	{
		 /// <summary>
        /// Adds the book.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <returns></returns>
        public Task<Book> AddBook(Book book);

        /// <summary>
        /// Edits the book.
        /// </summary>
        /// <param name="updatedBook">The updated book.</param>
        /// <returns></returns>
        public Task UpdateBook(Book updatedBook);

        /// <summary>
        /// Gets all books localized by language.
        /// </summary>
        /// <param name="languagePk">Language identifier.</param>
        /// <returns></returns>
        public Task<List<Book>> GetAllBooksLocalized(byte languagePk);

        /// <summary>
        /// Finds the book by identifier with localized data.
        /// </summary>
        /// <param name="bookPk">The book pk.</param>
        /// <param name="languagePk">Language identifier.</param>
        /// <returns></returns>
        public Task<Book?> FindBookByPkLocalized(Guid bookPk, byte languagePk);
		
        /// <summary>
        /// Returns books read grouped by years
        /// </summary>
        /// <returns>Quantity of books read by years</returns>
        public Task<Dictionary<int, int>> CountBooksByYears();

	}
}
