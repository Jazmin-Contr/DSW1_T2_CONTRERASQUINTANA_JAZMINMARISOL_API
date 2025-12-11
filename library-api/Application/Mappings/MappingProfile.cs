using library_api.Application.DTOs.Book;
using library_api.Application.DTOs.Loan;
using library_api.Domain.Entities;
using AutoMapper;

namespace library_api.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Book mappings
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.HasStock, opt => opt.MapFrom(src => src.HasStock()));

            CreateMap<CreateBookDto, Book>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<UpdateBookDto, Book>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // Loan mappings
            CreateMap<Loan, LoanDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : string.Empty))
                .ForMember(dest => dest.BookISBN, opt => opt.MapFrom(src => src.Book != null ? src.Book.ISBN : string.Empty));

            CreateMap<CreateLoanDto, Loan>()
                .ForMember(dest => dest.LoanDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now));
        }
    }
}
