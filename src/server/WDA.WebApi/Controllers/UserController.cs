using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;
using WDA.Application.Users.Commands.CreateUserCommand;
using WDA.Application.Users.Queries.GetUserByEmail;
using WDA.Shared.Errors;

namespace WDA.WebApi.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UserController(ILogger<UserController> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    [HttpGet("{email}", Name = "GetUserByEmail")]
    public async Task<ActionResult<UserDto?>> GetUserByEmail(string email, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var provider = scope.ServiceProvider;
        var validator = provider.GetRequiredService<IValidator<GetUserByEmailQuery>>();

        var getUserByEmailQuery = new GetUserByEmailQuery(email);
        var validationResult = await validator.ValidateAsync(getUserByEmailQuery, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Email validation failed.");
            return BadRequest(validationResult.Errors);
        }

        var sender = provider.GetRequiredService<IQueryHandler<GetUserByEmailQuery>>();
        var response = await sender.Handle(getUserByEmailQuery, cancellationToken);

        if (response.IsSuccess && response is Response<UserDto> success)
        {
            return Ok(success.Data);
        }

        return HandleFailureResponse(response.Error!);
    }

    [HttpPost]
    public async Task<ActionResult<string?>> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var provider = scope.ServiceProvider;
        var validator = provider.GetRequiredService<IValidator<CreateUserCommand>>();

        var createUserCommand = new CreateUserCommand(createUserDto);
        var validationResult = await validator.ValidateAsync(createUserCommand, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("create user dto validation failed.");
            return BadRequest(validationResult.Errors);
        }

        var sender = provider.GetRequiredService<ICommandHandler<CreateUserCommand>>();
        var response = await sender.Handle(createUserCommand, cancellationToken);

        if (response.IsSuccess && response is Response<UserDto> success)
        {
            return CreatedAt(success.Data!);
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