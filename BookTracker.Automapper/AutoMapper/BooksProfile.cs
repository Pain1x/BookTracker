using AutoMapper;
using BookTracker.BLL.Models;
using BookTracker.DAL.Entities.Books;

namespace BookTracker.Automapper.AutoMapper
{
    public class BooksProfile : Profile
    {
        public BooksProfile()
        {
            // Explicit mapping from DAL Entity (Book) to BLL Model (BookModel).
            CreateMap<Book, BookModel>().ReverseMap();
        }
    }
}