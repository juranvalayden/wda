using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;
using WDA.Application.Users.Commands.LoginUserCommand;
using WDA.Application.Users.Commands.RegisterUserCommand;
using WDA.Application.Users.Queries.GetUserById;
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
    [ProducesResponseType<UserDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> Register(RegisterUserDto registerUserDto, CancellationToken cancellationToken = default)
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

        var queryHandler = provider.GetRequiredService<IHandler<GetUserByIdQuery>>();
        var getUserByIdQuery = new GetUserByIdQuery(success.Data!);
        var queryResponse = await queryHandler.Handle(getUserByIdQuery, cancellationToken);

        return queryResponse is Response<UserDto> querySuccess
            ? CreatedAt(querySuccess.Data!)
            : HandleFailureResponse(queryResponse.Error!);
    }

    [HttpPost("login")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> Login(LoginUserDto loginUserDto, CancellationToken cancellationToken = default)
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

        var commandHandler = provider.GetRequiredService<IHandler<LoginUserCommand>>();
        var response = await commandHandler.Handle(loginUserCommand, cancellationToken);

        if (response is Response<string> successResponse)
        {
            return Ok(successResponse.Data);
        }

        return HandleFailureResponse(response.Error!);
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