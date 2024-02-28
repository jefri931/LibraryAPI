using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs {
    public class BookDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "The {0} must be between {2} and {1} characters long.")]
        public string Title { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "The {0} must be between {2} and {1} characters long.")]
        public string Author { get; set; }
        [Required]
        [StringLength(400, MinimumLength = 1, ErrorMessage = "The {0} must be between {2} and {1} characters long.")]
        public string Description { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "At least one page is required.")]
        public List<PageDTO> Pages { get; set; }
    }
}