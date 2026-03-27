namespace WDA.Application.Dtos;

public record RegisterUserDto(string FirstName, string LastName, string Email, string Password, string ConfirmedPassword);