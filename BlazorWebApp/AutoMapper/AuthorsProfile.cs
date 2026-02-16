using AutoMapper;

using BookTracker.BLL.Models;
using BookTracker.DAL.Entities.Authors;

namespace BlazorWebApp.AutoMapper
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<Author, AuthorModel>().ReverseMap();
        }
    }
}
