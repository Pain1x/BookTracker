using BookTracker.DAL.Entities.Authors;
using BookTracker.DAL.Entities.Languages;

namespace BookTracker.DAL.Entities.Translations
{
	public class AuthorTranslation
	{
		public int AuthorTranslationPk { get; set; }
		public Guid AuthorPk { get; set; }
		public byte LanguagePk { get; set; }
		public string Name { get; set; } = "";

		public Author Author { get; set; } = null!;
		public Language Language { get; set; } = null!;
	}
}