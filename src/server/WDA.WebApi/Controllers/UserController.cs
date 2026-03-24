using FluentValidation;
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
    private readonly IValidator<CreateUserCommand> _commandValidator;
    private readonly IValidator<GetUserByEmailQuery> _queryValidator;

    public UserController(ILogger<UserController> logger, 
        ICommandHandler<CreateUserCommand> commandHandler, 
        IQueryHandler<GetUserByEmailQuery> queryHandler,
        IValidator<CreateUserCommand> commandValidator,
        IValidator<GetUserByEmailQuery> queryValidator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
        _queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
        _commandValidator = commandValidator ?? throw new ArgumentNullException(nameof(commandValidator));
        _queryValidator = queryValidator ?? throw new ArgumentNullException(nameof(queryValidator));
    }

    [HttpGet("{email}", Name = "GetUserByEmail")]
    public async Task<ActionResult<UserDto?>> GetUserByEmail(string email, CancellationToken cancellationToken = default)
    {
        var getUserByEmailQuery = new GetUserByEmailQuery(email);

        var validationResult = await _queryValidator.ValidateAsync(getUserByEmailQuery, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Email validation failed.");
            return BadRequest(validationResult.Errors);
        }
        
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
        var createUserCommand = new CreateUserCommand(createUserDto);

        var validationResult = await _commandValidator.ValidateAsync(createUserCommand, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("create user dto validation failed.");
            return BadRequest(validationResult.Errors);
        }

        var commandResult = await _commandHandler.Handle(createUserCommand, cancellationToken);

        if (commandResult.IsFailure)
        {
            HandleFailureResponse(commandResult.Error!);
        }

        if (commandResult is not Success<string> commandSuccess || string.IsNullOrWhiteSpace(commandSuccess.Data))
            return BadRequest();

        var email = commandSuccess.Data;

        var queryResult = await _queryHandler.Handle(new GetUserByEmailQuery(email), cancellationToken);

        if (commandResult.IsFailure)
        {
            HandleFailureResponse(commandResult.Error!);
        }

        if (queryResult is not Success<UserDto> { Data: not null } querySuccess) return BadRequest();

        var userDto = querySuccess.Data;

        return CreatedAtRoute(
            "GetUserByEmail",
            new
            {
                email = userDto.Email,
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
            ErrorType.ErrorSaving => BadRequest(description),
            _ => BadRequest()
        };
    }
}