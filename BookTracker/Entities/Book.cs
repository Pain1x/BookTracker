namespace BookTracker.Entities
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
        public Guid BookPK { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; } = "";

        /// <summary>
        /// Gets or sets the author pk.
        /// </summary>
        /// <value>
        /// The author pk.
        /// </value>
        public Guid AuthorPK { get; set; }

        /// <summary>
        /// Gets or sets the genre pk.
        /// </summary>
        /// <value>
        /// The genre pk.
        /// </value>
        public Guid GenrePK { get; set; }

        /// <summary>
        /// Gets or sets the date read.
        /// </summary>
        /// <value>
        /// The date read.
        /// </value>
        public DateTime? DateRead { get; set; }

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
    }
}