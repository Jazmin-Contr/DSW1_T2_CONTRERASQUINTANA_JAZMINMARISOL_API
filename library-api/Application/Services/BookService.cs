using AutoMapper;
using library_api.Application.DTOs.Book;
using library_api.Application.Interfaces;
using library_api.Domain.Entities;
using library_api.Domain.Exceptions;
using library_api.Domain.Ports.Out;

namespace library_api.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BookDto?> GetByIdAsync(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            return book == null ? null : _mapper.Map<BookDto>(book);
        }

        public async Task<IEnumerable<BookDto>> GetAllAsync()
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<BookDto> CreateAsync(CreateBookDto dto)
        {
            // Validar que el ISBN sea único
            var existingBook = await _unitOfWork.Books.ISBNExistsAsync(dto.ISBN);
            if (existingBook)
            {
                throw new DuplicateEntityException("Book", "ISBN", dto.ISBN);
            }

            var book = _mapper.Map<Book>(dto);
            var createdBook = await _unitOfWork.Books.CreateAsync(book);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BookDto>(createdBook);
        }

        public async Task<BookDto> UpdateAsync(int id, UpdateBookDto dto)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null)
            {
                throw new NotFoundException("Book", id);
            }

            // Validar que el ISBN sea único (si cambió)
            if (book.ISBN != dto.ISBN)
            {
                var existingBook = await _unitOfWork.Books.ISBNExistsAsync(dto.ISBN);
                if (existingBook)
                {
                    throw new DuplicateEntityException("Book", "ISBN", dto.ISBN);
                }
            }

            _mapper.Map(dto, book);
            var updatedBook = await _unitOfWork.Books.UpdateAsync(book);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BookDto>(updatedBook);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null)
            {
                throw new NotFoundException("Book", id);
            }

            // Verificar si tiene préstamos activos
            var activeLoans = await _unitOfWork.Loans.GetLoansByBookIdAsync(id);
            var hasActiveLoans = activeLoans.Any(l => l.Status == "Active");

            if (hasActiveLoans)
            {
                throw new BusinessRuleException(
                    "BookHasActiveLoans",
                    $"No se puede eliminar el libro con ID {id} porque tiene préstamos activos.");
            }

            var result = await _unitOfWork.Books.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        public async Task<IEnumerable<BookDto>> GetBooksWithStockAsync()
        {
            var books = await _unitOfWork.Books.GetBooksWithStockAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<IEnumerable<BookDto>> SearchAsync(string searchTerm)
        {
            var books = await _unitOfWork.Books.SearchByTitleOrAuthorAsync(searchTerm);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }
    }
}

