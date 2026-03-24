using WDA.Shared.Errors;

namespace WDA.Application.Abstractions.Common;

public interface IQueryHandler<in TQuery> : IHandler<TQuery> where TQuery : IQuery;

public interface IQueryHandler<in TQuery, TResponse> : IHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : Response;
