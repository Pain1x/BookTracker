using BookTracker.DAL.Entities.Genres;
using BookTracker.DAL.Entities.Languages;

namespace BookTracker.DAL.Entities.Translations
{
	public class GenreTranslation
	{
		public int GenreTranslationPk { get; set; }
		public Guid GenrePk { get; set; }
		public byte LanguagePk { get; set; }
		public string Name { get; set; } = "";

		public Genre Genre { get; set; } = null!;
		public Language Language { get; set; } = null!;
	}
}