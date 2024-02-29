using LibraryAPI.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LibraryAPI.Database {
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Rental> Rentals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            #region Book
            modelBuilder.Entity<Book>()
                .HasKey(b => b.Id);
            modelBuilder.Entity<Book>()
                .Property(b => b.Title)
                .HasMaxLength(100)
                .IsRequired();
            modelBuilder.Entity<Book>()
                .Property(b => b.Author)
                .HasMaxLength(100)
                .IsRequired();
            modelBuilder.Entity<Book>()
                .Property(b => b.Description)
                .HasMaxLength(400)
                .IsRequired();
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Pages)
                .WithOne(p => p.Book)
                .HasForeignKey(p => p.BookId)
                .IsRequired(true);
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Publisher)
                .WithMany(p => p.PublishedBooks)
                .HasForeignKey(p => p.PublisherId)
                .IsRequired(true);

            #endregion

            #region Page
            modelBuilder.Entity<Page>()
                .HasKey(p => new { p.BookId, p.PageNumber });
            modelBuilder.Entity<Page>()
                .Property(p => p.Content)
                .IsRequired();
            #endregion

            #region Rental
            modelBuilder.Entity<Rental>()
                .HasKey(b => b.Id);
            modelBuilder.Entity<Rental>()
                .HasOne(p => p.Book)
                .WithMany(p => p.Rentals)
                .HasForeignKey(p => p.BookId)
                .IsRequired(true);
            modelBuilder.Entity<Rental>()
                .HasOne(p => p.User)
                .WithMany(p => p.Rentals)
                .HasForeignKey(p => p.UserId)
                .IsRequired(true);
            modelBuilder.Entity<Rental>()
                .Property(p => p.DueDate)
                .IsRequired();
            modelBuilder.Entity<Rental>()
                .Property(p => p.ReturnedDate)
                .HasDefaultValue(null);
            modelBuilder.Entity<Rental>()
                .Property(p => p.IsReturned)
                .HasDefaultValue(false);
            #endregion
        }
    }
}
