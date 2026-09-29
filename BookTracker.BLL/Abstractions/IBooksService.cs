using BookTracker.BLL.Models;
using BookTracker.DAL.Entities.Enums;

namespace BookTracker.BLL.Abstractions
{
	public interface IBooksService
	{
		/// <summary>
		/// Adds the book.
		/// </summary>
		/// <param name="book">The book.</param>
		/// <param name="targetLanguage">The target language.</param>
		/// <returns></returns>
		public Task AddBook(BookModel book, Languages targetLanguage);

		/// <summary>
		/// Edits the book.
		/// </summary>
		/// <param name="updatedBook">The updated book.</param>
		/// <returns></returns>
		public Task UpdateBook(BookModel updatedBook);

		/// <summary>
		/// Gets all books localized by language.
		/// </summary>
		/// <param name="languagePk">Language identifier.</param>
		/// <returns></returns>
		public Task<List<BookModel>> GetAllBooksLocalized(byte languagePk);

		/// <summary>
		/// Finds the book by identifier with localized data.
		/// </summary>
		/// <param name="bookPk">The book pk.</param>
		/// <param name="languagePk">Language identifier.</param>
		/// <returns></returns>
		public Task<BookModel> FindBookByPkLocalized(Guid bookPk, byte languagePk);

		/// <summary>
		/// Gets the count of books read for each year in the provided list of years.
		/// </summary>
		/// <returns></returns>
		Task<Dictionary<int, int>> CountBooksByYears();
	}
}
