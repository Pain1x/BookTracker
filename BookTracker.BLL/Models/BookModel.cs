namespace BookTracker.BLL.Models
{
	public class BookModel
	{
		public string? DateReadString => DateRead?.UtcDateTime.ToString("yyyy-MM-dd");

		public Guid BookPk { get; set; }

		public string Title { get; set; } = "";

		public DateTimeOffset? DateRead { get; set; }

		public int Rating { get; set; }

		public string Notes { get; set; } = "";

		/// <summary>
		/// Gets or sets the author.
		/// </summary>
		/// <value>
		/// The author.
		/// </value>
		public string? TitleEn { get; set; }
		public string? TitleUk { get; set; }

		// Author localization
		public string? AuthorEn { get; set; }
		public string? AuthorUk { get; set; }

		// Genre localization
		public string? GenreEn { get; set; }
		public string? GenreUk { get; set; }
	}
}
