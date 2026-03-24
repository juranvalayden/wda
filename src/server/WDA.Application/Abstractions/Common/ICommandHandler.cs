using WDA.Shared.Errors;

namespace WDA.Application.Abstractions.Common;

public interface ICommandHandler<in TCommand> : IHandler<TCommand> where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse> : IHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : Response;
