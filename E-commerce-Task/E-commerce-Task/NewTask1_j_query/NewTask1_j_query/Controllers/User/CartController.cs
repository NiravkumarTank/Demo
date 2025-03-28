using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NewTask1_j_query.Models.Admin;
using NewTask1_j_query.Models.Dtos;
using NewTask1_j_query.Models.User;
using PdfSharpCore.Pdf;
using PdfSharpCore;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using NewTask1_j_query.Repository.Interface;

namespace NewTask1_j_query.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly string ConnetionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Task_E_Commerce;Integrated Security=True;Trust Server Certificate=True";
        private readonly IEmailSender _emailSender;

        public CartController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost]
        [Route("AddToCart")]
        public ActionResult<CartDto> AddToCart([FromForm] CartDto cartDto)
            {

            try
            {
                SqlConnection conn = new SqlConnection(ConnetionString);

               
                string stockQuery = "SELECT Stock FROM Product WHERE ProductId = @pid";
                SqlCommand stockCmd = new SqlCommand(stockQuery, conn);
                stockCmd.Parameters.AddWithValue("@pid", cartDto.ProductId);

                conn.Open();
                int stock = (int)stockCmd.ExecuteScalar();
                conn.Close();

                if (stock == 0)
                {
                    return Ok(new { message = "No stock available for this product." });
                }

                string query = "SELECT COUNT(*) FROM Cart WHERE UserId = @uid AND ProductId = @pid";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uid", cartDto.UserId);
                cmd.Parameters.AddWithValue("@pid", cartDto.ProductId);

                conn.Open();
                int countUser = (int)cmd.ExecuteScalar();
                conn.Close();

                if (countUser == 0)
                {
                    SqlCommand cmd1 = new SqlCommand("insertCartProductStock", conn);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@uid", cartDto.UserId);
                    cmd1.Parameters.AddWithValue("@pid", cartDto.ProductId);
                    cmd1.Parameters.AddWithValue("@shoesNumber", cartDto.ShoesNumber);

                    conn.Open();
                    cmd1.ExecuteNonQuery();
                    conn.Close();
                }
                else
                {

                    string updateStockQuery = "updtaeCartProductStock";
                    SqlCommand cmd2 = new SqlCommand(updateStockQuery, conn);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@UserId", cartDto.UserId);
                    cmd2.Parameters.AddWithValue("@ProductId", cartDto.ProductId);


                    conn.Open();
                    int rowAffected = cmd2.ExecuteNonQuery();
                    conn.Close();

                    if (rowAffected > 0)
                    {
                        return Ok(new { message = "Stock Increase successfully.", data = cartDto });
                    }
                    else
                    {
                        return Ok(new { message = "No product available, stock is 0." });

                    }
                }

                return Ok(cartDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            }








        [HttpGet("{userId}")]
        public ActionResult<IEnumerable<ShowCartDto>> GetAllCart([FromRoute] int userId)
        {
            SqlConnection conn = new SqlConnection(ConnetionString);


            SqlCommand cmd = new SqlCommand("ShowAllCart", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<ShowCartDto> cartDtos = new List<ShowCartDto>();

            while (reader.Read())
            {
                ShowCartDto showCartDto = new ShowCartDto
                {   ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                    ImageUrl = $"{Request.Scheme}://{Request.Host}/Upload/{reader.GetString(reader.GetOrdinal("ImageUrl"))}",
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                    Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                    CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                    ShoesNumber = reader.GetInt32(reader.GetOrdinal("ShoesNumber")),
                };

                cartDtos.Add(showCartDto);
            }

            conn.Close();

            if (cartDtos.Any())
            {
                return Ok(cartDtos);
            }
            else
            {
                return Ok(new { message = "No carts found for the specified user." });
            }
        }




        [HttpPost]
        [Route("DecreaseStock")]
        public ActionResult DecreaseStock([FromForm] CartDto cartDto)
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConnetionString);

                SqlCommand cmd = new SqlCommand("decreaseStock", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@uid", cartDto.UserId); 
                cmd.Parameters.AddWithValue("@pid", cartDto.ProductId);
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                conn.Close();

                if (result > 0)
                {
                    return Ok(new { message = "Stock decreased successfully." });
                }
                else
                {
                    return BadRequest("Failed to decrease the stock.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete]
        [Route("DeleteCart")]
        public ActionResult DeleteCart(int uid, int pid)
        {
            SqlConnection conn = new SqlConnection(ConnetionString);
            conn.Open ();
            //string query = "DELETE FROM Cart WHERE UserId = @UserId AND ProductId = @ProductId";

            SqlCommand cmd = new SqlCommand("removeToCart", conn);
            cmd.CommandType= CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id",pid);
            cmd.Parameters.AddWithValue("@uid", uid);
            int row = cmd.ExecuteNonQuery();
            conn.Close();
            if (row > 0)
            {
                return Ok(new { message = "Product removed from cart successfully." });
            }
            else 
            {
                return NotFound();
            }

        }

        [HttpPost]
        [Route("update-size")]
        public ActionResult UpdateSize([FromForm] int uid, [FromForm] int pid, [FromForm] int newSize)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnetionString))
                {
                    conn.Open();

                    // Create a command to execute the stored procedure for updating the size in the cart
                    //SqlCommand cmd = new SqlCommand("updateSizeInCart", conn);
                    SqlCommand cmd = new SqlCommand(" UPDATE Cart SET ShoesNumber = @newSize WHERE ProductId = @id AND UserId = @uid;", conn);
                    //cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters for the stored procedure
                    cmd.Parameters.AddWithValue("@id", pid);    // ProductId
                    cmd.Parameters.AddWithValue("@uid", uid);   // UserId
                    cmd.Parameters.AddWithValue("@newSize", newSize);  // New SelectedSize

                    // Execute the command and check how many rows were affected
                    int row = cmd.ExecuteNonQuery();

                    if (row > 0)
                    {
                        return Ok(new { message = "Size updated successfully." });
                    }
                    else
                    {
                        return NotFound(new { message = "Product not found in the cart or size is the same." });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed and return a 500 error response
                return StatusCode(500, new { message = "An error occurred while updating the product size.", error = ex.Message });
            }
        }





        [HttpGet("total-cart-items/{userId}")]
        public async Task<IActionResult> GetTotalCartItems(int userId)
        {
            int totalItems = 0;

            using (var connection = new SqlConnection(ConnetionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("SELECT count(Stock) as total_cart_items FROM Cart WHERE UserId = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", userId);

                var result = await command.ExecuteScalarAsync();
                if (result != DBNull.Value)
                {
                    totalItems = Convert.ToInt32(result);
                }
            }

            return Ok(new { total_cart_items = totalItems });
        }

          
        





        [HttpGet]
        [Route("PdfGenerate/{userId}")]
        public IActionResult GetPdf(int userId)
        {
            // Connection to the database
            SqlConnection conn = new SqlConnection(ConnetionString);

            // Open connection to the database
            conn.Open();

            // Get User and Latest Order Details with a single inline SQL query
            SqlCommand cmdUser = new SqlCommand(@"
                    SELECT 
                        (u.FirstName + ' ' + u.LastName) AS UserName, 
                        u.Email, 
                        o.ShippingAddress,
                        o.OrderId,
                        o.OrderDate
                    FROM UserModel u
                    JOIN OrderTable o 
                        ON u.UserId = o.UserId
                    WHERE u.UserId = @UserId
                    AND o.OrderId = (
                        SELECT TOP 1 OrderId
                        FROM OrderTable
                        WHERE UserId = @UserId
                        ORDER BY OrderId DESC
                    )", conn);

            cmdUser.Parameters.AddWithValue("@UserId", userId);

            SqlDataReader readerUser = cmdUser.ExecuteReader();

            string userName = string.Empty;
            string userEmail = string.Empty;
            string userAddress = string.Empty;
            string orderId = string.Empty;
            DateOnly orderDate = DateOnly.MinValue; // Initialize as a default value

            if (readerUser.Read())
            {
                userName = readerUser.GetString(readerUser.GetOrdinal("UserName"));
                userEmail = readerUser.GetString(readerUser.GetOrdinal("Email"));
                userAddress = readerUser.GetString(readerUser.GetOrdinal("ShippingAddress"));
                orderId = readerUser.GetInt32(readerUser.GetOrdinal("OrderId")).ToString();

                // Converting DateTime to DateOnly
                DateTime orderDateTime = readerUser.GetDateTime(readerUser.GetOrdinal("OrderDate"));
                orderDate = DateOnly.FromDateTime(orderDateTime);
            }

            readerUser.Close(); // Close the reader for user details.

            // Get Order Items with an inline SQL query
            SqlCommand cmdOrderItems = new SqlCommand(@"
                SELECT p.ProductId, p.ImageUrl, p.Name, p.Price, oi.Quantity, oi.OrderItemId, oi.OrderId
                FROM OrderItems oi
                JOIN Product p ON p.ProductId = oi.ProductId
                WHERE oi.UserId = @UserId
                AND oi.OrderId = @OrderId
                ORDER BY oi.OrderItemId DESC", conn);

            cmdOrderItems.Parameters.AddWithValue("@UserId", userId);
            cmdOrderItems.Parameters.AddWithValue("@OrderId", orderId);

            SqlDataReader readerOrderItems = cmdOrderItems.ExecuteReader();

            // Start constructing HTML for the invoice with inline styles
            string htmlData = "";

            htmlData += "<div style='margin-top: 20px; text-align: center;'>";

            // Centered logo
            htmlData += "<img src='https://th.bing.com/th/id/OIP.-twB0h2FBnMiQlUFUeLKXAHaHa?rs=1&pid=ImgDetMain' alt='Company Logo' style='width: 10%; height: auto; object-fit: cover; margin-bottom: 10px;' />"; // Replace with your logo URL

            // Order ID and Order Date below the logo, centered
            htmlData += "<div>";
            htmlData += $"<h3 style='margin: 0;'>Order ID: {orderId}</h3>"; // Dynamic Order ID
            htmlData += $"<h5 style='margin: 0;'>Order Date: {orderDate.ToString("dd MMM yyyy")}</h5>"; // Displaying DateOnly
            htmlData += "</div>";

            htmlData += "</div>"; // Closing the centered container

            htmlData += "<hr/>"; // Closing user details

            htmlData += $"<p><strong>User Name:</strong> {userName}</p>"; // User name
            htmlData += $"<p><strong>Email:</strong> {userEmail}</p>"; // User email
            htmlData += $"<p><strong>Shipping Address:</strong> {userAddress}</p>"; // Shipping address

            // Shopping Cart Title
            htmlData += "<hr/>";
            htmlData += "<h3>Product</h3>";

            // Product Table Header
            htmlData += "<table style='width: 100%; border-collapse: collapse;'>";
            htmlData += "<thead>";
            htmlData += "<tr>";
            htmlData += "<th style='padding: 10px; text-align: center; border: 1px solid #ddd; background-color: #f4f4f4;'>S.No</th>"; // Serial Number column
            htmlData += "<th style='padding: 10px; text-align: center; border: 1px solid #ddd; background-color: #f4f4f4;'>Name</th>";
            htmlData += "<th style='padding: 10px; text-align: center; border: 1px solid #ddd; background-color: #f4f4f4;'>Price</th>";
            htmlData += "<th style='padding: 10px; text-align: center; border: 1px solid #ddd; background-color: #f4f4f4;'>Quantity</th>";
            htmlData += "<th style='padding: 10px; text-align: center; border: 1px solid #ddd; background-color: #f4f4f4;'>Total Price</th>";
            htmlData += "</tr>";
            htmlData += "</thead>";
            htmlData += "<tbody>";

            decimal masterTotal = 0; // Total price calculation
            int serialNo = 1; // Serial number counter

            // Reading data for each product in the cart
            while (readerOrderItems.Read())
            {
                ShowCartDto showCartDto = new ShowCartDto
                {
                    ProductId = readerOrderItems.GetInt32(readerOrderItems.GetOrdinal("ProductId")),
                    Name = readerOrderItems.GetString(readerOrderItems.GetOrdinal("Name")),
                    Price = readerOrderItems.GetDecimal(readerOrderItems.GetOrdinal("Price")),
                    Stock = readerOrderItems.GetInt32(readerOrderItems.GetOrdinal("Quantity"))
                };

                decimal totalPrice = showCartDto.Price * showCartDto.Stock;
                masterTotal += totalPrice;

                // Constructing rows for the products in the table with Serial Number
                htmlData += "<tr>";
                htmlData += $"<td style='padding: 10px; text-align: center; border: 1px solid #ddd;'>{serialNo}</td>"; // Serial Number
                htmlData += $"<td style='padding: 10px; text-align: center; border: 1px solid #ddd;'>{showCartDto.Name}</td>";
                htmlData += $"<td style='padding: 10px; text-align: center; border: 1px solid #ddd;'>₹{showCartDto.Price}</td>";
                htmlData += $"<td style='padding: 10px; text-align: center; border: 1px solid #ddd;'>{showCartDto.Stock}</td>";
                htmlData += $"<td style='padding: 10px; text-align: center; border: 1px solid #ddd;'>₹{totalPrice}</td>";
                htmlData += "</tr>";

                serialNo++; // Increment Serial Number
            }

            // Add a row for the total amount
            htmlData += "<tr>";
            htmlData += $"<td colspan='4' style='padding: 10px; text-align: right; border: 1px solid #ddd; font-weight: bold;'>Total Amount</td>"; // Colspan to span 4 columns
            htmlData += $"<td style='padding: 10px; text-align: center; border: 1px solid #ddd;'>₹{masterTotal}</td>"; // Total Price Column
            htmlData += "</tr>";

            htmlData += "</tbody></table>"; // Closing product table

            // Adding thank you message
            htmlData += "<div style='margin-top: 30px; font-size: 16px; font-weight: bold; text-align: center;'>Thank you for your purchase! </div>";
            htmlData += "<div style='font-size: 16px; font-weight: bold; text-align: center;'>Visit Again!</div>";
            //htmlData += "<div style='font-size: 16px; font-weight: bold; text-align: center;'>ગમેતો બીજા ને કેજો ના ગમે તો એમને કહેજો કહે</div>";
            
            htmlData += "</div>"; // Closing container

            conn.Close();

            // Generate the PDF from the HTML data
            var document = new PdfDocument();
            PdfGenerator.AddPdfPages(document, htmlData, PageSize.A4);

            byte[] res;
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms);
                res = ms.ToArray();
            }

            string filename = $"cart_{userId}.pdf";
            _emailSender.SendMailWithAttachment(userEmail, "Your Order PDF", "Please find the attached PDF with the details of your Order.", res,"OrderInvoice.pdf");

            return File(res, "application/pdf", filename);
        }





    }
}
