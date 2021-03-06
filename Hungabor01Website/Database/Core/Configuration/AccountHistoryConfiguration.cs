﻿using Database.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Core.Configuration
{
    public class AccountHistoryConfiguration : IEntityTypeConfiguration<AccountHistory>
    {
        public void Configure(EntityTypeBuilder<AccountHistory> builder)
        {
            builder.HasOne(ah => ah.User)
                .WithMany(user => user.AccountHistories)
                .HasForeignKey(ah => ah.UserId);      
        }
    }
}
