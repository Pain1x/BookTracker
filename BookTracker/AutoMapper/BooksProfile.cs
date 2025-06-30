using AutoMapper;
using BookTracker.Entities;
using BookTracker.Models;

namespace BookTracker.AutoMapper
{
    public class BooksProfile : Profile
    {
        public BooksProfile()
        {
            CreateMap<Book, BookModel>()
                .ForMember(dest => dest.AuthorName, opt => opt.Ignore()) // needs AuthorManager to resolve
                .ForMember(dest => dest.GenreName, opt => opt.Ignore());

            CreateMap<BookModel, Book>()
                .ForMember(dest => dest.AuthorPK, opt => opt.Ignore()) // needs AuthorManager to resolve
                .ForMember(dest => dest.GenrePK, opt => opt.Ignore());
        }
    }
}
