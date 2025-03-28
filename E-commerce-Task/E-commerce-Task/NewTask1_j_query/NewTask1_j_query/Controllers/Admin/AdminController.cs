using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NewTask1_j_query.Models.Admin;
using NewTask1_j_query.Models.Dtos;

namespace NewTask1_j_query.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly string ConnetionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Task_E_Commerce;Integrated Security=True;Trust Server Certificate=True";

        [HttpPost]
        [Route("AddProduct")]
        public ActionResult<ProductAddDto> DataAdd([FromForm] ProductAddDto productDto)
        {
            SqlConnection conn = new SqlConnection(ConnetionString);

          

            if (productDto.Image != null)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var filePath = Path.Combine(path, $"{Guid.NewGuid()}_{Path.GetFileName(productDto.Image.FileName)}");
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    productDto.Image.CopyTo(fs);
                }
                productDto.ImageUrl = Path.GetFileName(filePath);
            }

            // Insert the product using the stored procedure
            SqlCommand cmd = new SqlCommand("Insert_product", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@name", productDto.Name);
            cmd.Parameters.AddWithValue("@description", productDto.Description);
            cmd.Parameters.AddWithValue("@catagory", productDto.c_id); 
            cmd.Parameters.AddWithValue("@subcatagory", productDto.sc_id);
            cmd.Parameters.AddWithValue("@price", productDto.Price);
            cmd.Parameters.AddWithValue("@stock", productDto.Stock);
            cmd.Parameters.AddWithValue("@imageurl", productDto.ImageUrl);

            conn.Open();
            int row = cmd.ExecuteNonQuery();
            conn.Close();

            if (row > 0)
            {
                return Ok(new { message ="inserted data ",data= productDto });
            }
            else
            {
                return BadRequest("Failed to insert product into the database.");
            }
        }




        [HttpGet]
        [Route("ShowAllProduct")]
        public ActionResult<List<ShowProductDto>> GetAllProducts()
        {
            SqlConnection conn = new SqlConnection(ConnetionString);
            List<ShowProductDto> products = new List<ShowProductDto>();

            SqlCommand cmd = new SqlCommand("Select_All", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ShowProductDto product = new ShowProductDto
                {
                    ProductId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    Category = reader.GetString(3), 
                    Subcategory = reader.GetString(4),
                    Price = reader.GetDecimal(5),
                    Stock = reader.GetInt32(6),
                    ImageUrl = reader.GetString(7)
                };

                products.Add(product);
            }
            var data = products.Select(item => new
            {
                item.ProductId,
                item.Name,
                item.Description,
                item.Category,
                item.Subcategory,
                item.Price,
                item.Stock,
                ImageUrl = $"{Request.Scheme}:/{Request.Host}/Upload/{item.ImageUrl}"
                
            });

           conn.Close();

            return Ok(data); 
        }



       [HttpGet]
        [Route("GetById/{id}")]
        public ActionResult<ShowProductDto> GetById(int id)
        {
            SqlConnection conn = new SqlConnection(ConnetionString);
            List<ShowProductDto> products = new List<ShowProductDto>();

            SqlCommand cmd = new SqlCommand("SelectById", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                ShowProductDto product = new ShowProductDto
                {
                    ProductId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    Category = reader.GetString(3),
                    Subcategory = reader.GetString(4),
                    Price = reader.GetDecimal(5),
                    Stock = reader.GetInt32(6),
                    ImageUrl = reader.GetString(7)
                };

                var data = new
                {
                    product.ProductId,
                    product.Name,
                    product.Description,
                    product.Category,
                    product.Subcategory,
                    product.Price,
                    product.Stock,
                    ImageUrl = $"{Request.Scheme}://{Request.Host}/Upload/{product.ImageUrl}"
                };

                conn.Close();
                return Ok(data);
            }
            else
            {
                conn.Close();
                return NotFound(new { message = id + " not found" });
            }
        }









        [HttpGet]
        [Route("GetBySubCategory/{c_id}")]
        public ActionResult<List<Subcategory>> GetBySubCategory(int c_id)
        {
            SqlConnection conn = new SqlConnection(ConnetionString);
            string query = "SELECT sc_id, sc_Name FROM Subcategory WHERE c_id = @categoryid";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@categoryid", c_id);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<Subcategory> subcategories = new List<Subcategory>();

            while (reader.Read())
            {
                Subcategory subcategory = new Subcategory()
                {
                    sc_id = reader.GetInt32(0),
                    sc_Name = reader.GetString(1)
                };
                subcategories.Add(subcategory);
            }
            conn.Close();

            if (subcategories.Count > 0)
                return Ok(subcategories);
            else
                return Ok(new { message = "No Subcategory found for this category." });
        }


        [HttpGet]
        [Route("SubCategoryName/{sc_name}")]
        public ActionResult<IEnumerable<ShowProductDto>> GetBySubCategory1(string sc_name)
        {
            SqlConnection conn = new SqlConnection(ConnetionString);
            List<ShowProductDto> products = new List<ShowProductDto>();

            SqlCommand cmd = new SqlCommand("SelectByCategory", conn);
            cmd.Parameters.AddWithValue("@categoryName", sc_name);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ShowProductDto product = new ShowProductDto
                    {
                        ProductId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        Category = reader.GetString(3),
                        Subcategory = reader.GetString(4),
                        Price = reader.GetDecimal(5),
                        Stock = reader.GetInt32(6),
                        ImageUrl = reader.GetString(7)
                    };

                    

                    products.Add(product);
                }
                var data = products.Select(item => new
                {
                    item.ProductId,
                    item.Name,
                    item.Description,
                    item.Category,
                    item.Subcategory,
                    item.Price,
                    item.Stock,
                    ImageUrl = $"{Request.Scheme}://{Request.Host}/Upload/{item.ImageUrl}"
                });

                if (products.Any())
                {
                    return Ok(data);  // Return all products after processing all rows
                }
                else
                {
                    return NotFound(new { message = $"{sc_name} not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching data.", error = ex.Message });
            }
            finally
            {
                conn.Close();  // Close the connection in the finally block to ensure it's closed even if an error occurs
            }
        }




        [HttpDelete]
        [Route("DeleteById/{id}")]
        public ActionResult DeleteById(int id)
        {
            SqlConnection conn = new SqlConnection(ConnetionString);
            string query = "Select ImageUrl from Product where ProductId = @id";

            SqlCommand cmd1 = new SqlCommand(query, conn);
            cmd1.Parameters.AddWithValue("@id", id);
            conn.Open();
            string image = Convert.ToString(cmd1.ExecuteScalar());
            
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", image);

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            string query1 = "delete from Product Where ProductId = @id";
            SqlCommand command1 = new SqlCommand(query1, conn);

            command1.Parameters.AddWithValue("@id", id);

            command1.ExecuteNonQuery();
            conn.Close();
            return Ok(new { message ="Delete data Product" });
        }




        [HttpPut]
        [Route("updateProduct")]
        public async Task<ActionResult<ProductUpdateDto>> UpdateDataWithImage([FromForm] ProductUpdateDto product)
        {
            using (SqlConnection conn = new SqlConnection(ConnetionString))
            {
                await conn.OpenAsync();

                // Step 1: Retrieve the current image address for the product
                string query = "SELECT ImageUrl FROM Product WHERE ProductId = @id";
                SqlCommand cmd1 = new SqlCommand(query, conn);
                cmd1.Parameters.AddWithValue("@id", product.ProductId);

                string selectImage = Convert.ToString(await cmd1.ExecuteScalarAsync());

                if (selectImage == null)
                {
                    return NotFound(new { message = $"Product with ID {product.ProductId} not found." });
                }

                // Step 2: Handle image upload if a new image is provided
                if (product.Image != null)
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", selectImage);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload");
                    var newImagePath = Path.Combine(path, $"{Guid.NewGuid()}_{Path.GetFileName(product.Image.FileName)}");

                    // Save the new image
                    using (var fs = new FileStream(newImagePath, FileMode.Create))
                    {
                        await product.Image.CopyToAsync(fs);
                    }

                    // Set the new image URL in the product object
                    product.ImageUrl = Path.GetFileName(newImagePath);
                }
                else
                {
                    // If no new image is provided, keep the existing image
                    product.ImageUrl = selectImage;
                }

                // Step 3: Update product details in the database
                string query1 = @"UPDATE Product SET 
                            Name = @name, 
                            Description = @description,
                            Price = @price, 
                            Stock = @stock, 
                            ImageUrl = @address 
                            WHERE ProductId = @id";

                SqlCommand command = new SqlCommand(query1, conn);
                command.Parameters.AddWithValue("@id", product.ProductId);
                command.Parameters.AddWithValue("@name", product.Name);
                command.Parameters.AddWithValue("@description", product.Description);
                command.Parameters.AddWithValue("@price", product.Price);
                command.Parameters.AddWithValue("@stock", product.Stock);
                command.Parameters.AddWithValue("@address", product.ImageUrl); 

                await command.ExecuteNonQueryAsync();

                // Return the updated product as the response
                return Ok(product);
            }
        }



    }
}
