using BookTracker.DAL.Models;

namespace BookTracker.DAL.Abstractions
{
	public interface IBookTranslationJobScheduler
	{
		void Enqueue(BookTranslationJob job);
	}
}