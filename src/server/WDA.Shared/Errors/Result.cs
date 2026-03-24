namespace WDA.Shared.Errors;

public class Result
{
    public bool IsFailure { get; set; }
    public Error? Error { get; set; }

    protected Result(bool isFailure, Error? error)
    {
        IsFailure = isFailure;
        Error = error;
    }
}

public class Result<T> : Result
{
    protected Result(bool isFailure, Error? error) : base(isFailure, error)
    {
    }

    public static Success<T> Success(T data)
    {
        return Success<T>.Create(data);
    }
}