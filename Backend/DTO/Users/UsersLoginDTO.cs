using System.ComponentModel.DataAnnotations;

namespace Backend.DTO.Users
{
    public class UsersLoginDTO
    {
        [Required]
        public string UserNameOrEmail { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
