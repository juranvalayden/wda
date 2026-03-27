namespace WDA.Domain.Common.Abstractions;

public abstract record BaseEntity : IEntity
{
    public virtual int Id { get; set; }
}