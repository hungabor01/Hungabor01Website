using Hungabor01Website.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Hungabor01Website.Database.Configuration
{
  public class TestEntityConfiguration : IEntityTypeConfiguration<TestEntity>
  {
    public void Configure(EntityTypeBuilder<TestEntity> builder)
    {
      builder.ToTable("TestTable");

      builder.HasData
      (
        new TestEntity { Id= 1, Name = "Gábor"}
      );
    }
  }
}
