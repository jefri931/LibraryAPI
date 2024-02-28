
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs {
    public class PageDTO
    {
        [Required]
        public string Content { get; set; }
    }
}