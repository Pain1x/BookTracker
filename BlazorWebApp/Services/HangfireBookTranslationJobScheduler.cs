using BookTracker.DAL.Abstractions;
using BookTracker.DAL.Models;

using Hangfire;

namespace BlazorWebApp.Services
{
	public class HangfireBookTranslationJobScheduler(IBackgroundJobClient backgroundJobClient) : IBookTranslationJobScheduler
	{
		public void Enqueue(BookTranslationJob job)
		{
			backgroundJobClient.Enqueue<IBookTranslationProcessor>(
				processor => processor.ProcessUkrainianTranslationAsync(job));
		}
	}
}
