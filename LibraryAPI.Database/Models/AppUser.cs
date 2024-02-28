
using Microsoft.AspNetCore.Identity;

namespace LibraryAPI.Database.Models 
{
    public class AppUser : IdentityUser
    {
        public AppUser() {
            PublishedBooks = [];
            Rentals = [];
        }
        public string Name { get; set; }
        public List<Book> PublishedBooks { get; set; }
        public List<Rental> Rentals { get; set; }
    }
}