
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;

namespace NewTask1_j_query.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly string ConnetionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Task_E_Commerce;Integrated Security=True;Trust Server Certificate=True";

        [HttpPost("createpayment/{userId}")]
        //[Route("createpayment/{userid}")]
        public IActionResult CreatePayment( int userId)
        {
            if (userId <= 0)
            {
                // Log invalid user ID error
                return BadRequest("Invalid user ID.");
            }

            string getPendingOrdersQuery = @"
    SELECT OrderId 
    FROM OrderTable 
    WHERE UserId = @UserId AND Status = 'pending'";

            string calculateTotalQuery = @"
    SELECT SUM(oi.Quantity * p.Price) 
    FROM OrderItems oi
    JOIN Product p ON oi.ProductId = p.ProductId
    WHERE oi.OrderId = @OrderId AND p.Price IS NOT NULL AND oi.Quantity > 0";

            string insertPaymentQuery = @"
    INSERT INTO Payment (UserId, OrderId, TotalPayment, Status, PaymentDate)
    VALUES (@UserId, @OrderId, @TotalPayment, 'true', GETDATE());
    SELECT SCOPE_IDENTITY();";

            string updateOrderStatusToPaidQuery = @"
    UPDATE OrderTable 
    SET Status = 'paid'
    WHERE OrderId = @OrderId";

            using (SqlConnection conn = new SqlConnection(ConnetionString))
            {
                // Open connection
                conn.Open();

                // Begin a transaction
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Get all pending orders for the user
                        List<int> pendingOrders = new List<int>();
                        using (SqlCommand getPendingOrdersCmd = new SqlCommand(getPendingOrdersQuery, conn, transaction))
                        {
                            getPendingOrdersCmd.Parameters.AddWithValue("@UserId", userId);
                            using (var reader = getPendingOrdersCmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    pendingOrders.Add(reader.GetInt32(0)); // OrderId
                                }
                            }
                        }

                        if (pendingOrders.Count == 0)
                        {
                            // Log no pending orders found
                            return BadRequest("No pending orders found for this user.");
                        }

                        // Iterate over each pending order and process the payment
                        foreach (var orderId in pendingOrders)
                        {
                            decimal totalPayment = 0;
                            // Calculate the total amount for the current order
                            using (SqlCommand calculateTotalCmd = new SqlCommand(calculateTotalQuery, conn, transaction))
                            {
                                calculateTotalCmd.Parameters.AddWithValue("@OrderId", orderId);
                                object result = calculateTotalCmd.ExecuteScalar();

                                if (result == DBNull.Value || result == null)
                                {
                                    // Handle the case where no valid items were found for the order
                                    return BadRequest($"No valid items found for Order {orderId}. Please ensure the order contains products with prices.");
                                }
                                else
                                {
                                    totalPayment = Convert.ToDecimal(result);
                                }

                                if (totalPayment <= 0)
                                {
                                    // Log invalid total payment for the order
                                    return BadRequest($"The total amount for Order {orderId} is invalid (zero or negative).");
                                }
                            }

                            // Insert the payment record
                            int paymentId = 0;
                            using (SqlCommand insertPaymentCmd = new SqlCommand(insertPaymentQuery, conn, transaction))
                            {
                                insertPaymentCmd.Parameters.AddWithValue("@UserId", userId);
                                insertPaymentCmd.Parameters.AddWithValue("@OrderId", orderId);
                                insertPaymentCmd.Parameters.AddWithValue("@TotalPayment", totalPayment);

                                paymentId = Convert.ToInt32(insertPaymentCmd.ExecuteScalar());
                            }

                            // Update the order status to 'paid' after payment is completed
                            using (SqlCommand updateOrderStatusCmd = new SqlCommand(updateOrderStatusToPaidQuery, conn, transaction))
                            {
                                updateOrderStatusCmd.Parameters.AddWithValue("@OrderId", orderId);
                                updateOrderStatusCmd.ExecuteNonQuery();
                            }
                        }

                        // Commit the transaction after processing all orders
                        transaction.Commit();

                        // Return the success message
                        return Ok(new
                        {
                            message = "Payments created and all pending orders updated to paid successfully."
                        });
                    }
                    catch (SqlException sqlEx)
                    {
                        // Log SQL exception details
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            $"Database error: {sqlEx.Message} | Query: {calculateTotalQuery} | UserId: {userId}");
                    }
                    catch (Exception ex)
                    {
                        // Log general exception
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            $"Internal server error: {ex.Message}");
                    }
                }
            }
        }


    }
}
