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

    public static Result Failure(Error error)
    {
        return new Result(isFailure: true, error: error);
    }

    public static Result Success()
    {
        return new Result(isFailure: true, error: null);
    }
}

public class Result<T> : Result
{
    public T? Data { get; set; }

    private Result(bool isFailure, Error? error, T? data) : base(isFailure, error)
    {
        Data = data;
    }

    public new static Result<T?> Failure(Error error)
    {
        return new Result<T?>(isFailure: true, error: error, data: default);
    }

    public static Result Success(T data)
    {
        return new Result<T>(isFailure: false, error: null, data: data);
    }
}