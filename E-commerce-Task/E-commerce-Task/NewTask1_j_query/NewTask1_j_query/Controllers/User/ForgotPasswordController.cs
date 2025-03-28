using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NewTask1_j_query.Repository.Interface;

namespace NewTask1_j_query.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly string ConnetionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Task_E_Commerce;Integrated Security=True;Trust Server Certificate=True";
        private readonly IEmailSender _emailSender;

        public ForgotPasswordController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        // Temporary in-memory storage for OTP (use cache or database in production)
        private static Dictionary<string, (int otp, DateTime expirationTime)> otpStorage = new Dictionary<string, (int otp, DateTime expirationTime)>();

        // Store OTP with expiration time
        private void StoreOtpForUser(string email, int otp)
        {
            var expirationTime = DateTime.Now.AddMinutes(5); // OTP expires in 5 minutes
            otpStorage[email] = (otp, expirationTime);
        }

        // Get the OTP for the user (returns 0 if expired or not found)
        private (int otp, DateTime expirationTime) GetOtpForUser(string email)
        {
            if (otpStorage.ContainsKey(email))
            {
                var otpData = otpStorage[email];
                if (otpData.expirationTime > DateTime.Now)
                {
                    return otpData;
                }
            }
            return (0, DateTime.MinValue);
        }





        [HttpPost]
        [Route("Email")]
        public ActionResult ForgotPassword([FromForm] string email)
        {
            using (SqlConnection conn = new SqlConnection(ConnetionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT UserId, email FROM UserModel WHERE email = @email", conn);
                cmd.Parameters.AddWithValue("@email", email);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int userId = (int)reader["UserId"]; // Get the UserId
                    Random random = new Random();
                    int otp = random.Next(100000, 1000000); // Generate OTP

                    // Store OTP and its expiration time (e.g., 5 minutes)
                    StoreOtpForUser(email, otp);

                    // Send OTP to user via email (uncomment email sender in production)
                    _emailSender.EmailSenderAsync(email, "OTP for Password Reset", "Your OTP for password reset is: " + otp);

                    // Return OTP and UserId in the response
                    return Ok(new { message = "OTP sent successfully", otp = otp, userId = userId });
                }
                else
                {
                    // If email is not found, return a custom message
                    return Ok(new { message = "No such email registered. Please register first." });
                }
            }
        }



        // Assuming you already have these methods in place

        // Step 1: Verify OTP
        [HttpPost]
        [Route("Otp")]

        public ActionResult VerifyOtp([FromForm] int userId, [FromForm] int enteredOtp)
        {
            // Fetch user email using userId
            SqlConnection conn1 = new SqlConnection(ConnetionString);
            SqlCommand comd1 = new SqlCommand("SELECT email FROM UserModel WHERE UserId = @id", conn1);
            comd1.Parameters.AddWithValue("@id", userId);
            conn1.Open();

            var dataemail = comd1.ExecuteScalar() as string;

            if (string.IsNullOrEmpty(dataemail))
            {
                return BadRequest(new { message = "User not found" });
            }

            // Retrieve OTP data for user
            var otpData = GetOtpForUser(dataemail);

            if (otpData.otp == enteredOtp && otpData.expirationTime > DateTime.Now)
            {
                // OTP is valid, so store that the OTP is verified
                return Ok(new { message = "OTP verified successfully. Please proceed with resetting your password." });
            }
            else
            {
                // OTP expired or invalid, send a new OTP
                Random random = new Random();
                int newOtp = random.Next(100000, 1000000);

                // Store the new OTP for the user
                StoreOtpForUser(dataemail, newOtp);

                // Send the new OTP via email
                _emailSender.EmailSenderAsync(dataemail, "New OTP for Password Reset", "Your new OTP for password reset is: " + newOtp);

                return Ok(new { message = "OTP expired or invalid, a new OTP has been sent" });
            }
        }

        // Step 2: Reset Password
        [HttpPost]
        [Route("Password")]
        public ActionResult ResetPassword([FromForm]int userId,[FromForm] string newPassword)
        {
            // Fetch user email using userId
            SqlConnection conn1 = new SqlConnection(ConnetionString);
            SqlCommand comd1 = new SqlCommand("SELECT email FROM UserModel WHERE UserId = @id", conn1);
            comd1.Parameters.AddWithValue("@id", userId);
            conn1.Open();

            var dataemail = comd1.ExecuteScalar() as string;

            if (string.IsNullOrEmpty(dataemail))
            {
                return BadRequest(new { message = "User not found" });
            }

            // Ensure the OTP has been verified before allowing password reset
            var otpData = GetOtpForUser(dataemail);
            if (otpData.otp == 0 || otpData.expirationTime <= DateTime.Now)
            {
                return BadRequest(new { message = "OTP is not valid or has expired. Please request a new OTP." });
            }

            // Generate salt for password hashing
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash new password using PBKDF2
            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: newPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));

            // Update the user's password in the database using their email
            using (SqlConnection conn = new SqlConnection(ConnetionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE UserModel SET password = @newPassword, solt = @salt WHERE email = @email", conn);
                cmd.Parameters.AddWithValue("@newPassword", hash);
                cmd.Parameters.AddWithValue("@email", dataemail);
                cmd.Parameters.AddWithValue("@salt", Convert.ToBase64String(salt));

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Password updated successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to update password" });
                }
            }
        }


    }
}
