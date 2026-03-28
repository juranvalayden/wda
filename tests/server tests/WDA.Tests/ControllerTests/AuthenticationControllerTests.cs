using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;
using WDA.Application.Users.Commands.LoginUserCommand;
using WDA.Application.Users.Commands.RegisterUserCommand;
using WDA.Application.Users.Queries.GetUserById;
using WDA.Shared.Errors;
using WDA.Tests.TestHelpers;
using WDA.WebApi.Controllers;

namespace WDA.Tests.ControllerTests;

[TestFixture]
public class AuthenticationControllerTests
{
    private TestServiceScopeHelper<AuthenticationController> _testServiceScopeHelper = null!;
    private AuthenticationController? _testController;

    [SetUp]
    public void SetUp()
    {
        _testServiceScopeHelper = new TestServiceScopeHelper<AuthenticationController>();
        _testController = new AuthenticationController(_testServiceScopeHelper.LoggerMock.Object, _testServiceScopeHelper.ScopeFactoryMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _testServiceScopeHelper.Dispose();
        _testController = null;
    }

    [Test]
    public async Task Register_ReturnsCreatedAt_OnSuccess()
    {
        // Arrange
        var registerDto = TestData.ValidRegisterUserDto();

        var validatorMock = new Mock<IValidator<RegisterUserCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _testServiceScopeHelper.SetupValidatorFor(validatorMock.Object);

        var commandHandlerMock = new Mock<IHandler<RegisterUserCommand>>();
        const string createdUserId = "user-id-123";
        var commandResponse = Response<string>.Success(createdUserId);

        commandHandlerMock
            .Setup(h => h.Handle(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResponse);

        _testServiceScopeHelper.SetupHandlerFor(commandHandlerMock.Object);

        var queryHandlerMock = new Mock<IHandler<GetUserByIdQuery>>();
        var userDto = TestData.ValidUserDto(createdUserId);
        var queryResponse = Response<UserDto>.Success(userDto);
        queryHandlerMock
            .Setup(h => h.Handle(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResponse);

        _testServiceScopeHelper.SetupHandlerForQuery(queryHandlerMock.Object);

        // Act
        var actionResult = await _testController!.Register(registerDto);

        // Assert
        Assert.IsInstanceOf<CreatedAtRouteResult>(actionResult.Result);
        var createdResult = actionResult.Result as CreatedAtRouteResult;

        Assert.AreEqual("GetUserByEmail", createdResult!.RouteName);
        Assert.IsInstanceOf<UserDto>(createdResult.Value);

        var returnedUser = (UserDto)createdResult.Value!;
        Assert.AreEqual(userDto.Email, returnedUser.Email);
        Assert.AreEqual(userDto.Id, returnedUser.Id);
    }

    [Test]
    public async Task Register_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var registerDto = TestData.InvalidRegisterUserDto();

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

        _testServiceScopeHelper.SetupValidatorFor(validatorMock.Object);

        // Act
        var actionResult = await _testController!.Register(registerDto);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(actionResult.Result);
        var badRequest = (BadRequestObjectResult)actionResult.Result!;
        Assert.AreEqual(invalidResult.Errors, badRequest.Value);
    }

    [Test]
    public async Task Login_ReturnsOk_OnSuccess()
    {
        // Arrange
        var loginDto = TestData.ValidLoginUserDto();

        var validatorMock = new Mock<IValidator<LoginUserCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _testServiceScopeHelper.SetupValidatorFor(validatorMock.Object);

        var commandHandlerMock = new Mock<IHandler<LoginUserCommand>>();
        const string expectedToken = "jwt-token-abc";
        var successResponse = Response<string>.Success(expectedToken);
        commandHandlerMock
            .Setup(h => h.Handle(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResponse);

        _testServiceScopeHelper.SetupHandlerFor(commandHandlerMock.Object);

        // Act
        var actionResult = await _testController!.Login(loginDto);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(actionResult.Result);
        var ok = (OkObjectResult)actionResult.Result!;

        Assert.IsInstanceOf<string>(ok.Value);
        var tokenReturned = (string)ok.Value!;
        Assert.AreEqual(expectedToken, tokenReturned);
    }

    [Test]
    public async Task Login_ReturnsNotFound_OnHandlerErrorNotFound()
    {
        // Arrange
        var loginDto = TestData.InvalidLoginUserDto();

        var validatorMock = new Mock<IValidator<LoginUserCommand>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _testServiceScopeHelper.SetupValidatorFor(validatorMock.Object);

        var commandHandlerMock = new Mock<IHandler<LoginUserCommand>>();
        // var error = new Error(ErrorType.NotFound, "User not found");
        var expectedError = UserErrors.NotFound(loginDto.Email);
        var failureResponse = Response.Failure(expectedError);

        commandHandlerMock
            .Setup(h => h.Handle(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResponse);

        _testServiceScopeHelper.SetupHandlerFor(commandHandlerMock.Object);
        
        // Act
        var actionResult = await _testController!.Login(loginDto);

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(actionResult.Result);
        var notFound = (NotFoundObjectResult)actionResult.Result!;
        Assert.AreEqual(expectedError.Description, notFound.Value);
    }
}