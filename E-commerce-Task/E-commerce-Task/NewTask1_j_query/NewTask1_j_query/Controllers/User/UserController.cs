using System.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NewTask1_j_query.Models.Dtos;
using NewTask1_j_query.Models.User;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure.Core;
using NewTask1_j_query.Repository.Interface;


namespace NewTask1_j_query.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly string ConnetionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Task_E_Commerce;Integrated Security=True;Trust Server Certificate=True";


        private readonly IEmailSender _emailSender;

        public UserController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }


        [HttpGet]
        [Route("GetAllUser")]
        public ActionResult<List<UserModel>> GetUsers()
        {
            List<UserModel> users = new List<UserModel>();
            SqlConnection conn = new SqlConnection(ConnetionString);
            //string query = "select * from UserModel";
            SqlCommand cmd = new SqlCommand("SelectAllUserModel", conn);
            cmd.CommandType= CommandType.StoredProcedure;

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                UserModel user = new UserModel()
                {
                    UserId = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Email = reader.GetString(3),
                    PhoneNumber = reader.GetString(6),
                    Gender = reader.GetString(7),
                };
                users.Add(user);    
            }
            var data = users.Select(p => new {
                p.UserId,
                p.FirstName,
                p.LastName,
                p.Email,
                p.PhoneNumber,
                p.Gender
            });
            conn.Close();
            return Ok(data);
        }



        [HttpGet]
        [Route("GetByIdUser/{id}")]
        public ActionResult<List<UserModel>> GetById(int id)
        {
            SqlConnection conn = new SqlConnection(ConnetionString);
            SqlCommand cmd = new SqlCommand("SelectByIdUserModel", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserId", id);


            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                UserModel user = new UserModel()
                {
                    UserId = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Email = reader.GetString(3),
                    PhoneNumber = reader.GetString(6),
                    Gender = reader.GetString(7),
                };

                var data = new
                {
                    user.UserId,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber,
                    user.Gender
                };
                conn.Close();
                return Ok(data);
            }
            return BadRequest(new { message = "no data  " });
        }


        [HttpDelete]
        [Route("DeleteById/{id}")]
        public ActionResult<List<UserModel>> DeleteById(int id)
        {
            SqlConnection conn = new SqlConnection(ConnetionString);
            //string query = "Delete from UserModel Where UserId = @id";
            SqlCommand cmd = new SqlCommand("DeleteUserModel", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            int row =cmd.ExecuteNonQuery();
            conn.Close();

            if (row > 0)
            {
                   return Ok(new { message = id + " is Deleted" });
            }
            return NotFound(new { message = id + " is not Found" });
        }


        [HttpPut]
        [Route("UpdateUser/{id}")]
        public ActionResult<UserModel> UpdateData(int id, [FromBody] UpdateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SqlConnection conn = new SqlConnection(ConnetionString);

            SqlCommand cmd = new SqlCommand("UpdateUserModel", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@fname", userDto.FirstName);
            cmd.Parameters.AddWithValue("@lname", userDto.LastName);
            cmd.Parameters.AddWithValue("@email", userDto.Email);
            cmd.Parameters.AddWithValue("@phone", userDto.PhoneNumber);
            cmd.Parameters.AddWithValue("@gender", userDto.Gender ?? string.Empty);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            return Ok(new { message = "Data updated successfully", data = userDto });
        }




        [HttpPost]
        [Route("Login")]
        public ActionResult<Login> LoginData([FromForm] Login login)
        {
            SqlConnection conn = new SqlConnection(ConnetionString);

            SqlCommand cmd = new SqlCommand("CheckUserLogin", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@email", login.Email);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var datapassword = reader["Password"].ToString();
                byte[] saltdata = Convert.FromBase64String(reader["solt"].ToString());

                var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: login.Password,
                    salt: saltdata,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                ));



                int userId = Convert.ToInt32(reader["UserId"]);
                string firstName = Convert.ToString(reader["FirstName"]);
                string role = Convert.ToString(reader["Role"]);

                conn.Close();

                if (role == "Admin" && hash == datapassword)
                {
                    return Ok(new
                    {
                        message = "Login successful",
                        data = new
                        {
                            UserId = userId,
                            FirstName = firstName,
                            Role = "Admin"
                        }
                    });
                }
                else if (role == "User" && hash == datapassword)
                {
                    return Ok(new
                    {
                        message = "Login successful",
                        data = new
                        {
                            UserId = userId,
                            FirstName = firstName,
                            Role = "User"
                        }
                    });
                }
                else
                {
                    conn.Close();
                    return Ok(new { message = "Invalid Email or password" });
                }

            }
            else
            {
                conn.Close();
                return Ok(new { message = "Invalid Email or password" });
            }
        }




        //[HttpPost]
        //[Route("Registration")]
        //public ActionResult<UserModel> CreatUser([FromForm] UserModel user)
        //{
        //    SqlConnection conn = new SqlConnection(ConnetionString);

        //    string emailCheck = "SELECT COUNT(1) FROM UserModel WHERE Email = @email";
        //    SqlCommand checkEmailCmd = new SqlCommand(emailCheck, conn);
        //    checkEmailCmd.Parameters.AddWithValue("@email", user.Email);

        //    conn.Open();
        //    int emailExists = (int)checkEmailCmd.ExecuteScalar();
        //    conn.Close();

        //    if (emailExists > 0)
        //    {
        //        return Ok(new { message = "Email already exists" });
        //    }


        //    var salt = new byte[128 / 8];
        //    using (var rendon = RandomNumberGenerator.Create())
        //    {
        //        rendon.GetBytes(salt);
        //    }

        //    var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        //            password: user.Password,
        //            salt: salt,
        //            prf: KeyDerivationPrf.HMACSHA256,
        //            iterationCount: 10000,
        //            numBytesRequested: 256 / 8
        //        ));



        //    SqlCommand cmd = new SqlCommand("Insert_UserModel", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@fname", user.FirstName);
        //    cmd.Parameters.AddWithValue("@lname", user.LastName);
        //    cmd.Parameters.AddWithValue("@email", user.Email);
        //    cmd.Parameters.AddWithValue("@password", hash);
        //    cmd.Parameters.AddWithValue("@solt", Convert.ToBase64String(salt));
        //    cmd.Parameters.AddWithValue("@phonenumber", user.PhoneNumber);
        //    cmd.Parameters.AddWithValue("@gender", user.Gender);


        //    conn.Open();
        //    int row = cmd.ExecuteNonQuery();
        //    conn.Close();

        //    if (row > 0)
        //    {
        //        return Ok(new { data = user, message = "Registration successful" });
        //    }
        //    else
        //    {
        //        return BadRequest(new { message = "Error, no data inserted" });
        //    }

        //}

        [HttpPost]
        [Route("Registration")]
        public async Task<ActionResult<UserModel>> CreatUser([FromForm] UserModel user)
        {
            SqlConnection conn = new SqlConnection(ConnetionString);

            // Check if email already exists
            string emailCheck = "SELECT COUNT(1) FROM UserModel WHERE Email = @email";
            SqlCommand checkEmailCmd = new SqlCommand(emailCheck, conn);
            checkEmailCmd.Parameters.AddWithValue("@email", user.Email);

            conn.Open();
            int emailExists = (int)checkEmailCmd.ExecuteScalar();
            conn.Close();

            if (emailExists > 0)
            {
                return Ok(new { message = "Email already exists" });
            }

            // Store user details in session (including password temporarily)
            HttpContext.Session.SetString("FirstName", user.FirstName);
            HttpContext.Session.SetString("LastName", user.LastName);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("PhoneNumber", user.PhoneNumber);
            HttpContext.Session.SetString("Gender", user.Gender);
            HttpContext.Session.SetString("Password", user.Password);  

            // Generate OTP
            Random random = new Random();
            string otp = random.Next(100000, 999999).ToString(); // Generates a 6-digit OTP

            // Store OTP in session or temporary storage with an expiration time
            HttpContext.Session.SetString("Otp", otp); // Save OTP in session
            HttpContext.Session.SetString("OtpExpiration", DateTime.Now.AddMinutes(5).ToString()); // OTP expiration time

            // Send OTP to the user's email
            bool emailSent = await _emailSender.EmailSenderAsync(user.Email, "Your OTP for registration", $"Your OTP is: {otp}");

            if (emailSent)
            {
                return Ok(new { message = "OTP sent to email. Please verify to complete registration." ,otp = otp });
            }
            else
            {
                return BadRequest(new { message = "Failed to send OTP" });
            }
        }

        [HttpPost]
        [Route("VerifyOtp")]
        public async Task<ActionResult> VerifyOtp([FromForm] OtpVerificationRequest request)
        {
            // Get the OTP and expiration time from the session
            string storedOtp = HttpContext.Session.GetString("Otp");
            string expirationTimeString = HttpContext.Session.GetString("OtpExpiration");
            string storedEmail = HttpContext.Session.GetString("Email");
            string storedPassword = HttpContext.Session.GetString("Password");

            if (storedOtp == null || expirationTimeString == null)
            {
                return BadRequest(new { message = "OTP not found or expired. Please request a new OTP." });
            }

            DateTime otpExpirationTime = DateTime.Parse(expirationTimeString);

            // Check if OTP has expired
            if (otpExpirationTime < DateTime.Now)
            {
                return BadRequest(new { message = "OTP has expired" });
            }

            // Check if the email in the OTP verification request matches the stored email
            if (request.Email != storedEmail)
            {
                // If the email doesn't match, we treat it as a request for a new OTP
                // Generate a new OTP for the new email

                // Generate a new OTP
                Random random = new Random();
                string newOtp = random.Next(100000, 999999).ToString();

                // Store the new OTP and expiration time in the session
                HttpContext.Session.SetString("Otp", newOtp);
                HttpContext.Session.SetString("OtpExpiration", DateTime.Now.AddMinutes(5).ToString()); // 5 minute expiration

                // Update the session with the new email
                HttpContext.Session.SetString("Email", request.Email);

                // Send the new OTP to the new email
                bool emailSent = await _emailSender.EmailSenderAsync(request.Email, "Your OTP for registration", $"Your OTP is: {newOtp}");

                if (emailSent)
                {
                    return Ok(new { message = "OTP sent to new email. Please verify to complete registration." });
                }
                else
                {
                    return BadRequest(new { message = "Failed to send OTP to new email." });
                }
            }

            // If the email matches, proceed with OTP verification
            if (storedOtp == request.Otp)
            {
                // OTP is valid, proceed with user registration
                return CompleteRegistration(request.Email, storedPassword);
            }
            else
            {
                return BadRequest(new { message = "Invalid OTP" });
            }
        }


        private ActionResult CompleteRegistration(string email, string password)
        {
            SqlConnection conn = new SqlConnection(ConnetionString);

            // Retrieve user details from session (including password)
            string firstName = HttpContext.Session.GetString("FirstName");
            string lastName = HttpContext.Session.GetString("LastName");
            string phoneNumber = HttpContext.Session.GetString("PhoneNumber");
            string gender = HttpContext.Session.GetString("Gender");
            string sessionPassword = HttpContext.Session.GetString("Password"); // Retrieve password from session

            // Check if user already exists
            string getUserDataQuery = "SELECT * FROM UserModel WHERE Email = @Email";
            SqlCommand getUserDataCmd = new SqlCommand(getUserDataQuery, conn);
            getUserDataCmd.Parameters.AddWithValue("@Email", email);

            conn.Open();
            using (SqlDataReader reader = getUserDataCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    // User already exists, skip registration
                    conn.Close();
                    return Ok(new { message = "User already registered" });
                }
            }

            // Register user with hashed password and salt
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: sessionPassword, // Use the password from session
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));

            // Insert new user
            string insertQuery = "INSERT INTO UserModel (FirstName, LastName, Email, Password, Solt, PhoneNumber, Gender) " +
                                 "VALUES(@fname, @lname, @email, @password, @solt, @phonenumber, @gender)";
            SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@fname", firstName);
            insertCmd.Parameters.AddWithValue("@lname", lastName);
            insertCmd.Parameters.AddWithValue("@email", email);
            insertCmd.Parameters.AddWithValue("@password", hash);
            insertCmd.Parameters.AddWithValue("@solt", Convert.ToBase64String(salt));
            insertCmd.Parameters.AddWithValue("@phonenumber", phoneNumber);
            insertCmd.Parameters.AddWithValue("@gender", gender);

            int row = insertCmd.ExecuteNonQuery();
            conn.Close();

            if (row > 0)
            {
                // Clear session data after successful registration
                HttpContext.Session.Remove("FirstName");
                HttpContext.Session.Remove("LastName");
                HttpContext.Session.Remove("Email");
                HttpContext.Session.Remove("PhoneNumber");
                HttpContext.Session.Remove("Gender");
                HttpContext.Session.Remove("Password"); // Clear the password from session
                HttpContext.Session.Remove("Otp");
                HttpContext.Session.Remove("OtpExpiration");

                return Ok(new { message = "Registration successful" });
            }
            else
            {
                return BadRequest(new { message = "Error, user not registered" });
            }
        }

        public class OtpVerificationRequest
        {
            public string Email { get; set; }
            public string Otp { get; set; }
        }


    }
}
