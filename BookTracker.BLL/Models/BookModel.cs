using BookTracker.DAL.Entities;

namespace BookTracker.BLL.Models
{
    public class BookModel
    {
        public string? DateReadString => DateRead?.UtcDateTime.ToString("yyyy-MM-dd");

        public Guid BookPK { get; set; }

        public string Title { get; set; } = "";

        public DateTimeOffset? DateRead { get; set; }

        public int Rating { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>
        /// The author.
        /// </value>
        public required AuthorModel Author { get; set; }

        /// <summary>
        /// Gets or sets the genre.
        /// </summary>
        /// <value>
        /// The genre.
        /// </value>
        public required GenreModel Genre { get; set; }
    }
}
