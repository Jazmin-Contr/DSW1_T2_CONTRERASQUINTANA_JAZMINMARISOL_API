using library_api.Application.DTOs.Book;

namespace library_api.Application.Interfaces
{
    public interface IBookService
    {
        Task<BookDto?> GetByIdAsync(int id);
        Task<IEnumerable<BookDto>> GetAllAsync();
        Task<BookDto> CreateAsync(CreateBookDto dto);
        Task<BookDto> UpdateAsync(int id, UpdateBookDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<BookDto>> GetBooksWithStockAsync();
        Task<IEnumerable<BookDto>> SearchAsync(string searchTerm);
    }
}