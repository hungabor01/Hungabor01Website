using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Hungabor01Website.Database.Core;
using Hungabor01Website.BusinessLogic.Enums;
using Hungabor01Website.Database.UnitOfWork;
using Hungabor01Website.DataAccess.Managers.Interfaces;
using Hungabor01Website.BusinessLogic.MessageSenders;

namespace Hungabor01Website.DataAccess.Managers.Classes
{
    public class RegistrationManager : IRegistrationManager
    {
        protected readonly IServiceProvider ServiceProvider;

        public RegistrationManager(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public async Task LogUserActionToDatabaseAsync(ApplicationUser user, UserActionType type, string description = null)
        {
            using (var unitOfWork = ServiceProvider.GetService<IUnitOfWork>())
            {
                await unitOfWork.AccountHistoryRepository.LogUserActionToDatabaseAsync(user, type, description);
                await unitOfWork.CompleteAsync();
            }
        }

        public async Task<bool> SendEmailAsync(string emailAddress, string subject, string emailBody)
        {         
            var emailSender = ServiceProvider.GetService<IMessageSender>();            
            return await emailSender.SendMessageAsync(emailAddress, subject, emailBody);
        }
    }
}
