namespace WDA.Application.Abstractions.Common;

public interface ICommand : IBaseCommand;

public interface ICommand<TResponse> : IBaseCommand;