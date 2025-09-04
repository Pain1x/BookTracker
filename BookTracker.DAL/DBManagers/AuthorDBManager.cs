using BookTracker.DAL.DBContexts;
using BookTracker.DAL.DBManagers;
using BookTracker.DAL.Entities;

namespace BookTracker.Managers
{
    public class AuthorDbManager(BooksDbContext booksDbContext) : BaseDbManager(booksDbContext)
    {
        /// <summary>
        /// The authors
        /// </summary>
        private List<Author> _authors;

        ///// <summary>
        ///// Adds the author.
        ///// </summary>
        ///// <param name="name">The name.</param>
        ///// <returns></returns>
        //public Author AddAuthor(string name)
        //{
        //    // Check if author already exists (case-insensitive)
        //    var existing = authors.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        //    if (existing != null)
        //        return existing;

        //    var author = new Author { Name = name };
        //    authors.Add(author);
        //    AuthorsRepostitory.Save(authors);
        //    return author;
        //}

        /// <summary>
        /// Finds the author by identifier.
        /// </summary>
        /// <param name="authorPk">The author pk.</param>
        /// <returns></returns>
        public Author? FindAuthorById(Guid authorPk)
        {
            return _authors.FirstOrDefault(a => a.AuthorPk == authorPk);
        }

        /// <summary>
        /// Finds the name of the author by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Author? FindAuthorByName(string name)
        {
            return _authors.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Lists the authors.
        /// </summary>
        public void ListAuthors()
        {
            Console.WriteLine("Authors:");
            foreach (var author in _authors.OrderBy(x => x.Name))
            {
                Console.WriteLine($"{author.AuthorPk} - {author.Name}");
            }
        }

        ///// <summary>
        ///// Deletes the name of the author by.
        ///// </summary>
        ///// <param name="name">The name.</param>
        ///// <returns></returns>
        //public void DeleteAuthorByName(string name)
        //{
        //    var author = authors.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        //    if (author != null)
        //    {
        //        authors.Remove(author);
        //        AuthorsRepostitory.Save(authors);

        //        Console.WriteLine($"The {author.Name} is deleted");
        //    }
        //    Console.WriteLine("This author is not present in database");
        //}
    }
}
