using WDA.Shared.Errors;

namespace WDA.Application.Abstractions.Common;

public interface IHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<Response<TResponse>> Handle(TRequest request, CancellationToken cancellationToken = default);
}

public interface IHandler<in TRequest> where TRequest : IRequest
{
    Task<Response> Handle(TRequest request, CancellationToken cancellationToken = default);
}