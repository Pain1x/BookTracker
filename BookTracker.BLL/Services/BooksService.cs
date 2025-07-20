using AutoMapper;
using BookTracker.BLL.Abstarctions;
using BookTracker.BLL.Models;
using BookTracker.DAL.Entities;

namespace BookTracker.BLL.Services
{
    public class BooksService(BookDBManager booksDBManager, IMapper mapper) : IBooksService
    {
        #region Private Fields

        /// <summary>
        /// The manager
        /// </summary>
        private BookDBManager BooksDBManager = booksDBManager;

        /// <summary>
        /// The manager
        /// </summary>
        private IMapper Mapper = mapper;

        #endregion

        #region Implementation of IBooksService

        ///<inheritdoc/>
        public Task AddBook(BookModel book) => BooksDBManager.AddBook(Mapper.Map<BookModel, Book>(book));

        ///<inheritdoc/>
        public Task EditBook(BookModel updatedBook) => BooksDBManager.EditBook(Mapper.Map<BookModel, Book>(updatedBook));

        ///<inheritdoc/>
        public Task DeleteBook(Guid bookPK) => BooksDBManager.DeleteBook(bookPK);

        ///<inheritdoc/>
        public async Task<List<BookModel>> GetAllBooks() => Mapper.Map<List<Book>, List<BookModel>>(await BooksDBManager.GetAllBooks());

        ///<inheritdoc/>
        public async Task<BookModel?> FindBookById(Guid bookPK) => Mapper.Map<Book?, BookModel?>(await BooksDBManager.FindBookById(bookPK));

        ///<inheritdoc/>
        public Task<int> CountBooksByYear(int year) => BooksDBManager.CountBooksByYear(year);

        ///<inheritdoc/>
        public Task<int> CountBooksByAuthor(string authorName) => BooksDBManager.CountBooksByAuthor(authorName);

        ///<inheritdoc/>
        public Task<int> CountBooksByGenre(string genreName) => BooksDBManager.CountBooksByGenre(genreName);

        #endregion
    }
}