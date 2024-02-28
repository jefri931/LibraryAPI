using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs {
    public class LoginDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}