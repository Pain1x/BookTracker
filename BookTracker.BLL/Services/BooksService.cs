using AutoMapper;

using BookTracker.BLL.Abstractions;
using BookTracker.BLL.Models;
using BookTracker.DAL.Abstractions;
using BookTracker.DAL.Entities.Books;
using BookTracker.DAL.Entities.Enums;
using BookTracker.DAL.Models;

namespace BookTracker.BLL.Services
{
	public class BooksService(IBookDbManager booksDbManager, IMapper mapper, IBookTranslationJobScheduler scheduler) : IBooksService
	{
		#region Private Fields

		/// <summary>
		/// The manager
		/// </summary>
		private readonly IBookDbManager _booksDbManager = booksDbManager;

		/// <summary>
		/// The mapper
		/// </summary>
		private readonly IMapper _mapper = mapper;
		
		/// <summary>
		/// The manager
		/// </summary>
		private readonly IBookTranslationJobScheduler _scheduler = scheduler;

		#endregion

		#region Implementation of IBooksService

		///<inheritdoc/>
		public async Task AddBook(BookModel book, Languages targetLanguage)
		{
			await _booksDbManager.AddBook(_mapper.Map<BookModel, Book>(book));
			
			_scheduler.Enqueue(new BookTranslationJob
			{
				AuthorPk = book.Author.AuthorPk,
				AuthorName =  book.Author.Name,
				BookPk = book.BookPk,
				Title =  book.Title,
				GenrePk = book.Genre.GenrePk,
				Genre =  book.Genre.Name,
				TargetLanguage = targetLanguage
			});
		}
		
		///<inheritdoc/>
		public Task UpdateBook(BookModel updatedBook) =>
			_booksDbManager.UpdateBook(_mapper.Map<BookModel, Book>(updatedBook));

		///<inheritdoc/>
		public async Task<List<BookModel>> GetAllBooksLocalized(byte languagePk) =>
			_mapper.Map<List<Book>, List<BookModel>>(await _booksDbManager.GetAllBooksLocalized(languagePk));
		
		///<inheritdoc/>
		public async Task<BookModel> FindBookByPkLocalized(Guid bookPk, byte languagePk) =>
			_mapper.Map<Book, BookModel>(await _booksDbManager.FindBookByPkLocalized(bookPk, languagePk));
		
		///<inheritdoc/>
        public Task<Dictionary<int, int>> CountBooksByYears() =>
	        _booksDbManager.CountBooksByYears();

		#endregion
	}
}