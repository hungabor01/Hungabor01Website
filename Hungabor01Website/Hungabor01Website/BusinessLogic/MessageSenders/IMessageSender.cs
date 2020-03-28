using System.Threading.Tasks;

namespace Hungabor01Website.BusinessLogic.MessageSenders
{
    public interface IMessageSender
    {
        public Task<bool> SendMessageAsync(string emailAddress, string subject, string message);
    }
}
