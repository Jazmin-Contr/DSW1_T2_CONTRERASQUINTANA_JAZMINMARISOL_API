using library_api.Application.DTOs.Loan;

namespace library_api.Application.Interfaces
{
    public interface ILoanService
    {
        Task<LoanDto?> GetByIdAsync(int id);
        Task<IEnumerable<LoanDto>> GetAllAsync();
        Task<IEnumerable<LoanDto>> GetActiveLoansAsync();
        Task<LoanDto> CreateAsync(CreateLoanDto dto);
        Task<LoanDto> ReturnLoanAsync(int id);
        Task<IEnumerable<LoanDto>> GetLoansByStudentAsync(string studentName);
    }
}
