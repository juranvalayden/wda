using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;
using WDA.Application.Users.Commands.LoginUserCommand;
using WDA.Application.Users.Commands.RegisterUserCommand;
using WDA.Application.Users.Queries.GetUserById;
using WDA.Shared.Errors;
using WDA.Tests.TestData;
using WDA.WebApi.Controllers;

namespace WDA.Tests.AuthenticationControllerTests;

[TestFixture]
public class AuthenticationControllerTests
{
    private Mock<ILogger<AuthenticationController>> _loggerMock = null!;
    private Mock<IServiceScopeFactory> _scopeFactoryMock = null!;
    private Mock<IServiceScope> _scopeMock = null!;
    private Mock<IServiceProvider> _providerMock = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<AuthenticationController>>();
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _scopeMock = new Mock<IServiceScope>();
        _providerMock = new Mock<IServiceProvider>();

        _scopeMock.Setup(s => s.ServiceProvider).Returns(_providerMock.Object);
        _scopeFactoryMock.Setup(f => f.CreateScope()).Returns(_scopeMock.Object);
    }

    private void SetupValidatorFor<TCommand>(IValidator<TCommand> validator)
        where TCommand : class
    {
        _providerMock
            .Setup(p => p.GetService(typeof(IValidator<TCommand>)))
            .Returns(validator);
    }

    private void SetupHandlerFor<TCommand>(IHandler<TCommand> handler)
        where TCommand : IRequest
    {
        _providerMock
            .Setup(p => p.GetService(typeof(IHandler<TCommand>)))
            .Returns(handler);
    }

    private void SetupHandlerForQuery<TQuery>(IHandler<TQuery> handler)
        where TQuery : IRequest
    {
        _providerMock
            .Setup(p => p.GetService(typeof(IHandler<TQuery>)))
            .Returns(handler);
    }


    [Test]
    public async Task Register_ReturnsCreatedAt_OnSuccess()
    {
        // Arrange
        var controller = new AuthenticationController(_loggerMock.Object, _scopeFactoryMock.Object);

        var registerDto = TestControllerData.ValidRegisterUserDto();

        var validatorMock = new Mock<IValidator<RegisterUserCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        SetupValidatorFor(validatorMock.Object);

        // Command handler returns Response<string> with created user id
        var commandHandlerMock = new Mock<IHandler<RegisterUserCommand>>();
        var createdUserId = "user-id-123";
        var commandResponse = Response<string>.Success(createdUserId);

        commandHandlerMock
            .Setup(h => h.Handle(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResponse);

        SetupHandlerFor(commandHandlerMock.Object);

        // Query handler returns Response<UserDto>
        var queryHandlerMock = new Mock<IHandler<GetUserByIdQuery>>();
        var userDto = TestControllerData.ValidUserDto(createdUserId);
        var queryResponse = Response<UserDto>.Success(userDto);
        queryHandlerMock
            .Setup(h => h.Handle(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResponse);

        SetupHandlerForQuery(queryHandlerMock.Object);

        // Act
        var result = await controller.Register(registerDto);

        // Assert
        Assert.IsInstanceOf<CreatedAtRouteResult>(result);
        var createdResult = (CreatedAtRouteResult)result;
        Assert.AreEqual("GetUserByEmail", createdResult.RouteName);
        Assert.IsInstanceOf<UserDto>(createdResult.Value);
        var returnedUser = (UserDto)createdResult.Value!;
        Assert.AreEqual(userDto.Email, returnedUser.Email);
        Assert.AreEqual(userDto.Id, returnedUser.Id);
    }

    [Test]
    public async Task Register_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var controller = new AuthenticationController(_loggerMock.Object, _scopeFactoryMock.Object);

        var registerDto = TestControllerData.InvalidRegisterUserDto();

        // Validator that returns invalid result
        var failures = new List<ValidationFailure>
        {
            new("Email", "Email is invalid"),
            new("Password", "Password too short")
        };

        var invalidResult = new ValidationResult(failures);

        var validatorMock = new Mock<IValidator<RegisterUserCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);

        SetupValidatorFor(validatorMock.Object);

        // Act
        var result = await controller.Register(registerDto);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        var badRequest = (BadRequestObjectResult)result;
        Assert.AreEqual(invalidResult.Errors, badRequest.Value);
    }

    [Test]
    public async Task Login_ReturnsOk_OnSuccess()
    {
        // Arrange
        var controller = new AuthenticationController(_loggerMock.Object, _scopeFactoryMock.Object);

        var loginDto = TestControllerData.ValidLoginUserDto();

        var validatorMock = new Mock<IValidator<LoginUserCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        SetupValidatorFor(validatorMock.Object);

        var commandHandlerMock = new Mock<IHandler<LoginUserCommand>>();
        const string token = "jwt-token-abc";
        var successResponse = Response<string>.Success(token);
        commandHandlerMock
            .Setup(h => h.Handle(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResponse);

        SetupHandlerFor(commandHandlerMock.Object);

        // Act
        var result = await controller.Login(loginDto);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var ok = (OkObjectResult)result;
        Assert.AreEqual(token, ok.Value);
    }

    [Test]
    public async Task Login_ReturnsNotFound_OnHandlerErrorNotFound()
    {
        // Arrange
        var controller = new AuthenticationController(_loggerMock.Object, _scopeFactoryMock.Object);

        var loginDto = TestControllerData.InvalidLoginUserDto();

        var validatorMock = new Mock<IValidator<LoginUserCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        SetupValidatorFor(validatorMock.Object);

        var commandHandlerMock = new Mock<IHandler<LoginUserCommand>>();
        var error = new Error(ErrorType.NotFound, "User not found");
        var failureResponse = Response.Failure(error);

        commandHandlerMock
            .Setup(h => h.Handle(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResponse);

        SetupHandlerFor(commandHandlerMock.Object);

        // Act
        var result = await controller.Login(loginDto);

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
        var notFound = (NotFoundObjectResult)result;
        Assert.AreEqual(error.Description, notFound.Value);
    }
}