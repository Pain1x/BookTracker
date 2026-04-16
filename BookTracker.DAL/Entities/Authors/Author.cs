using BookTracker.DAL.Entities.Translations;

namespace BookTracker.DAL.Entities.Authors
{
	/// <summary>
	/// Author Entity
	/// </summary>
	public class Author
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		public Guid AuthorPk { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; set; } = "";

		public ICollection<AuthorTranslation> Translations { get; set; } = new List<AuthorTranslation>();
	}
}
