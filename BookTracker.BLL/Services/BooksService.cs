using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookTracker.BLL.Abstractions;
using BookTracker.BLL.Models;
using BookTracker.DAL.Abstractions;
using BookTracker.DAL.Entities.Books;
using System.Linq;
using BookTracker.Common.Enums;
using BookTracker.Jobs.Abstractions;
using BookTracker.Jobs.Models;

namespace BookTracker.BLL.Services
{
    public class BooksService(IBookDbManager booksDbManager, IMapper mapper, IBookTranslationJobScheduler scheduler)
        : IBooksService
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
            var bookToTranslate = await _booksDbManager.AddBook(_mapper.Map<BookModel, Book>(book));

            _scheduler.Enqueue(new BookTranslationJob
            {
                AuthorPk = bookToTranslate.Author.AuthorPk,
                AuthorName = bookToTranslate .Author.Name,
                BookPk = bookToTranslate .BookPk,
                Title = bookToTranslate .Title,
                GenrePk = bookToTranslate .Genre.GenrePk,
                Genre = bookToTranslate .Genre.Name,
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
        public async Task<List<BookCountSummaryModel>> CountBooksByYears()
        {
            // Await the raw dictionary result (e.g., Dictionary<int, int>) from the DB manager
            var yearCounts = await _booksDbManager.CountBooksByYears();

            // Transform the key-value pairs into a List of BookCountSummaryModel objects
            var summaryList = yearCounts
                .Select(kvp => new BookCountSummaryModel
                {
                    Year = kvp.Key,
                    Count = kvp.Value
                })
                .ToList();

            // Return the transformed list wrapped in a Task
            return summaryList;
        }

        #endregion
    }
}