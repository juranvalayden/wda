namespace WDA.Shared.Errors;

public class Failure : Result
{
    private Failure(Error error) : base(isFailure: true, error: error)
    {
        Error = error;
    }

    public static Failure Create(Error error)
    {
        return new Failure(error);
    }
}