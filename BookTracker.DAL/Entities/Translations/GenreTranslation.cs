using BookTracker.DAL.Abstractions;
using BookTracker.DAL.Entities.Genres;

namespace BookTracker.DAL.Entities.Translations
{
	public class GenreTranslation : TranslationEntity
	{
		public int GenreTranslationPk { get; set; }
		public Guid GenrePk { get; set; }
		public string Name { get; set; } = "";
		public Genre Genre { get; set; } = null!;
	}
}