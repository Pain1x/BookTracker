namespace BookTracker.BLL.Models
{
    /// <summary>
    /// Summary model for book counts grouped by year.
    /// </summary>
    public class BookCountSummaryModel
    {
        // Key represents the year (e.g., 2023)
        public int Year { get; set; }

        // Value represents the count of books in that year
        public int Count { get; set; }
    }
}