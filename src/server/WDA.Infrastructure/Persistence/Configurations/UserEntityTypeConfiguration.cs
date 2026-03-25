using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WDA.Domain.Entities;
using WDA.Shared;

namespace WDA.Infrastructure.Persistence.Configurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

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

        builder.HasIndex(u => u.Email)
            .IsUnique();

        BaseAuditableEntityConfiguration<User>.ConfigureBaseAuditableEntity(builder);
    }
}