using NewTask1_j_query.Models.Admin;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewTask1_j_query.Models.Dtos
{
    public class ProductAddDto
    {
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int c_id { get; set; }
        [Required]
        public int sc_id { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }
    }
}
