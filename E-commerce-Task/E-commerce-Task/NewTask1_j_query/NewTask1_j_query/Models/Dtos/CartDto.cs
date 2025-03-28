using NewTask1_j_query.Models.Admin;
using NewTask1_j_query.Models.User;

namespace NewTask1_j_query.Models.Dtos
{
    public class CartDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Stock { get; set; }
        public int? ShoesNumber { get; set; }    
    }
}
