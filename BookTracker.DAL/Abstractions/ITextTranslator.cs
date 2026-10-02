using BookTracker.Common.Enums;

namespace BookTracker.DAL.Abstractions
{
	public interface ITextTranslator
	{
		/// <summary>
		/// Translates given text into from target language.
		/// </summary>
		/// <param name="sourceText">The text to translate.</param>
		/// <param name="targetLanguage">The language to translate from.</param>
		/// <returns></returns>
		Task<string> TranslateAsync(string sourceText, Languages targetLanguage);
	}
}