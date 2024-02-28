namespace LibraryAPI.DTOs {
    public class RentalDTO
    {
        public int Id { get; set; }
        public DateTime DueDate { get; set; }
        public BookDTO Book { get; set; }
    }
}