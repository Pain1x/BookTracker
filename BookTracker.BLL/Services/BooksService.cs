using AutoMapper;
using BookTracker.BLL.Models;
using BookTracker.DAL.Entities;

namespace BookTracker.BLL.Services
{
    public class BooksService
    {
        /// <summary>
        /// The manager
        /// </summary>
        private BookDBManager BooksDBManager;

        /// <summary>
        /// The manager
        /// </summary>
        private IMapper Mapper;

        public BooksService(BookDBManager booksDBManager, IMapper mapper)
        {
            BooksDBManager = booksDBManager;
            Mapper = mapper;
        }

        public List<BookModel> GetAllBooks() => Mapper.Map<List<Book>, List<BookModel>>(BooksDBManager.GetAllBooks());

        //public void AddBook(BookModel book)
        //{
        //    BooksDBManager.AddBook(book);
        //}
    }
}
