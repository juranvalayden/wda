using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;
using WDA.Application.Users.Queries.GetUserByEmail;
using WDA.Shared.Errors;

namespace WDA.WebApi.Controllers;

[Authorize]
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
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        var queryHandler = provider.GetRequiredService<IHandler<GetUserByEmailQuery>>();
        var response = await queryHandler.Handle(getUserByEmailQuery, cancellationToken);

        if (response.IsSuccess && response is Response<UserDto> success)
        {
            return Ok(success.Data);
        }    

        return HandleFailureResponse(response.Error!);
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