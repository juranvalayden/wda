namespace WDA.Domain.Common.Abstractions;

public abstract record BaseEntity<T>
{
    public T Id { get; set; }
}