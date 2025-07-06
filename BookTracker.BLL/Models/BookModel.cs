using BookTracker.DAL.Entities;

namespace BookTracker.BLL.Models
{
    public class BookModel
    {
        public Guid BookPK { get; set; }

        public string Title { get; set; } = "";

        public DateTime? DateRead { get; set; }

        public int Rating { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>
        /// The author.
        /// </value>
        public Author Author { get; set; }

        /// <summary>
        /// Gets or sets the genre.
        /// </summary>
        /// <value>
        /// The genre.
        /// </value>
        public Genre Genre { get; set; }
    }
}
