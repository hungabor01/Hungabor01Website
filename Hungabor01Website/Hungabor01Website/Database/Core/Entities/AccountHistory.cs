using System;

namespace Hungabor01Website.Database.Core.Entities
{
    public class AccountHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; }  
        public ApplicationUser User { get; set; }
        public DateTime DateTime { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
