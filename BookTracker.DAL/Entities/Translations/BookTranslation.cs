using BookTracker.DAL.Entities.Books;
using BookTracker.DAL.Entities.Languages;

namespace BookTracker.DAL.Entities.Translations
{
	public class BookTranslation
	{
		public int BookTranslationPk { get; set; }
		public Guid BookPk { get; set; }
		public byte LanguagePk { get; set; }
		public string Title { get; set; } = "";

		public Book Book { get; set; } = null!;
		public Language Language { get; set; } = null!;
	}
}