using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs {
    public class RentRequestDTO
    {
        [Required]
        public int BookId { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
    }
}