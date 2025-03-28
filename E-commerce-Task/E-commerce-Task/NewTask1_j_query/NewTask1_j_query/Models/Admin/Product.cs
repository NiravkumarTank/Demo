using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewTask1_j_query.Models.Admin
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public Category Category { get; set; }
        [Required]
        public Subcategory Subcategory { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Stock {  get; set; }
        public string? ImageUrl { get; set; }
        public DateOnly? CreatedDate { get; set; }
        
        
        [NotMapped]
        public IFormFile? Image {  get; set; }




    }
}
