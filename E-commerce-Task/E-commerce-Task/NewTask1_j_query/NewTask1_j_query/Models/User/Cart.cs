using NewTask1_j_query.Models.Admin;

namespace NewTask1_j_query.Models.User
{
    public class Cart
    {
        public int CartId { get; set; }
        public UserModel User { get; set; }
        public Product Product { get; set; }    
        public int Stock {  get; set; }
    }
}
