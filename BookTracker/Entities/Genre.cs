namespace BookTracker.Entities
{
    /// <summary>
    /// Genre Entity
    /// </summary>
    public class Genre
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid GenrePK { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = "";
    }
}
