using System.ComponentModel.DataAnnotations;

namespace DatingApp.Api.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        public string  Username { get; set; }
        [Required]
        [StringLength(8,MinimumLength=3,ErrorMessage="Pasword should be between 3 and 8 characters")]
        public string  Password { get; set; }
    }
}