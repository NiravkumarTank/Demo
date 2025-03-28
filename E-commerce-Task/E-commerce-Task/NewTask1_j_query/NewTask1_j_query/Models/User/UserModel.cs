using System.ComponentModel.DataAnnotations;

namespace NewTask1_j_query.Models.User
{
    public class UserModel
    {
        public int UserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

        public string? Gender { get; set; }
        public string? Role {  get; set; }
    }
}
