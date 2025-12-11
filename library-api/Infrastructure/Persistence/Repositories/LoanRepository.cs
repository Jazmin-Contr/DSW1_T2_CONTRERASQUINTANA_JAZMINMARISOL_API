using Microsoft.EntityFrameworkCore;
using library_api.Domain.Entities;
using library_api.Domain.Ports.Out;
using library_api.Infrastructure.Persistence.Context;

namespace library_api.Infrastructure.Persistence.Repositories
{
    public class LoanRepository : Repository<Loan>, ILoanRepository
    {
        public LoanRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
        {
            return await _dbSet
                .Include(l => l.Book)
                .Where(l => l.Status == "Active")
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetLoansByBookIdAsync(int bookId)
        {
            return await _dbSet
                .Include(l => l.Book)
                .Where(l => l.BookId == bookId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetLoansByStudentNameAsync(string studentName)
        {
            return await _dbSet
                .Include(l => l.Book)
                .Where(l => l.StudentName.Contains(studentName))
                .ToListAsync();
        }

        public async Task<Loan?> GetLoanWithBookAsync(int id)
        {
            return await _dbSet
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Loan>> GetAllWithBooksAsync()
        {
            return await _dbSet
                .Include(l => l.Book)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();
        }
    }
}
