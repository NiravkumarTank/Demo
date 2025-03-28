using NewTask1_j_query.Models.Email;
using System.Net.Mail;
using System.Net;
using NewTask1_j_query.Repository.Interface;

namespace NewTask1_j_query.Repository.Service
{
    public class EmailSenders : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSenders(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public async Task<bool> EmailSenderAsync(string email, string subject, string message)
        {

            bool status = false;
            try
            {
                GetEmailSetting getEmailSetting = new GetEmailSetting()
                {
                    SecretKey = _configuration.GetValue<string>("AppSettings:SecretKey"),
                    From = _configuration.GetValue<string>("AppSettings:EmailSettings:From"),
                    SmtpServer = _configuration.GetValue<string>("AppSettings:EmailSettings:SmtpServer"),
                    Port = _configuration.GetValue<int>("AppSettings:EmailSettings:Port"),
                    EnableSSL = _configuration.GetValue<bool>("AppSettings:EmailSettings:EnableSSL"),
                };

                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(getEmailSetting.From),
                    Subject = subject,
                    Body = message
                };

                mailMessage.To.Add(email);
                SmtpClient smtpClient = new SmtpClient(getEmailSetting.SmtpServer)
                {
                    Port = getEmailSetting.Port,
                    Credentials = new NetworkCredential(getEmailSetting.From, getEmailSetting.SecretKey),
                    EnableSsl = getEmailSetting.EnableSSL
                };

                await smtpClient.SendMailAsync(mailMessage);
                status = true;

            }
            catch (Exception ex)
            {
                status = false;
                // Log exception or return a more detailed error message
                Console.WriteLine($"Error occurred: {ex.Message}");
            }

            return status;
        }

        public void SendMailWithAttachment(string email, string subject, string body, byte[] pdfContent, string fileName)
        {
            bool status = false;
            try
            {
                // Retrieve email settings from the configuration
                string fromMail = _configuration.GetValue<string>("AppSettings:EmailSettings:From");
                string smtpHost = _configuration.GetValue<string>("AppSettings:EmailSettings:SmtpServer");
                int smtpPort = _configuration.GetValue<int>("AppSettings:EmailSettings:Port");
                string smtpPassword = _configuration.GetValue<string>("AppSettings:SecretKey");

                // Create a new MailMessage object
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromMail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.To.Add(email);
                    mail.IsBodyHtml = true;

                    // Attach the PDF to the email
                    using (MemoryStream ms = new MemoryStream(pdfContent))
                    {
                        Attachment attachment = new Attachment(ms, fileName, "application/pdf");
                        mail.Attachments.Add(attachment);

                        // Send the email asynchronously using SmtpClient
                        using (SmtpClient smtp = new SmtpClient(smtpHost, smtpPort))
                        {
                            smtp.Credentials = new NetworkCredential(fromMail, smtpPassword);
                            smtp.EnableSsl = true;

                            smtp.Send(mail);
                        }
                    }
                }

                status = true; // Email sent successfully
            }
            catch (Exception ex)
            {
                status = false;
                // Log the error, or throw a more specific exception if necessary
                Console.WriteLine($"Error occurred: {ex.Message}");
            }

            //return status;
        }




    }
}
