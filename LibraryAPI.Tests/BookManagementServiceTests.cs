using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LibraryAPI.Database;
using LibraryAPI.Database.Models;
using LibraryAPI.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

public class BookManagementServiceTests
{
    [Fact]
    public async Task GetAll_WithUserId_ReturnsFilteredBooks()
    {
        var userId = "testUserId";
        var booksData = new List<Book>
        {
            new Book { Id = 1, Title = "Book 1", PublisherId = userId },
            new Book { Id = 2, Title = "Book 2", PublisherId = "otherUserId" },
            new Book { Id = 3, Title = "Book 3", PublisherId = userId }
        }.AsQueryable();

        var mockDbContext = new Mock<ApplicationDbContext>();
        var mockDbSet = new Mock<DbSet<Book>>();
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(booksData.Provider);
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(booksData.Expression);
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(booksData.ElementType);
        mockDbSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(() => booksData.GetEnumerator());

        mockDbContext.Setup(m => m.Books).Returns(mockDbSet.Object);

        var bookManagementService = new BookManagementService(mockDbContext.Object);
        var result = await bookManagementService.GetAll(userId);

        Assert.Equal(2, result.Count());
        Assert.All(result, book => Assert.Equal(userId, book.PublisherId));
    }

    [Fact]
    public async Task Get_ValidBookId_ReturnsBook()
    {
        var bookId = 1;
        var bookData = new Book { Id = bookId, Title = "Test Book" };

        var mockDbContext = new Mock<ApplicationDbContext>();
        mockDbContext.Setup(m => m.Books.FindAsync(bookId)).ReturnsAsync(bookData);

        var bookManagementService = new BookManagementService(mockDbContext.Object);
        var result = await bookManagementService.Get(bookId);

        Assert.NotNull(result);
        Assert.Equal(bookId, result.Id);
        Assert.Equal("Test Book", result.Title);
    }

    [Fact]
    public async Task Create_ValidBook_SavesBookToDbContext()
    {
        var book = new Book { Title = "New Book", Pages = new List<Page> { new Page { Content = "Page 1" } } };
        var mockDbContext = new Mock<ApplicationDbContext>();
        var mockDbSet = new Mock<DbSet<Book>>();
        mockDbContext.Setup(m => m.Books).Returns(mockDbSet.Object);

        var bookManagementService = new BookManagementService(mockDbContext.Object);
        await bookManagementService.Create(book);

        mockDbSet.Verify(m => m.AddAsync(book, default), Times.Once);
        mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Update_ValidBook_UpdatesBookInDbContext()
    {
        var existingBook = new Book { Id = 1, Title = "Existing Book", Pages = new List<Page> { new Page { Content = "Page 1" } } };
        var updatedBook = new Book { Id = 1, Title = "Updated Book", Pages = new List<Page> { new Page { Content = "Updated Page 1" } } };

        var mockDbContext = new Mock<ApplicationDbContext>();
        var mockDbSet = new Mock<DbSet<Book>>();
        mockDbContext.Setup(m => m.Books).Returns(mockDbSet.Object);

        mockDbContext.Setup(m => m.Books.FindAsync(1)).ReturnsAsync(existingBook);

        var bookManagementService = new BookManagementService(mockDbContext.Object);
        await bookManagementService.Update(updatedBook);

        mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        Assert.Equal("Updated Book", existingBook.Title);
        Assert.Single(existingBook.Pages);
        Assert.Equal("Updated Page 1", existingBook.Pages.First().Content);
    }

    [Fact]
    public async Task Delete_ValidBook_RemovesBookFromDbContext()
    {
        var bookId = 1;
        var book = new Book { Id = bookId, Title = "Book to Delete", Pages = new List<Page> { new Page { Content = "Page 1" } } };

        var mockDbContext = new Mock<ApplicationDbContext>();
        var mockDbSet = new Mock<DbSet<Book>>();
        mockDbContext.Setup(m => m.Books).Returns(mockDbSet.Object);

        mockDbContext.Setup(m => m.Books.FindAsync(bookId)).ReturnsAsync(book);

        var bookManagementService = new BookManagementService(mockDbContext.Object);
        await bookManagementService.Delete(book);

        mockDbSet.Verify(m => m.Remove(book), Times.Once);
        mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task IsBookPublisher_ValidBookAndUserId_ReturnsTrue()
    {
        var bookId = 1;
        var userId = "testUserId";
        var book = new Book { Id = bookId, Title = "Test Book", PublisherId = userId };

        var mockDbContext = new Mock<ApplicationDbContext>();
        mockDbContext.Setup(m => m.Books.AnyAsync(It.IsAny<Expression<Func<Book, bool>>>(), default))
        .ReturnsAsync((Expression<Func<Book, bool>> predicate, CancellationToken cancellationToken) =>
        {
            var queryableBooks = mockDbContext.Object.Books.Where(predicate.Compile()).AsQueryable();
            return queryableBooks.Any();
        });

        var bookManagementService = new BookManagementService(mockDbContext.Object);
        var result = await bookManagementService.IsBookPublisher(bookId, userId);

        Assert.True(result);
    }

}
