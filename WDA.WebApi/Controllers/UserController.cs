using Microsoft.AspNetCore.Mvc;
using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;
using WDA.Application.Users.Commands.CreateUserCommand;
using WDA.Application.Users.Queries.GetUserByEmail;
using WDA.Shared.Errors;
using Error = WDA.Shared.Errors.Error;

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
        var sender = provider.GetRequiredService<IQueryHandler<GetUserByEmailQuery>>();

        var getUserByEmailQuery = new GetUserByEmailQuery(email);
        var result = await sender.Handle(getUserByEmailQuery, cancellationToken);

        return result switch
        {
            { IsFailure: true, Error: not null } => HandleFailureResponse(result.Error),
            Success<UserDto> success => Ok(success.Data),
            _ => BadRequest()
        };
    }

    [HttpPost]
    public async Task<ActionResult<int?>> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var provider = scope.ServiceProvider;
        var sender = provider.GetRequiredService<ICommandHandler<CreateUserCommand>>();

        var command = new CreateUserCommand(createUserDto);
        var result = await sender.Handle(command, cancellationToken);

        return result switch
        {
            { IsFailure: true, Error: not null } => HandleFailureResponse(result.Error),
            Success<int> success =>
                CreatedAtRoute("GetUserById", new
                {
                    Id = success.Data,

                }, success.Data),
            _ => BadRequest()
        };
    }

    private ActionResult HandleFailureResponse(Error error)
    {
        var description = error.Description;

        return error.ErrorType switch
        {
            ErrorType.NotFound => NotFound(description),
            ErrorType.AlreadyExists => Conflict(description),
            ErrorType.ErrorSaving => BadRequest(description),
            _ => BadRequest()
        };
    }
}