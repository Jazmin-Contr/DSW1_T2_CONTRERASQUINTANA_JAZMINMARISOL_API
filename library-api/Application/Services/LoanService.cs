using AutoMapper;
using library_api.Application.DTOs.Loan;
using library_api.Application.Interfaces;
using library_api.Domain.Entities;
using library_api.Domain.Exceptions;
using library_api.Domain.Ports.Out;

namespace library_api.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LoanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<LoanDto?> GetByIdAsync(int id)
        {
            var loan = await _unitOfWork.Loans.GetLoanWithBookAsync(id);
            return loan == null ? null : _mapper.Map<LoanDto>(loan);
        }

        public async Task<IEnumerable<LoanDto>> GetAllAsync()
        {
            var loans = await _unitOfWork.Loans.GetAllWithBooksAsync();
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        public async Task<IEnumerable<LoanDto>> GetActiveLoansAsync()
        {
            var loans = await _unitOfWork.Loans.GetActiveLoansAsync();
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        public async Task<LoanDto> CreateAsync(CreateLoanDto dto)
        {
            // Verificar que el libro existe
            var book = await _unitOfWork.Books.GetByIdAsync(dto.BookId);
            if (book == null)
            {
                throw new NotFoundException("Book", dto.BookId);
            }

            // REGLA DE NEGOCIO: No se puede prestar un libro si el Stock es igual a 0
            if (!book.HasStock())
            {
                throw new BusinessRuleException(
                    "NoStockAvailable",
                    $"No se puede realizar el préstamo. El libro '{book.Title}' no tiene stock disponible.");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Crear el préstamo
                var loan = _mapper.Map<Loan>(dto);
                loan.LoanDate = DateTime.Now;
                loan.Status = "Active";

                var createdLoan = await _unitOfWork.Loans.CreateAsync(loan);

                // REGLA DE NEGOCIO: Al registrar un préstamo, el Stock del libro debe disminuir en 1
                book.DecreaseStock();
                await _unitOfWork.Books.UpdateAsync(book);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Obtener el préstamo con el libro para devolver
                var loanWithBook = await _unitOfWork.Loans.GetLoanWithBookAsync(createdLoan.Id);
                return _mapper.Map<LoanDto>(loanWithBook!);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<LoanDto> ReturnLoanAsync(int id)
        {
            var loan = await _unitOfWork.Loans.GetLoanWithBookAsync(id);
            if (loan == null)
            {
                throw new NotFoundException("Loan", id);
            }

            if (!loan.IsActive())
            {
                throw new BusinessRuleException(
                    "LoanAlreadyReturned",
                    "Este préstamo ya fue devuelto anteriormente.");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Marcar el préstamo como devuelto
                loan.Return();
                await _unitOfWork.Loans.UpdateAsync(loan);

                // REGLA DE NEGOCIO: Al devolver un préstamo, el Stock del libro debe aumentar en 1
                var book = await _unitOfWork.Books.GetByIdAsync(loan.BookId);
                if (book != null)
                {
                    book.IncreaseStock();
                    await _unitOfWork.Books.UpdateAsync(book);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<LoanDto>(loan);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IEnumerable<LoanDto>> GetLoansByStudentAsync(string studentName)
        {
            var loans = await _unitOfWork.Loans.GetLoansByStudentNameAsync(studentName);
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }
    }
}
