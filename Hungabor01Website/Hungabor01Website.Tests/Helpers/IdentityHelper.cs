using Hungabor01Website.Database;
using Hungabor01Website.Database.Entities;

namespace Hungabor01Website.Tests.Helpers
{
  public class IdentityHelper
  {
    public ApplicationUser TestUser { get; }
    
    public IdentityHelper(string userId)
    {
      TestUser = CreateTestUser(userId);
    }

    private ApplicationUser CreateTestUser(string id)
    {
      return new ApplicationUser
      {
        Id = id,
      };
    }

    public void AddTestUser(AppDbContext context)
    {
      var testUser = context.Users.Find(TestUser.Id);
      if (testUser == null)
      {
        context.Users.Add(TestUser);
        context.SaveChanges();
      }
    }

    public void RemoveTestUser(AppDbContext context)
    {
      var testUser = context.Users.Find(TestUser.Id);
      if (testUser != null)
      {
        context.Users.Remove(TestUser);
        context.SaveChanges();
      }
    }
  }
}
