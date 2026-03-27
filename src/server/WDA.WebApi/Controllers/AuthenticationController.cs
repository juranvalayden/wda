using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;
using WDA.Application.Users.Commands.LoginUserCommand;
using WDA.Application.Users.Commands.RegisterUserCommand;
using WDA.Application.Users.Queries.GetUserByEmail;
using WDA.Shared.Errors;

namespace WDA.WebApi.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AuthenticationController(ILogger<AuthenticationController> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var provider = scope.ServiceProvider;
        var validator = provider.GetRequiredService<IValidator<RegisterUserCommand>>();

        var registerUserCommand = new RegisterUserCommand(registerUserDto);
        var validationResult = await validator.ValidateAsync(registerUserCommand, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Register user dto validation failed.");
            return BadRequest(validationResult.Errors);
        }

        var commandHandler = provider.GetRequiredService<IHandler<RegisterUserCommand>>();
        var commandResponse = await commandHandler.Handle(registerUserCommand, cancellationToken);

        if (commandResponse is not Response<string> success) return HandleFailureResponse(commandResponse.Error!);

        var queryHandler = provider.GetRequiredService<IHandler<GetUserByEmailQuery>>();
        var queryResponse = await queryHandler.Handle(new GetUserByEmailQuery(success.Data!), cancellationToken);

        return queryResponse is Response<UserDto> querySuccess
            ? CreatedAt(querySuccess.Data!)
            : HandleFailureResponse(queryResponse.Error!);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var provider = scope.ServiceProvider;
        var validator = provider.GetRequiredService<IValidator<LoginUserCommand>>();

        var loginUserCommand = new LoginUserCommand(loginUserDto);
        var validationResult = await validator.ValidateAsync(loginUserCommand, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Email validation failed.");
            return BadRequest(validationResult.Errors);
        }

        var sender = provider.GetRequiredService<IQueryHandler<GetUserByEmailQuery>>();
        var getUserByEmailQuery = new GetUserByEmailQuery(loginUserDto.Email);
        var queryResponse = await sender.Handle(getUserByEmailQuery, cancellationToken);

        if (queryResponse.IsSuccess && queryResponse is Response<UserDto> success)
        {
            return Ok(success.Data);
        }

        return HandleFailureResponse(queryResponse.Error!);
    }

    private CreatedAtRouteResult CreatedAt(UserDto userDto)
    {
        return CreatedAtRoute(
            "GetUserByEmail",
            new
            {
                email = userDto.Email
            },
            userDto);
    }

    private ActionResult HandleFailureResponse(Error error)
    {
        var description = error.Description;

        return error.ErrorType switch
        {
            ErrorType.NotFound => NotFound(description),
            ErrorType.AlreadyExists => Conflict(description),
            _ => BadRequest()
        };
    }
}