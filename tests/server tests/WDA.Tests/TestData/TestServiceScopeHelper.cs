using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using WDA.Application.Abstractions.Common;

namespace WDA.Tests.TestData;

public class TestServiceScopeHelper<TController> : IDisposable where TController : ControllerBase
{
    public Mock<ILogger<TController>> LoggerMock { get; }
    public Mock<IServiceScopeFactory> ScopeFactoryMock { get; }
    public Mock<IServiceScope> ScopeMock { get; }
    public Mock<IServiceProvider> ProviderMock { get; }

    public TestServiceScopeHelper()
    {
        LoggerMock = new Mock<ILogger<TController>>();
        ScopeFactoryMock = new Mock<IServiceScopeFactory>();
        ScopeMock = new Mock<IServiceScope>();
        ProviderMock = new Mock<IServiceProvider>();

        ScopeMock.Setup(s => s.ServiceProvider).Returns(ProviderMock.Object);
        ScopeFactoryMock.Setup(f => f.CreateScope()).Returns(ScopeMock.Object);
    }

    public void SetupValidatorFor<T>(IValidator<T> validator)
        where T : class
    {
        ProviderMock
            .Setup(p => p.GetService(typeof(IValidator<T>)))
            .Returns(validator);
    }


    public void SetupHandlerFor<TCommand>(IHandler<TCommand> handler)
        where TCommand : IRequest
    {
        ProviderMock
            .Setup(p => p.GetService(typeof(IHandler<TCommand>)))
            .Returns(handler);
    }

    public void SetupHandlerForQuery<TQuery>(IHandler<TQuery> handler)
        where TQuery : IRequest
    {
        ProviderMock
            .Setup(p => p.GetService(typeof(IHandler<TQuery>)))
            .Returns(handler);
    }

    public void Dispose()
    {
    }
}

