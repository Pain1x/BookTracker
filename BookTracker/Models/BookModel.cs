namespace BookTracker.Models
{
    public class BookModel
    {
        public Guid BookPK { get; set; }

        public string Title { get; set; } = "";

        public string AuthorName { get; set; } = "";

        public string GenreName { get; set; } = "";

        public DateTime? DateRead { get; set; }

        public int Rating { get; set; }
    }
}
