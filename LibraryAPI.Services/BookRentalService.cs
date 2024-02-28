
using LibraryAPI.Exceptions;
using LibraryAPI.Database.Models;
using LibraryAPI.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LibraryAPI.Services 
{
    public class BookRentalService : IBookRentalService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IBookManagementService _bookManager;

        public BookRentalService(ApplicationDbContext dbContext, IBookManagementService bookManager) {
            _dbContext = dbContext;
            _bookManager = bookManager;
        }

        public async Task<Rental?> Get(int rentalId) {
            return await _dbContext.Rentals.FindAsync(rentalId);
        }

        public async Task<Rental> Rent(string userId, int bookId, DateTime dueDate) {
            if(DateTime.UtcNow > dueDate) {
                throw new ArgumentOutOfRangeException("Due Date cannot be less than the current date.");
            }

            var book = await _bookManager.Get(bookId);
            if(book == null) {
                throw new NotFoundException("Book not found.");
            }
            var rental = new Rental()
            {
                BookId = bookId,
                UserId = userId,
                DueDate = dueDate
            };
            await _dbContext.Rentals.AddAsync(rental);
            await _dbContext.SaveChangesAsync();

            return rental;
        }

        public async Task Extend(int rentalId, DateTime dueDate) {
            if(DateTime.UtcNow > dueDate) {
                throw new ArgumentOutOfRangeException("Due Date cannot be less than the current date.");
            }

            var rental = await Get(rentalId);
            if(rental == null) {
                throw new NotFoundException("Rental not found.");
            }

            if(rental.DueDate > dueDate) {
                throw new ArgumentOutOfRangeException("Due Date cannot be less than the current due date.");
            }

            rental.DueDate = dueDate;
            _dbContext.Rentals.Update(rental);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Return(int rentalId) {
            var rental = await Get(rentalId);
            if(rental == null) {
                throw new NotFoundException("Rental not found.");
            }

            rental.IsReturned = true;
            _dbContext.Rentals.Update(rental);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsRenter(string userId, int rentalId) {
            return await _dbContext.Rentals.AnyAsync(r => r.UserId == userId && r.Id == rentalId);
        }

        public async Task<IEnumerable<Rental>> GetOverdueBooks(string userId) {
            return await _dbContext.Rentals.Include(r => r.Book).Where(r => r.UserId == userId && r.IsReturned == false && r.DueDate < DateTime.UtcNow).ToListAsync();
        }
    }
}