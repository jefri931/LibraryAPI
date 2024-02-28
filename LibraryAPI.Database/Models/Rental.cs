using Microsoft.AspNetCore.Identity;

namespace LibraryAPI.Database.Models 
{
    public class Rental 
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public bool IsReturned { get; set; }
    }
}