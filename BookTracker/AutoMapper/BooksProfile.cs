using AutoMapper;
using BookTracker.BLL.Models;
using BookTracker.DAL.Entities;

namespace BookTracker.AutoMapper
{
    public class BooksProfile : Profile
    {
        public BooksProfile()
        {
            CreateMap<Book, BookModel>();
        }
    }
}
