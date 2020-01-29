using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Hungabor01Website.Database.Entities
{
  /// <summary>
  /// Extended IdentityUser class
  /// </summary>
  public class ApplicationUser : IdentityUser
  {
    /// <summary>
    /// The attachments of the user
    /// </summary>
    public virtual ICollection<Attachment> Attachments { get; set; }
    
    /// <summary>
    /// The account history of the user
    /// </summary>
    public virtual ICollection<AccountHistory> AccountHistories { get; set; }

    public ApplicationUser()
    {
      Attachments = new HashSet<Attachment>();
      AccountHistories= new HashSet<AccountHistory>();
    }
  }
}
