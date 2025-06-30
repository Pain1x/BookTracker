using BookTracker.Entities;
using BookTracker.Repositories;

namespace BookTracker.Managers
{
    public class AuthorManager
    {
        /// <summary>
        /// The authors
        /// </summary>
        private List<Author> authors;

        public AuthorManager()
        {
            authors = AuthorRepository.Load();
        }

        /// <summary>
        /// Adds the author.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Author AddAuthor(string name)
        {
            // Check if author already exists (case-insensitive)
            var existing = authors.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
                return existing;

            var author = new Author { Name = name };
            authors.Add(author);
            AuthorRepository.Save(authors);
            return author;
        }

        /// <summary>
        /// Finds the author by identifier.
        /// </summary>
        /// <param name="authorPK">The author pk.</param>
        /// <returns></returns>
        public Author? FindAuthorById(Guid authorPK)
        {
            return authors.FirstOrDefault(a => a.AuthorPK == authorPK);
        }

        /// <summary>
        /// Finds the name of the author by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Author? FindAuthorByName(string name)
        {
            return authors.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Lists the authors.
        /// </summary>
        public void ListAuthors()
        {
            Console.WriteLine("Authors:");
            foreach (var author in authors.OrderBy(x => x.Name))
            {
                Console.WriteLine($"{author.AuthorPK} - {author.Name}");
            }
        }

        /// <summary>
        /// Deletes the name of the author by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public void DeleteAuthorByName(string name)
        {
            var author = authors.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (author != null)
            {
                authors.Remove(author);
                AuthorRepository.Save(authors);

                Console.WriteLine($"The {author.Name} is deleted");
            }
            Console.WriteLine("This author is not present in database");
        }
    }
}
