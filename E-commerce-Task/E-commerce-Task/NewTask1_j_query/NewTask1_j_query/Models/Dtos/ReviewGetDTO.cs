namespace NewTask1_j_query.Models.Dtos
{
    public class ReviewGetDTO
    {
        public int ReviewId { get; set; }      
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }

        public string ProductName { get; set; }

        public string Comment { get; set; }
        public DateOnly Reviews_date { get; set; }

    }
}
