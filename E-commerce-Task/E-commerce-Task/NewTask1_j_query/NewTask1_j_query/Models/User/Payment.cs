namespace NewTask1_j_query.Models.User
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public decimal TotalPayment { get; set; }
        public string Status { get; set; }
        public DateTime PaymentDate { get; set; }
    }

}
