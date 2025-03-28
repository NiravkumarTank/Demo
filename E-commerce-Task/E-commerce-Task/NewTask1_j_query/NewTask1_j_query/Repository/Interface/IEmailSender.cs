using System.Threading.Tasks;

namespace NewTask1_j_query.Repository.Interface
{
    public interface IEmailSender
    {
        Task<bool> EmailSenderAsync(string email, string subject, string message);
        void SendMailWithAttachment(string email, string subject, string body, byte[] pdfContent, string fileName);
    }
}
