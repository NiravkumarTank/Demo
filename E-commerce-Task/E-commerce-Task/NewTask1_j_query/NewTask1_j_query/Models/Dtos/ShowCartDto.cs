namespace NewTask1_j_query.Models.Dtos
{
    public class ShowCartDto
    {
        public int ProductId { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public decimal TotalPrice => Price * Stock;
        public string CategoryName { get; set; }
        public int ShoesNumber {  get; set; }

    }
}
