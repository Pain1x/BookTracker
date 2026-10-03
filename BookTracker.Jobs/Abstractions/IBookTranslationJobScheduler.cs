using BookTracker.Jobs.Models;

namespace BookTracker.Jobs.Abstractions
{
	public interface IBookTranslationJobScheduler
	{
		/// <summary>
		/// Enqueues the job.
		/// </summary>
		/// <param name="job">The job to enqueue</param>
		void Enqueue(BookTranslationJob job);
	}
}