using Microsoft.EntityFrameworkCore;
using library_api.Domain.Entities;
using library_api.Domain.Ports.Out;
using library_api.Infrastructure.Persistence.Context;

namespace library_api.Infrastructure.Persistence.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Book?> GetByISBNAsync(string isbn)
        {
            return await _dbSet.FirstOrDefaultAsync(b => b.ISBN == isbn);
        }

        public async Task<bool> ISBNExistsAsync(string isbn)
        {
            return await _dbSet.AnyAsync(b => b.ISBN == isbn);
        }

        public async Task<IEnumerable<Book>> GetBooksWithStockAsync()
        {
            return await _dbSet.Where(b => b.Stock > 0).ToListAsync();
        }

        public async Task<IEnumerable<Book>> SearchByTitleOrAuthorAsync(string searchTerm)
        {
            return await _dbSet
                .Where(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm))
                .ToListAsync();
        }
    }
}
