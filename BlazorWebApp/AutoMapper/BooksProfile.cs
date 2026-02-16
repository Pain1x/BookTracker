using AutoMapper;
using BookTracker.BLL.Models;
using BookTracker.DAL.Entities.Books;

namespace BlazorWebApp.AutoMapper
{
    public class BooksProfile : Profile
    {
        public BooksProfile()
        {
            CreateMap<Book, BookModel>().ReverseMap();
        }
    }
}
