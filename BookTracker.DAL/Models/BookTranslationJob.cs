namespace BookTracker.DAL.Models
{
	public class BookTranslationJob
	{
		public Guid BookPk { get; init; }

		public Guid AuthorPk { get; init; }

		public Guid GenrePk { get; init; }

		public string Title { get; init; } = "";

		public string AuthorName { get; init; } = "";

		public string GenreName { get; init; } = "";
	}
}