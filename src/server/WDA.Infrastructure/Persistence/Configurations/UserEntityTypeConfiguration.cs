using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WDA.Domain.Entities;
using WDA.Shared;

namespace WDA.Infrastructure.Persistence.Configurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "wda");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("Id")
            .HasConversion<int>()
            .ValueGeneratedOnAdd();

        builder.Property(u => u.FirstName)
            .HasConversion<string>()
            .HasMaxLength(Constants.FirstNameMaxTextLength)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasConversion<string>()
            .HasMaxLength(Constants.LastNameMaxTextLength)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasConversion<string>()
            .HasMaxLength(Constants.EmailMaxTextLength)
            .IsRequired();

        builder.Property(u => u.Password)
            .HasConversion<string>()
            .HasMaxLength(Constants.PasswordMaxTextLength)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .HasConversion<DateTimeOffset>()
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .HasColumnType("timestamptz")
            .HasConversion(u => u,
                u => DateTime.SpecifyKind(u, DateTimeKind.Utc));

        builder.Property(u => u.CreatedBy)
            .HasConversion<string>()
            .HasMaxLength(Constants.CreatedByMaxTextLength)
            .IsRequired();

        builder.Property(u => u.LastModified)
            .HasColumnType("timestamptz")
            .IsRequired(false)
            .HasConversion(
                u => u,
                u => DateTime.SpecifyKind(u ?? default, DateTimeKind.Utc));

        builder.Property(u => u.LastModifiedBy)
            .HasConversion<string>()
            .HasMaxLength(Constants.LastModifiedByMaxTextLength)
            .IsRequired(false);

        builder.HasIndex(u => u.Id);

        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}