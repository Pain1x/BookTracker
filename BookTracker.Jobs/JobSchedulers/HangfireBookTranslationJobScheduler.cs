using BookTracker.Jobs.Abstractions;
using BookTracker.Jobs.Models;
using Hangfire;

namespace BookTracker.Jobs.JobSchedulers
{
	public class HangfireBookTranslationJobScheduler(IBackgroundJobClient backgroundJobClient) : IBookTranslationJobScheduler
	{
		///<inhericdoc/>
		public void Enqueue(BookTranslationJob job)
		{
			backgroundJobClient.Enqueue<IBookTranslationProcessor>(
				processor => processor.ProcessTranslationAsync(job));
		}
	}
}
