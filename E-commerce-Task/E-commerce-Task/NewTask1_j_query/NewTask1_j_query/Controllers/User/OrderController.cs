using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using NewTask1_j_query.Models.Admin;
using NewTask1_j_query.Models.Dtos;
using NewTask1_j_query.Models.User;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NewTask1_j_query.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly string ConnetionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Task_E_Commerce;Integrated Security=True;Trust Server Certificate=True";

      

        [HttpPost]
        [Route("PlaceOrder/")]
        public IActionResult PlaceOrder([FromBody] Order order)
        {
            if (order.UserId <= 0 || string.IsNullOrEmpty(order.ShippingAddress))
            {
                return BadRequest("Invalid user ID or shipping address.");
            }

            string checkCartQuery = "SELECT COUNT(*) FROM Cart WHERE UserId = @UserId";
            string insertOrderQuery = @"
        INSERT INTO OrderTable (UserId, ShippingAddress, Status)
        VALUES (@UserId, @ShippingAddress, 'pending');
        SELECT SCOPE_IDENTITY();";  // Get the newly generated OrderId

            string insertOrderItemsQuery = @"
        INSERT INTO OrderItems (OrderId, ProductId, Quantity, UserId,ShoesNumber)
        SELECT @OrderId, ProductId, Stock, UserId,ShoesNumber
        FROM Cart
        WHERE UserId = @UserId";

            string deleteCartItemsQuery = "DELETE FROM Cart WHERE UserId = @UserId";

            using (SqlConnection conn = new SqlConnection(ConnetionString))
            {
                try
                {
                    conn.Open();

                    // Check if the user has items in their cart
                    using (SqlCommand checkCartCmd = new SqlCommand(checkCartQuery, conn))
                    {
                        checkCartCmd.Parameters.AddWithValue("@UserId", order.UserId);
                        int cartItemCount = (int)checkCartCmd.ExecuteScalar();

                        if (cartItemCount > 0)
                        {
                            // There are items in the cart, so insert the order and change status to 'pending'
                            using (SqlCommand insertOrderCmd = new SqlCommand(insertOrderQuery, conn))
                            {
                                insertOrderCmd.Parameters.AddWithValue("@UserId", order.UserId);
                                insertOrderCmd.Parameters.AddWithValue("@ShippingAddress", order.ShippingAddress);
                                 

                                // Execute the insert and get the new OrderId using SCOPE_IDENTITY()
                                int orderId = Convert.ToInt32(insertOrderCmd.ExecuteScalar());

                                // Now insert cart items into OrderItems table with the generated OrderId
                                using (SqlCommand insertOrderItemsCmd = new SqlCommand(insertOrderItemsQuery, conn))
                                {
                                    insertOrderItemsCmd.Parameters.AddWithValue("@OrderId", orderId);  // Use the generated OrderId
                                    insertOrderItemsCmd.Parameters.AddWithValue("@UserId", order.UserId);
                                    //insertOrderItemsCmd.Parameters.AddWithValue("@ShoesNumber", order.ShoesNumber);
                                    insertOrderItemsCmd.ExecuteNonQuery();
                                }

                                // Once the order is placed, remove cart items
                                using (SqlCommand deleteCartCmd = new SqlCommand(deleteCartItemsQuery, conn))
                                {
                                    deleteCartCmd.Parameters.AddWithValue("@UserId", order.UserId);
                                    deleteCartCmd.ExecuteNonQuery();
                                }

                                // Return the order ID of the newly created order
                                return Ok(new { message = "Order placed successfully", OrderId = orderId });
                            }
                        }
                        else
                        {
                            // No items in cart, show message
                            return BadRequest("You have no items in your cart. Please add items to your cart first.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error or handle it as needed
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
                }
            }
        }






        [HttpGet]
        [Route("GetPendingOrders")]
        public IActionResult GetPendingOrders(int userId)
        {
            string getOrderQuery = @"
            SELECT o.OrderId, o.UserId, o.ShippingAddress, o.Status, 
                   oi.ProductId, p.Name, p.ImageUrl, oi.Quantity,p.ProductId,p.Price,oi.ShoesNumber
            FROM OrderTable o
            INNER JOIN OrderItems oi ON o.OrderId = oi.OrderId
            INNER JOIN Product p ON oi.ProductId = p.ProductId
            WHERE o.UserId = @UserId AND o.Status = 'Pending'";  

            List<OrderDetailDto> orderDetails = new List<OrderDetailDto>();

            using (SqlConnection conn = new SqlConnection(ConnetionString))
            {
                try
                {
                    conn.Open();

                    // Fetch orders with the status 'Pending' for the given userId
                    using (SqlCommand getOrderCmd = new SqlCommand(getOrderQuery, conn))
                    {
                        getOrderCmd.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataReader reader = getOrderCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Create a DTO for each order item
                                var orderDetail = new OrderDetailDto
                                {
                                    OrderId = reader.GetInt32(0),
                                    UserId = reader.GetInt32(1),
                                    ShippingAddress = reader.GetString(2),
                                    Status = reader.GetString(3),
                                    ProductName = reader.GetString(5),
                                    ProductImage = reader.GetString(6),
                                    Quantity = reader.GetInt32(7),
                                    ProductId = reader.GetInt32(8),
                                    Price = reader.GetDecimal(9),
                                    ShoesNumber= reader.GetInt32(10),
                                };

                                // Add the order item to the list
                                orderDetails.Add(orderDetail);
                            }
                        }
                    }

                    // Build the response with the image URL
                    var data = orderDetails.Select(item => new
                    {
                        item.OrderId,
                        item.UserId,
                        item.ShippingAddress,
                        item.Status,
                        item.ProductName,
                        ProductImage = $"{Request.Scheme}://{Request.Host}/Upload/{item.ProductImage}",
                        item.Quantity,
                        item.ProductId,
                        item.Price,
                        Total = (item.Quantity * item.Price),
                        item.ShoesNumber,
                    });

                    // If no pending orders were found, return a message suggesting to add items to the cart
                    if (orderDetails.Count == 0)
                    {
                        return Ok(new { message = "No pending orders" });
                    }

                    // Return the order details
                    return Ok(data);
                }
                catch (Exception ex)
                {
                    // Log the error or handle it as needed
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
                }
            }
        }




        //[HttpDelete]
        //[Route("RemoveOrder")]
        //public IActionResult RemoveOrder(int orderId, int productId)
        //{
        //    string getOrderItemQuery = @"
        //    SELECT Quantity 
        //    FROM OrderItems 
        //    WHERE OrderId = @OrderId AND ProductId = @ProductId";

        //    //string removeOrderItemQuery = @"
        //    //DELETE FROM OrderItems 
        //    //WHERE OrderId = @OrderId AND ProductId = @ProductId";

        //    string updateProductStockQuery = @"
        //    UPDATE Product 
        //    SET Stock = Stock + @Quantity
        //    WHERE ProductId = @ProductId";

        //    string updateOrderStatusQuery = @"
        //    UPDATE OrderTable 
        //    SET Status = 'Cancelled' 
        //    WHERE OrderId = @OrderId";

        //    using (SqlConnection conn = new SqlConnection(ConnetionString))
        //    {
        //        try
        //        {
        //            conn.Open();

        //            // Begin a transaction to ensure consistency
        //            using (SqlTransaction transaction = conn.BeginTransaction())
        //            {
        //                try
        //                {
        //                    // Step 1: Fetch the quantity of the order item for the given OrderId and ProductId
        //                    int quantity = 0;
        //                    using (SqlCommand getOrderItemCmd = new SqlCommand(getOrderItemQuery, conn, transaction))
        //                    {
        //                        getOrderItemCmd.Parameters.AddWithValue("@OrderId", orderId);
        //                        getOrderItemCmd.Parameters.AddWithValue("@ProductId", productId);

        //                        var result = getOrderItemCmd.ExecuteScalar();
        //                        if (result != null)
        //                        {
        //                            quantity = Convert.ToInt32(result);
        //                        }
        //                        else
        //                        {
        //                            return NotFound("Order item not found for the given OrderId and ProductId.");
        //                        }
        //                    }

        //                    // Step 2: Remove the order item from the OrderItems table
        //                    //using (SqlCommand removeOrderItemCmd = new SqlCommand(removeOrderItemQuery, conn, transaction))
        //                    //{
        //                    //    removeOrderItemCmd.Parameters.AddWithValue("@OrderId", orderId);
        //                    //    removeOrderItemCmd.Parameters.AddWithValue("@ProductId", productId);
        //                    //    removeOrderItemCmd.ExecuteNonQuery();
        //                    //}

        //                    // Step 3: Update the stock of the product
        //                    using (SqlCommand updateProductStockCmd = new SqlCommand(updateProductStockQuery, conn, transaction))
        //                    {
        //                        updateProductStockCmd.Parameters.AddWithValue("@Quantity", quantity);
        //                        updateProductStockCmd.Parameters.AddWithValue("@ProductId", productId);
        //                        updateProductStockCmd.ExecuteNonQuery();
        //                    }

        //                    // Step 4: Update the status of the order to 'Cancelled'
        //                    using (SqlCommand updateOrderStatusCmd = new SqlCommand(updateOrderStatusQuery, conn, transaction))
        //                    {
        //                        updateOrderStatusCmd.Parameters.AddWithValue("@OrderId", orderId);
        //                        updateOrderStatusCmd.ExecuteNonQuery();
        //                    }

        //                    // Commit the transaction after successful removal, stock update, and status change
        //                    transaction.Commit();

        //                    return Ok(new { message = "Order item removed, product stock updated, and order status cancelled successfully." });
        //                }
        //                catch (Exception ex)
        //                {
        //                    // Rollback in case of any error
        //                    transaction.Rollback();
        //                    return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // Handle connection issues or other external errors
        //            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        //        }
        //    }
        //}
        [HttpDelete]
        [Route("RemoveOrder")]
        public IActionResult RemoveOrder(int orderId, int productId)
        {
            string getOrderItemQuery = @"
        SELECT Quantity 
        FROM OrderItems 
        WHERE OrderId = @OrderId AND ProductId = @ProductId";

            string updateProductStockQuery = @"
        UPDATE Product 
        SET Stock = Stock + @Quantity
        WHERE ProductId = @ProductId";

            string removeOrderItemQuery = @"
        DELETE FROM OrderItems 
        WHERE OrderId = @OrderId AND ProductId = @ProductId";

            string updateOrderStatusQuery = @"
        UPDATE OrderTable 
        SET Status = 'Cancelled' 
        WHERE OrderId = @OrderId";

            using (SqlConnection conn = new SqlConnection(ConnetionString))
            {
                try
                {
                    conn.Open();

                    // Begin a transaction to ensure consistency
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Step 1: Fetch the quantity of the product in the order
                            int quantity = 0;
                            using (SqlCommand getOrderItemCmd = new SqlCommand(getOrderItemQuery, conn, transaction))
                            {
                                getOrderItemCmd.Parameters.AddWithValue("@OrderId", orderId);
                                getOrderItemCmd.Parameters.AddWithValue("@ProductId", productId);

                                var result = getOrderItemCmd.ExecuteScalar();
                                if (result != null)
                                {
                                    quantity = Convert.ToInt32(result);
                                }
                                else
                                {
                                    return NotFound("No order item found for the given OrderId and ProductId.");
                                }
                            }

                            // Step 2: Update the stock for the product
                            using (SqlCommand updateProductStockCmd = new SqlCommand(updateProductStockQuery, conn, transaction))
                            {
                                updateProductStockCmd.Parameters.AddWithValue("@Quantity", quantity);
                                updateProductStockCmd.Parameters.AddWithValue("@ProductId", productId);
                                updateProductStockCmd.ExecuteNonQuery();
                            }

                            // Step 3: Remove the product from the OrderItems table
                            using (SqlCommand removeOrderItemCmd = new SqlCommand(removeOrderItemQuery, conn, transaction))
                            {
                                removeOrderItemCmd.Parameters.AddWithValue("@OrderId", orderId);
                                removeOrderItemCmd.Parameters.AddWithValue("@ProductId", productId);
                                removeOrderItemCmd.ExecuteNonQuery();
                            }

                            // Step 4: Check if the order has any items left. If not, update the order status to 'Cancelled'
                            string checkOrderItemsQuery = @"
                        SELECT COUNT(*) 
                        FROM OrderItems 
                        WHERE OrderId = @OrderId";

                            using (SqlCommand checkOrderItemsCmd = new SqlCommand(checkOrderItemsQuery, conn, transaction))
                            {
                                checkOrderItemsCmd.Parameters.AddWithValue("@OrderId", orderId);
                                int remainingItems = (int)checkOrderItemsCmd.ExecuteScalar();

                                if (remainingItems == 0)
                                {
                                    using (SqlCommand updateOrderStatusCmd = new SqlCommand(updateOrderStatusQuery, conn, transaction))
                                    {
                                        updateOrderStatusCmd.Parameters.AddWithValue("@OrderId", orderId);
                                        updateOrderStatusCmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            // Commit the transaction after all operations
                            transaction.Commit();

                            return Ok(new { message = "Product removed from order, product stock updated." });
                        }
                        catch (Exception ex)
                        {
                            // Rollback in case of any error
                            transaction.Rollback();
                            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle connection issues or other external errors
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
                }
            }
        }




        [HttpGet]
        [Route("GetPaidOrders")]
        public IActionResult GetPaidOrders(int userId)
        {
            string getOrderQuery = @"
SELECT o.OrderId, o.UserId, o.ShippingAddress, o.Status, 
       oi.ProductId, p.Name, p.ImageUrl, oi.Quantity,p.ProductId,p.Price,oi.ShoesNumber,o.OrderDate
FROM OrderTable o
INNER JOIN OrderItems oi ON o.OrderId = oi.OrderId
INNER JOIN Product p ON oi.ProductId = p.ProductId
WHERE o.UserId = @UserId AND o.Status = 'Paid' ORDER BY o.OrderId DESC";

            List<OrderDetailDto> orderDetails = new List<OrderDetailDto>();

            using (SqlConnection conn = new SqlConnection(ConnetionString))
            {
                try
                {
                    conn.Open();

                    // Fetch orders with the status 'Paid' for the given userId
                    using (SqlCommand getOrderCmd = new SqlCommand(getOrderQuery, conn))
                    {
                        getOrderCmd.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataReader reader = getOrderCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Create a DTO for each order item
                                var orderDetail = new OrderDetailDto
                                {
                                    OrderId = reader.GetInt32(0),
                                    UserId = reader.GetInt32(1),
                                    ShippingAddress = reader.GetString(2),
                                    Status = reader.GetString(3),
                                    ProductName = reader.GetString(5),
                                    ProductImage = reader.GetString(6),
                                    Quantity = reader.GetInt32(7),
                                    ProductId = reader.GetInt32(8),
                                    Price = reader.GetDecimal(9),
                                    ShoesNumber = reader.GetInt32(10),
                                    OrderDate = DateOnly.FromDateTime(reader.GetDateTime(11)),
                                };

                                // Add the order item to the list
                                orderDetails.Add(orderDetail);
                            }
                        }
                    }

                    // Group orders by OrderDate
                    var groupedOrders = orderDetails
                        .GroupBy(order => order.OrderDate)
                        .OrderByDescending(group => group.Key)  // Order by latest OrderDate first
                        .Take(5)  // Take the last 5 order dates
                        .Select(group => new
                        {
                            OrderDate = group.Key,
                            Orders = group.Select(item => new
                            {
                                item.OrderId,
                                item.UserId,
                                item.ShippingAddress,
                                item.Status,
                                item.ProductName,
                                ProductImage = $"{Request.Scheme}://{Request.Host}/Upload/{item.ProductImage}",
                                item.Quantity,
                                item.ProductId,
                                item.Price,
                                Total = (item.Quantity * item.Price),
                                item.ShoesNumber,
                                item.OrderDate
                            })
                        });

                    // If no paid orders were found, return a custom message
                    if (!groupedOrders.Any())
                    {
                        return Ok(new { Message = "No Paid Orders" });
                    }

                    // Return the grouped order details
                    return Ok(groupedOrders);
                }
                catch (Exception ex)
                {
                    // Log the error or handle it as needed
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
                }
            }
        }


    }

}
