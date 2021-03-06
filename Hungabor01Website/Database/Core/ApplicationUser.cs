﻿using Database.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Database.Core
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Attachment> Attachments { get; set; }    
        public virtual ICollection<AccountHistory> AccountHistories { get; set; }

        public ApplicationUser()
        {
            Attachments = new HashSet<Attachment>();
            AccountHistories= new HashSet<AccountHistory>();
        }
    }
}
