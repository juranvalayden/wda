namespace WDA.Domain.Common.Abstractions;

public abstract record BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string CreatedBy { get; set; } = "System";

    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

    public string LastModifiedBy { get; set; } = "System";

    public void OnEntityCreated(string createdBy, DateTime createdAt)
    {
        CreatedBy = createdBy;
        CreatedAt = createdAt;

        OnEntityUpdated(createdBy, createdAt);
    }

    private void OnEntityUpdated(string lastModifiedBy, DateTime lastModifiedAt)
    {
        LastModifiedBy = lastModifiedBy;
        LastModifiedAt = lastModifiedAt;
    }
}