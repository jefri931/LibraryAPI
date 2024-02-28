using LibraryAPI.Database.Models;
using LibraryAPI.Database;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Services 
{
    public class BookManagementService : IBookManagementService 
    {
        private readonly ApplicationDbContext _dbContext;

        public BookManagementService(ApplicationDbContext dbContext) {
            _dbContext = dbContext;
        }

        private void SetPageNumbers(List<Page> pages) {
            for(int i = 1; i <= pages.Count; i++) {
                pages[i - 1].PageNumber = i;
            }
        }

        private async Task ClearPages(int bookId) {
            var pages = await _dbContext.Pages.Where(p => p.BookId == bookId).ToListAsync();
            pages.ForEach(page => _dbContext.Pages.Remove(page));
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Book>> GetAll(string? userId = null) {
            IQueryable<Book> books = _dbContext.Books.Include(b => b.Pages);
            if(userId != null) {
                books = books.Where(b => b.PublisherId == userId);
            }
            return await books.ToListAsync();
        }

        public async Task<Book?> Get(int bookId) {
            return await _dbContext.Books.FindAsync(bookId);
        }

        public async Task Create(Book book) {
            SetPageNumbers(book.Pages);
            await _dbContext.Books.AddAsync(book);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Book book) {
            var newPages = book.Pages;
            book.Pages = [];
            await ClearPages(book.Id);
            SetPageNumbers(newPages);
            book.Pages = newPages;
            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Book book) {
            await ClearPages(book.Id);
            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsBookPublisher(int bookId, string userId) {
            return await _dbContext.Books.AnyAsync(b => b.Id == bookId && b.PublisherId == userId);
        }
    }
}