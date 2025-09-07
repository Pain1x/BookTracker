using AutoMapper;
using BookTracker.BLL.Abstractions;
using BookTracker.BLL.Models;
using BookTracker.DAL.Abstractions;
using BookTracker.DAL.Entities;

namespace BookTracker.BLL.Services
{
    public class BooksService(IBookDbManager booksDbManager, IMapper mapper) : IBooksService
    {
        #region Private Fields

        /// <summary>
        /// The manager
        /// </summary>
        private readonly IBookDbManager _booksDbManager = booksDbManager;

        /// <summary>
        /// The manager
        /// </summary>
        private readonly IMapper _mapper = mapper;

        #endregion

        #region Implementation of IBooksService

        ///<inheritdoc/>
        public Task AddBook(BookModel book) => _booksDbManager.AddBook(_mapper.Map<BookModel, Book>(book));

        ///<inheritdoc/>
        public Task UpdateBook(BookModel updatedBook) => _booksDbManager.UpdateBook(_mapper.Map<BookModel, Book>(updatedBook));

        ///<inheritdoc/>
        public Task DeleteBook(Guid bookPk) => _booksDbManager.DeleteBook(bookPk);

        ///<inheritdoc/>
        public async Task<List<BookModel>> GetAllBooks() => _mapper.Map<List<Book>, List<BookModel>>(await _booksDbManager.GetAllBooks());

        ///<inheritdoc/>
        public async Task<BookModel?> FindBookByPk(Guid bookPk) => _mapper.Map<Book?, BookModel?>(await _booksDbManager.FindBookByPk(bookPk));

        ///<inheritdoc/>
        public Task<int> CountBooksByYear(int year) => _booksDbManager.CountBooksByYear(year);

        ///<inheritdoc/>
        public Task<int> CountBooksByAuthor(string authorName) => _booksDbManager.CountBooksByAuthor(authorName);

        ///<inheritdoc/>
        public Task<int> CountBooksByGenre(string genreName) => _booksDbManager.CountBooksByGenre(genreName);

        #endregion
    }
}