using WDA.Application.Abstractions.Common;

namespace WDA.Application.Users.Queries.GetUserByEmail;

public record GetUserByEmailQuery(string Email) : IQuery;