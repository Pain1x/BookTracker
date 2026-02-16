using AutoMapper;

using BookTracker.BLL.Models;
using BookTracker.DAL.Entities.Genres;

namespace BlazorWebApp.AutoMapper
{
    public class GenresProfile : Profile
    {
        public GenresProfile()
        {
            CreateMap<Genre, GenreModel>().ReverseMap();
        }
    }
}
