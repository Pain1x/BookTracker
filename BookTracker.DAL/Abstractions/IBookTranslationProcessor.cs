using BookTracker.DAL.Models;

namespace BookTracker.DAL.Abstractions
{
	public interface IBookTranslationProcessor
	{
		Task ProcessUkrainianTranslationAsync(BookTranslationJob job);
	}
}