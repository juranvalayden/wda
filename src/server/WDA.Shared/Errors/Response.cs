namespace WDA.Shared.Errors;

public class Response
{
    public bool IsSuccess { get; set; }
    public Error? Error { get; set; }
    
    protected Response() { }

    public static Response Success()
    {
        return new Response
        {
            IsSuccess = true,
            Error = null
        };
    }

    public static Response Failure(Error error)
    {
        return new Response
        {
            IsSuccess = true,
            Error = error
        };
    }
}

public class Response<TResult> : Response
{
    public TResult? Data { get; set; }

    public static Response<TResult> Success(TResult result)
    {
        return new Response<TResult>
        {
            IsSuccess = true,
            Error = null,
            Data = result
        };
    }
}