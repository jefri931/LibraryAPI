using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs {
    public class ExtendRequestDTO
    {
        [Required]
        public int RentalId { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
    }
}