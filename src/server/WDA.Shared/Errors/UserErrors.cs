namespace WDA.Shared.Errors;

public static class UserErrors
{
    public static Error NotFound(string email) =>
        new(ErrorType.NotFound, $"The user with email '{email}' was not found.");

    public static Error AlreadyExists(string email) =>
        new(ErrorType.AlreadyExists, $"The user with email '{email}' already exists.");

    public static Error ErrorSaving() =>
        new(ErrorType.ErrorSaving, "Error saving the user.");

    public static Error ValidationErrors(string errors) =>
        new(ErrorType.ValidationErrors, $"Validation errors: {errors}");
}