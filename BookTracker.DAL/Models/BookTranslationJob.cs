using BookTracker.DAL.Entities.Languages;

namespace BookTracker.DAL.Models
{
	public class BookTranslationJob
	{
		public Guid BookPk { get; init; }

		public Guid AuthorPk { get; init; }

		public Guid GenrePk { get; init; }

		public string Title { get; init; } = "";

		public string AuthorName { get; init; } = "";

		public string Genre { get; init; } = "";

		public Languages TargetLanguage { get; init; } = Languages.Ukrainian;
	}
}