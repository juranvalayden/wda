using WDA.Shared.Errors;

namespace WDA.Application.Abstractions.Common;

public interface IQuery : IRequest;

public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : Response;