namespace WDA.Shared.Errors;

public static class UserErrors
{
    public static Error NotFound(string email) =>
        new(ErrorType.NotFound, $"The user with email '{email}' was not found.");

    public static Error AlreadyExists(string email) =>
        new(ErrorType.AlreadyExists, $"The user with email '{email}' already exists.");

    public static Error ErrorRegisteringUser() => 
        new(ErrorType.ErrorCreatingUser, "Error occurred registering the user.");
    
    public static Error Null => 
        new(ErrorType.Null, "Error creating the token.");
}