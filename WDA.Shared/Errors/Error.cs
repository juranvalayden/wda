namespace WDA.Shared.Errors;

public record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static Error NullValue = new("Error.NullValue", "Null value was provided.");
    public static implicit operator Result(Error error) => Result.Failure(error);
    public Result ToResult() => Result.Failure(this);
}