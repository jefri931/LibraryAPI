
using LibraryAPI.Database.Models;
using System.Collections.Generic;

namespace LibraryAPI.Services 
{
    public interface IBookRentalService 
    {
        Task<Rental?> Get(int rentalId);
        Task<Rental> Rent(string userId, int bookId, DateTime dueDate);
        Task Extend(int rentalId, DateTime dueDate);
        Task Return(int rentalId);
        Task<bool> IsRenter(string userId, int rentalId);
        Task<IEnumerable<Rental>> GetOverdueBooks(string userId);
    }
}