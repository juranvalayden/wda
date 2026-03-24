namespace WDA.Application.Abstractions.Common;

public interface IQuery<out TResponse> : IRequest<TResponse>;