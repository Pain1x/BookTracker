using BookTracker.DAL.Abstractions;
using BookTracker.DAL.Entities.Books;

namespace BookTracker.DAL.Entities.Translations
{
	public class BookTranslation: TranslationEntity
	{
		public int BookTranslationPk { get; set; }
		public Guid BookPk { get; set; }
		public string Title { get; set; } = "";
		public Book Book { get; set; } = null!;
	}
}