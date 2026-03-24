namespace WDA.Domain.Common.Abstractions;

public abstract record BaseAuditableEntity<T> : BaseEntity<T>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string CreatedBy { get; set; } = "System";

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}