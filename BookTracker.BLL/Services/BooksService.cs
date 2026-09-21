using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// Assuming BookTracker.BLL.Abstractions and BookTracker.BLL.Models are correctly set up in the project references.
using BookTracker.BLL.Abstractions; 
using BookTracker.BLL.Models;
using BookTracker.DAL.Abstractions;
using BookTracker.DAL.Entities.Books;

namespace BookTracker.BLL.Services
{
    /// <summary>
    /// Service class containing all business logic actions related to books.
    /// </summary>
    public class BooksService : IBooksService
    {
        // --- Private Fields (Updated) ---
        private readonly IBookDbManager _booksDbManager;
        private readonly IMapper _mapper;
        private readonly ITranslationService _translationService; // New dependency

        /// <summary>
        /// Initializes a new instance of the BooksService class.
        /// </summary>
        public BooksService(IBookDbManager booksDbManager, IMapper mapper, ITranslationService translationService) 
            : _booksDbManager(booksDbManager)
            , _mapper(mapper)
            , _translationService(translationService) // Assignment of new dependency
        {
        }

        #region Implementation of IBooksService (Updated AddBook method)

        /// <inheritdoc/>
        public async Task<BookModel> AddBook(BookModel book) 
        {
            // PHASE 2: Translation Integration Point
            if (book == null) throw new ArgumentNullException(nameof(book));
            
            // Determine source language. In a real system, this would come from the user input/context.
            byte sourceLang = Language.Unknown; // Placeholder for actual detection logic

            // Step 1: Get translations using the local LLM service
            var translations = await _translationService.TranslateMetadataAsync(sourceLang, new List<Language> { Language.English, Language.Ukrainian }, book);

            if (translations == null || !translations.Any())
            {
                 Console.WriteLine("Warning: Failed to retrieve translations. Saving book without localized metadata.");
            }
            else
            {
                // Step 2: Apply translated data back into the BookModel (enrichment)
                // NOTE TO USER: You must ensure your BookModel has properties matching these keys 
                // (e.g., TitleEn, AuthorUk) and that this logic correctly extracts values from the 'translations' dictionary structure.

                if (translations["title"] != null && translations["title"]["en"] != null)
                {
                    book.Title = translations["title"]["en"]; // Example: Using English for primary title
                }
                // Add similar enrichment logic here for other fields (Author, Genre, etc.) using the correct language keys.
            }

            // Step 3: Map and save the enriched model
            var bookToSave = _mapper.Map<BookModel, Book>(book);
            return await _booksDbManager.AddBook(bookToSave);
        }


		///<inheritdoc/>
		public Task UpdateBook(BookModel updatedBook) => _booksDbManager.UpdateBook(_mapper.Map<BookModel, Book>(updatedBook));

		///<inheritdoc/>
		public Task DeleteBook(Guid bookPk) => _booksDbManager.DeleteBook(bookPk);

		///<inheritdoc/>
		public async Task<List<BookModel>> GetAllBooks() => _mapper.Map<List<Book>, List<BookModel>>(await _booksDbManager.GetAllBooks());

		///<inheritdoc/>
		public async Task<List<BookModel>> GetAllBooksLocalized(byte languagePk)
			=> _mapper.Map<List<Book>, List<BookModel>>(await _booksDbManager.GetAllBooksLocalized(languagePk));

		///<inheritdoc/>
		public async Task<BookModel?> FindBookByPk(Guid bookPk) => _mapper.Map<Book?, BookModel?>(await _booksDbManager.FindBookByPk(bookPk));

		///<inheritdoc/>
		public async Task<BookModel?> FindBookByPkLocalized(Guid bookPk, byte languagePk)
			=> _mapper.Map<Book?, BookModel?>(await _booksDbManager.FindBookByPkLocalized(bookPk, languagePk));
        #endregion

        ///<inheritdoc/>
		public Task<int> CountBooksByAuthor(string authorName) => _booksDbManager.CountBooksByAuthor(authorName);

		///<inheritdoc/>
		public Task<int> CountBooksByGenre(string genreName) => _booksDbManager.CountBooksByGenre(genreName);

		///<inheritdoc/>
		public Task<Dictionary<int, int>> CountBooksByYears() => _booksDbManager.CountBooksByYears();

	}
}