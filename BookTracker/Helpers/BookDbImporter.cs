namespace BookTracker.Helpers
{
    internal class BookDbImporter
    {
        /// <summary>
        /// The connection string
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookDbImporter"/> class.
        /// </summary>
        /// <param name="connStr">The connection string.</param>
        public BookDbImporter(string connStr)
        {
            connectionString = connStr;
        }

        ///// <summary>
        ///// Inserts the books.
        ///// </summary>
        ///// <param name="books">The books.</param>
        //public void InsertBooks(List<Book> books)
        //{
        //    using var connection = new SqlConnection(connectionString);
        //    connection.Open();

        //    foreach (var book in books)
        //    {
        //        var command = new SqlCommand(@"
        //        INSERT INTO Books (BookPK, Title, Author, Genre, DateRead, Rating, Notes)
        //        VALUES (@BookPK, @Title, @Author, @Genre, @DateRead, @Rating, @Notes)", connection);

        //        command.Parameters.AddWithValue("@BookPK", Guid.NewGuid());
        //        command.Parameters.AddWithValue("@Title", book.Title);
        //        command.Parameters.AddWithValue("@Author", book.Author);
        //        command.Parameters.AddWithValue("@Genre", book.Genre);
        //        command.Parameters.AddWithValue("@DateRead", (object?)book.DateRead ?? DBNull.Value);
        //        command.Parameters.AddWithValue("@Rating", book.Rating);
        //        command.Parameters.AddWithValue("@Notes", book.Notes);

        //        command.ExecuteNonQuery();
        //    }
        //}
    }
}
