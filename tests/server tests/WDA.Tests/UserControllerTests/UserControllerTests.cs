using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;
using WDA.Application.Users.Queries.GetUserByEmail;
using WDA.Shared.Errors;
using WDA.Tests.TestData;
using WDA.WebApi.Controllers;

namespace WDA.Tests.UserControllerTests;

[TestFixture]
public class UserControllerTests
{
    private TestServiceScopeHelper<UserController> _testServiceScopeHelper = null!;
    private UserController? _testController;

    [SetUp]
    public void SetUp()
    {
        _testServiceScopeHelper = new TestServiceScopeHelper<UserController>();
        _testController = new UserController(_testServiceScopeHelper.LoggerMock.Object, _testServiceScopeHelper.ScopeFactoryMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _testServiceScopeHelper.Dispose();
        _testController = null;
    }

    [Test]
    public async Task GetUserByEmail_ReturnsOk_OnSuccess()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<GetUserByEmailQuery>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _testServiceScopeHelper.SetupValidatorFor(validatorMock.Object);

        var handlerMock = new Mock<IHandler<GetUserByEmailQuery>>();
        var userDto = TestControllerData.ValidUserDto("xyz");
        var successResponse = Response<UserDto>.Success(userDto);
        handlerMock
            .Setup(h => h.Handle(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResponse);

        _testServiceScopeHelper.SetupHandlerFor(handlerMock.Object);

        // Act
        var actionResult = await _testController!.GetUserByEmail(userDto.Email);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(actionResult.Result);
        var ok = (OkObjectResult)actionResult.Result!;
        Assert.IsInstanceOf<UserDto>(ok.Value);
        var returned = (UserDto)ok.Value!;
        Assert.AreEqual(userDto.Email, returned.Email);
        Assert.AreEqual(userDto.Id, returned.Id);
    }

    [Test]
    public async Task GetUserByEmail_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var email = "invalid-email";

        var failures = new List<ValidationFailure> { new("Email", "Email is invalid") };
        var invalidResult = new ValidationResult(failures);

        var validatorMock = new Mock<IValidator<GetUserByEmailQuery>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
        _testServiceScopeHelper.SetupValidatorFor(validatorMock.Object);

        // Act
        var actionResult = await _testController!.GetUserByEmail(email);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(actionResult.Result);
        var badRequest = (BadRequestObjectResult)actionResult.Result!;
        Assert.AreEqual(invalidResult.Errors, badRequest.Value);
    }

    [Test]
    public async Task GetUserByEmail_ReturnsNotFound_WhenHandlerReturnsNotFound()
    {
        // Arrange
        var email = "missing@example.com";

        // Validator returns valid
        var validatorMock = new Mock<IValidator<GetUserByEmailQuery>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        
        _testServiceScopeHelper.SetupValidatorFor(validatorMock.Object);

        // Handler returns failure with NotFound error
        var handlerMock = new Mock<IHandler<GetUserByEmailQuery>>();
        var error = new Error(ErrorType.NotFound, "User not found");
        var failureResponse = Response.Failure(error);
        handlerMock
            .Setup(h => h.Handle(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResponse);
        _testServiceScopeHelper.SetupHandlerFor(handlerMock.Object);

        // Act
        var actionResult = await _testController!.GetUserByEmail(email);

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(actionResult.Result);
        var notFound = (NotFoundObjectResult)actionResult.Result!;
        Assert.AreEqual(error.Description, notFound.Value);
    }
}