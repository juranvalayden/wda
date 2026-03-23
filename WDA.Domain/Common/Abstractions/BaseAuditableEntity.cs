namespace WDA.Domain.Common.Abstractions;

public abstract record BaseAuditableEntity<T> : BaseEntity<T>
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public string CreatedBy { get; set; } = "System";

    public DateTimeOffset LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}