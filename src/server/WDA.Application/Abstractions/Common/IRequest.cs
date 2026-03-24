namespace WDA.Application.Abstractions.Common;

public interface IRequest : IBaseRequest;

public interface IRequest<out TResponse> : IBaseRequest;