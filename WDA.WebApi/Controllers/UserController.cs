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
    private readonly ICommandHandler<CreateUserCommand> _commandHandler;
    private readonly IQueryHandler<GetUserByEmailQuery> _queryHandler;

    public UserController(ILogger<UserController> logger, ICommandHandler<CreateUserCommand> commandHandler, IQueryHandler<GetUserByEmailQuery> queryHandler)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
        _queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
    }

    [HttpGet("{email}", Name = "GetUserByEmail")]
    public async Task<ActionResult<UserDto?>> GetUserByEmail(string email, CancellationToken cancellationToken = default)
    {
        var getUserByEmailQuery = new GetUserByEmailQuery(email);
        var result = await _queryHandler.Handle(getUserByEmailQuery, cancellationToken);

        return result switch
        {
            { IsFailure: true, Error: not null } => HandleFailureResponse(result.Error),
            Success<UserDto> success => Ok(success.Data),
            _ => BadRequest()
        };
    }

    [HttpPost]
    public async Task<ActionResult<string?>> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default)
    {
        var command = new CreateUserCommand(createUserDto);
        var result = await _commandHandler.Handle(command, cancellationToken);

        return result switch
        {
            { IsFailure: true, Error: not null } => HandleFailureResponse(result.Error),
            Success<int> success =>
                CreatedAtRoute("GetUserByEmail", new
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