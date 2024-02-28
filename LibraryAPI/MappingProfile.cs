using AutoMapper;
using LibraryAPI.Database.Models;
using LibraryAPI.DTOs;

namespace LibraryAPI {
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BookDTO, Book>();
            CreateMap<Book, BookDTO>();
            CreateMap<PageDTO, Page>();
            CreateMap<Page, PageDTO>();
            CreateMap<Book, BookInfoDTO>();
            CreateMap<Rental, RentalDTO>();
        }
    }
}
