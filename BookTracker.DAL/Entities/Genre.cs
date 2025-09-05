namespace BookTracker.DAL.Entities
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
        public Guid GenrePk { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = "";
    }
}
