using BookTracker.DAL.Abstractions;
using BookTracker.DAL.Entities.Authors;

namespace BookTracker.DAL.Entities.Translations
{
	public class AuthorTranslation : TranslationEntity
	{
		public int AuthorTranslationPk { get; set; }
		public Guid AuthorPk { get; set; }
		public string Name { get; set; } = "";
		public Author Author { get; set; } = null!;
	}
}