namespace WDA.Shared.Errors;

public record Error(ErrorType ErrorType, string Description)
{
    public static readonly Error None = new(ErrorType.None, string.Empty);
    public static Error NullValue = new(ErrorType.Null, "Null value was provided.");
    
    public static implicit operator Result(Error error) => Failure.Create(error);
    public Result ToResult() => Failure.Create(this);
}