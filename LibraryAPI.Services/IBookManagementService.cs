using System.Security.Claims;
using LibraryAPI.Database.Models;

namespace LibraryAPI.Services 
{
    public interface IBookManagementService {
        Task<IEnumerable<Book>> GetAll(string? userId = null);
        Task<Book?> Get(int bookId);
        Task Create(Book book);
        Task Update(Book book);
        Task Delete(Book book);
        Task<bool> IsBookPublisher(int bookId, string userId);
    }
}