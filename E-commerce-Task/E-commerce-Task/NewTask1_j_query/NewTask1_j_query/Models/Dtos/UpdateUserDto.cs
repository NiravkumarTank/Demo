using System.ComponentModel.DataAnnotations;

namespace NewTask1_j_query.Models.Dtos
{
    public class UpdateUserDto
    {
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

        public string? Gender { get; set; }
    }
}
