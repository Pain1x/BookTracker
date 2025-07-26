using AutoMapper;
using BookTracker.BLL.Models;
using BookTracker.DAL.Entities;

namespace BlazorWebApp.AutoMapper
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<Author, AuthorModel>();
        }
    }
}
