namespace WDA.Application.Abstractions.Common;

public interface IQuery : IBaseQuery;

public interface IQuery<out TResponse> : IBaseQuery;