namespace BookTracker.DAL.Entities
{
    /// <summary>
    /// Book Entity
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid BookPk { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; } = "";

        /// <summary>
        /// Gets or sets the date read.
        /// </summary>
        /// <value>
        /// The date read.
        /// </value>
        public DateTimeOffset? DateRead { get; set; }

        /// <summary>
        /// Gets or sets the rating.
        /// </summary>
        /// <value>
        /// The rating.
        /// </value>
        public int Rating { get; set; }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        /// <value>
        /// The notes.
        /// </value>
        public string Notes { get; set; } = "";

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>
        /// The author.
        /// </value>
        public required Author Author { get; set; }

        /// <summary>
        /// Gets or sets the author pk.
        /// </summary>
        /// <value>
        /// The author pk.
        /// </value>
        public Guid AuthorPk { get; set; }

        /// <summary>
        /// Gets or sets the genre.
        /// </summary>
        /// <value>
        /// The genre.
        /// </value>
        public required Genre Genre { get; set; }

        /// <summary>
        /// Gets or sets the genre pk.
        /// </summary>
        /// <value>
        /// The genre pk.
        /// </value>
        public Guid GenrePk { get; set; }
    }
}