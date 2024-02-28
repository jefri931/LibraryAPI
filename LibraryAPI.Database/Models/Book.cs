namespace LibraryAPI.Database.Models {
    public class Book
    {
        public Book () {
            Pages = [];
            Rentals = [];
        }
        
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string PublisherId { get; set; }
        public AppUser Publisher { get; set; }
        public List<Page> Pages { get; set; }
        public List<Rental> Rentals { get; set; }

    }
}