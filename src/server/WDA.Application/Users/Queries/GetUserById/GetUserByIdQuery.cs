using WDA.Application.Abstractions.Common;

namespace WDA.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(string UserId) : IQuery;