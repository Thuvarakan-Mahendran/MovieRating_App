using System.ComponentModel.DataAnnotations;

namespace Backend.DTO.Users
{
    public class UsersRegistrationDTO
    {
        [Required]
        [StringLength(50,MinimumLength =3)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50,MinimumLength =8)]
        public string Password { get; set; } = string.Empty;
    }
}
