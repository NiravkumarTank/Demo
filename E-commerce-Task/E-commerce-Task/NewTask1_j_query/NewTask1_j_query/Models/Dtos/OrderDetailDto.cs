namespace NewTask1_j_query.Models.Dtos
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string ShippingAddress { get; set; }
        public string Status { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int ShoesNumber { get; set; }
        public DateOnly OrderDate {  get; set; }

    }
}
