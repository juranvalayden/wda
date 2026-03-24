namespace WDA.Application.Abstractions.Common;

public interface ICommand : IRequest;

public interface ICommand<out TResponse> : IRequest<TResponse>;