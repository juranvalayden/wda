namespace WDA.Shared.Errors;

public class Success<T> : Result<T>
{
    public T Data { get; }

    private Success(bool isFailure, Error? error, T data) : base(isFailure, error)
    {
        Data = data;
    }

    public static Success<T> Create(T data)
    {
        return new Success<T>(false, null, data);
    }
}