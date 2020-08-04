using System.Threading.Tasks;

namespace BusinessLogic.Services.Interfaces
{
    public interface IEmailSender
    {
        public Task<bool> SendEmailAsync(string emailAddress, string subject, string message);
    }
}
