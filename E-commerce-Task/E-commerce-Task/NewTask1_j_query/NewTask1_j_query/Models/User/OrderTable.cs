using NewTask1_j_query.Models.Admin; 

namespace NewTask1_j_query.Models.User
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string ShippingAddress { get; set; }  
        public DateTime? OrderDate { get; set; }
        public string Status { get; set; }
        public int ShoesNumber {  get; set; }

    }
}
