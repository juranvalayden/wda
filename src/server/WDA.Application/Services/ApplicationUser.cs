using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using WDA.Shared;

namespace WDA.Application.Services;

public class ApplicationUser : IdentityUser
{
    [StringLength(Constants.FirstNameMaxTextLength), MinLength(Constants.FirstNameMinTextLength)]
    public string FirstName { get; init; }

    [StringLength(Constants.LastNameMaxTextLength), MinLength(Constants.LastNameMinTextLength)]
    public string LastName { get; init; }

    [StringLength(Constants.PasswordMaxTextLength), MinLength(Constants.PasswordMinTextLength)]
    public string Password { get; set; }

    [StringLength(Constants.PasswordMaxTextLength), MinLength(Constants.PasswordMinTextLength)]
    public string ConfirmedPassword { get; set; }

    public ApplicationUser(string firstName, string lastName, string email, string password, string confirmedPassword)
    {
        FirstName = firstName;
        LastName = lastName;
        Password = password;
        ConfirmedPassword = confirmedPassword;
        UserName = email;
        Email = email;
    }
}