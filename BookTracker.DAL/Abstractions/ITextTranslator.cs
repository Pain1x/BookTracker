namespace BookTracker.DAL.Abstractions
{
	public interface ITextTranslator
	{
		Task<string> TranslateToUkrainianAsync(string sourceText, CancellationToken cancellationToken = default);
	}
}