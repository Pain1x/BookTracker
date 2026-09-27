using BookTracker.DAL.Models;

namespace BookTracker.DAL.Abstractions
{
	public interface IBookTranslationProcessor
	{
		/// <summary>
		/// Processes the translation of the book.
		/// </summary>
		/// <param name="job">The job with params.</param>
		/// <returns></returns>
		Task ProcessTranslationAsync(BookTranslationJob job);
	}
}