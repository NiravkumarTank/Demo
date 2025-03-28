using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NewTask1_j_query.Models.Admin;

namespace NewTask1_j_query.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly string ConnetionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Task_E_Commerce;Integrated Security=True;Trust Server Certificate=True";

        [HttpPost]
        [Route("SubCategoryAdd")]
        public ActionResult<Subcategory> AddCategory([FromForm] Subcategory subcategory)
        {
            SqlConnection conn = new SqlConnection(ConnetionString);
            string query = "INSERT INTO SubCategory (sc_Name,c_id) VALUES (@name,@c_id)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", subcategory.sc_Name);
            cmd.Parameters.AddWithValue("@c_id", subcategory.c_id);
            conn.Open();
            int row = cmd.ExecuteNonQuery();
            conn.Close();

            if (row > 0)
            {
                return Ok(new { message = "Category Inserted.", data = subcategory });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("SubCategoryShow")]
        public ActionResult<List<Subcategory>> ShowAllSubCategory()
        {
            SqlConnection conn = new SqlConnection(ConnetionString);
            string query = "Select * from Subcategory ";
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            cmd.ExecuteNonQuery();

            List<Subcategory> Subcategorys = new List<Subcategory>();
           
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Subcategory category = new Subcategory()
                {
                    sc_id = reader.GetInt32(0),
                    sc_Name = reader.GetString(1),
                    c_id = reader.GetInt32(2),
                };
                Subcategorys.Add(category);
            }
            conn.Close();

           return Ok(Subcategorys);
        }


        [HttpGet]
        [Route("CategoryShow")]
        public ActionResult<List<Category>> ShowAllCategory()
        {
            
            SqlConnection conn = new SqlConnection(ConnetionString);
            string query = "Select* from Category ";
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            cmd.ExecuteNonQuery();

            List<Category> categories = new List<Category>();

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Category category = new Category()
                {
                    c_Id = reader.GetInt32(0),
                    c_Name = reader.GetString(1),
                  
                };
                categories.Add(category);
            }
            conn.Close();

            return Ok(categories);
        }
    }
}
