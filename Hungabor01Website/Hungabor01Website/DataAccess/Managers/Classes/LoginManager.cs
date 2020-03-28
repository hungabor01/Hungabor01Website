using Hungabor01Website.DataAccess.Managers.Interfaces;
using System;

namespace Hungabor01Website.DataAccess.Managers.Classes
{
    public class LoginManager : RegistrationManager, ILoginManager
    {
        public LoginManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
