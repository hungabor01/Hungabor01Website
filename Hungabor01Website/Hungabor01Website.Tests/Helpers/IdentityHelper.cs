using Hungabor01Website.Database.Core;

namespace Hungabor01Website.Tests.Helpers
{
    public class IdentityHelper
    {
        public ApplicationUser TestUser { get; }

        public IdentityHelper(string userId)
        {
            TestUser = new ApplicationUser
            {
                Id = userId,
            };
        }

        public void AddTestUser(AppDbContext context)
        {
            var testUserInDb = context.Users.Find(TestUser.Id);
            if (testUserInDb == null)
            {
                context.Users.Add(TestUser);
                context.SaveChanges();
            }
        }

        public void RemoveTestUser(AppDbContext context)
        {
            var testUserInDb = context.Users.Find(TestUser.Id);
            if (testUserInDb != null)
            {
                context.Users.Remove(TestUser);
                context.SaveChanges();
            }
        }
    }
}
