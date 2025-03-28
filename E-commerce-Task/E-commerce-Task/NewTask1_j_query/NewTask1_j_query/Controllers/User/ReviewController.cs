using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NewTask1_j_query.Models.Dtos;
using NewTask1_j_query.Models.User;

namespace NewTask1_j_query.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly string ConnetionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Task_E_Commerce;Integrated Security=True;Trust Server Certificate=True";




        // Add Review
        [HttpPost("addReview")]
        public IActionResult AddReview([FromBody] ReviewModel review)
        {
            if (review == null || string.IsNullOrWhiteSpace(review.Comment))
            {
                return BadRequest("Review data is required.");
            }

            using (SqlConnection con = new SqlConnection(ConnetionString))
            {
                SqlCommand cmd = new SqlCommand("AddReview", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", review.UserId);
                cmd.Parameters.AddWithValue("@ProductId", review.ProductId);
                cmd.Parameters.AddWithValue("@Comment", review.Comment);

                con.Open();
                object result = cmd.ExecuteScalar(); // Get the inserted ReviewId

                int reviewId = (result != null && int.TryParse(result.ToString(), out int id)) ? id : 0;

                if (reviewId > 0)
                {
                    return Ok(new { message = "Review added successfully.", ReviewId = reviewId });
                }
                return StatusCode(500, "Failed to add review.");
            }
        }

        // Get reviews by Game ID
        [HttpGet("GetProductIdReview/{ProductId}")]
        public IActionResult GetReviewsByGameId(int ProductId)
        {
            if (ProductId <= 0)
            {
                return BadRequest("Invalid Game ID.");
            }

            List<ProductReviewDTO> reviews = new List<ProductReviewDTO>();

            using (SqlConnection con = new SqlConnection(ConnetionString))
            {
                SqlCommand cmd = new SqlCommand("GetReviewsByGameId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", ProductId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reviews.Add(new ProductReviewDTO
                    {

                        Fullname = reader["Fullname"].ToString(),
                        Comment = reader["Comment"].ToString()
                    });
                }
            }

            if (reviews.Count == 0)
            {
                return Ok("No reviews found for this Product.");
            }
            return Ok(reviews);
        }

        // Get All Reviews
        [HttpGet("getAllReviews")]
        public IActionResult GetAllReviews()
        {
            List<ReviewGetDTO> reviews = new List<ReviewGetDTO>();

            using (SqlConnection con = new SqlConnection(ConnetionString))
            {
                SqlCommand cmd = new SqlCommand("GetAllReviews", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reviews.Add(new ReviewGetDTO
                    {
                        ReviewId = Convert.ToInt32(reader["ReviewId"]),
                        UserId = Convert.ToInt32(reader["UserId"]),
                        ProductId = Convert.ToInt32(reader["ProductId"]),
                        Fullname = reader["Fullname"].ToString(),
                        Email = reader["Email"].ToString(),
                        ProductName = reader["Name"].ToString(),
                        Comment = reader["Comment"].ToString(),
                        Reviews_date = DateOnly.FromDateTime(Convert.ToDateTime(reader["reviews_date"]))

                    });
                }
            }

            if (reviews.Count > 0)
            {
                return Ok(reviews);
            }
            return NotFound("No reviews found.");
        }

    }
}
