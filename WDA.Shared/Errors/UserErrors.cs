namespace WDA.Shared.Errors;

public static class UserErrors
{
    public static Error NotFound(Guid id)
    {
        return new Error("Users.NotFound",$"The user with Id:'{id}' was not found.");
    }

    public static Error NotFound(string email)
    {
        return new Error("Users.NotFound", $"The user with email'{email}' was not found.");
    }

    public static Error AlreadyExists(string email)
    {
        return new Error("Users.AlreadyExists", $"The user with email'{email}' already exists.");
    }

    public static Error ErrorSaving()
    {
        return new Error("Users.ErrorSaving", "Error Saving the user.");
    }
}