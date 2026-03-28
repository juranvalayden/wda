using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using WDA.Application.Abstractions.Common;

namespace WDA.Tests.TestHelpers;

public class TestServiceScopeHelper<TController> : IDisposable where TController : ControllerBase
{
    public Mock<ILogger<TController>> LoggerMock { get; }
    public Mock<IServiceScopeFactory> ScopeFactoryMock { get; }
    // ReSharper disable once InconsistentNaming
    private Mock<IServiceScope> _serviceScope { get; }
    // ReSharper disable once InconsistentNaming
    private Mock<IServiceProvider> _providerMock { get; }

    public TestServiceScopeHelper()
    {
        LoggerMock = new Mock<ILogger<TController>>();
        ScopeFactoryMock = new Mock<IServiceScopeFactory>();
        _serviceScope = new Mock<IServiceScope>();
        _providerMock = new Mock<IServiceProvider>();

        _serviceScope.Setup(s => s.ServiceProvider).Returns(_providerMock.Object);
        ScopeFactoryMock.Setup(f => f.CreateScope()).Returns(_serviceScope.Object);
    }

    public void SetupValidatorFor<T>(IValidator<T> validator)
        where T : class
    {
        _providerMock
            .Setup(p => p.GetService(typeof(IValidator<T>)))
            .Returns(validator);
    }


    public void SetupHandlerFor<TCommand>(IHandler<TCommand> handler)
        where TCommand : IRequest
    {
        _providerMock
            .Setup(p => p.GetService(typeof(IHandler<TCommand>)))
            .Returns(handler);
    }

    public void SetupHandlerForQuery<TQuery>(IHandler<TQuery> handler)
        where TQuery : IRequest
    {
        _providerMock
            .Setup(p => p.GetService(typeof(IHandler<TQuery>)))
            .Returns(handler);
    }

    public void Dispose()
    {
    }
}

