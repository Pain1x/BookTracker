using BookTracker.DAL.Entities.Languages;

namespace BookTracker.DAL.Abstractions
{
	public interface ITextTranslator
	{
		Task<string> TranslateAsync(string sourceText, Languages targetLanguage);
	}
}