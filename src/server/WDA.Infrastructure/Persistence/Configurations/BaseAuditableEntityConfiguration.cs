using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WDA.Domain.Common.Abstractions;
using WDA.Shared;

namespace WDA.Infrastructure.Persistence.Configurations;

public static class BaseAuditableEntityConfiguration<T> where T : BaseAuditableEntity
{
    public static void ConfigureBaseAuditableEntity(EntityTypeBuilder<T> builder) 
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("Id")
            .HasConversion<int>()
            .ValueGeneratedOnAdd();

        builder.Property(u => u.CreatedAt)
            .HasColumnType("timestamptz")
            .HasConversion(u => u, u => DateTime.SpecifyKind(u, DateTimeKind.Utc));

        builder.Property(u => u.CreatedBy)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(Constants.CreatedByMaxTextLength);

        builder.Property(u => u.LastModifiedAt)
            .HasColumnType("timestamptz")
            .HasConversion(u => u, u => DateTime.SpecifyKind(u, DateTimeKind.Utc));

        builder.Property(u => u.LastModifiedBy)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(Constants.LastModifiedByMaxTextLength);

        builder.HasIndex(u => u.Id);
    }
}