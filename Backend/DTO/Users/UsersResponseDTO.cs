using System.ComponentModel.DataAnnotations;

namespace Backend.DTO.Users
{
    public class UsersResponseDTO
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;
    }
}
