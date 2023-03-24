using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(30,MinimumLength = 6)]
        public string Username { get; set; }
        [Required]
        [StringLength(30,MinimumLength = 6)]
        public string Password { get; set; }
    }
}