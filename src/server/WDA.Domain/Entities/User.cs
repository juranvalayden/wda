using WDA.Domain.Common.Abstractions;

namespace WDA.Domain.Entities;

public record User(string FirstName, string LastName, string Email, string Password) : BaseAuditableEntity;