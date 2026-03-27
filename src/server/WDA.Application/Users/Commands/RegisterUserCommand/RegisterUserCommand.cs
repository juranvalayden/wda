using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;

namespace WDA.Application.Users.Commands.RegisterUserCommand;

public record RegisterUserCommand(RegisterUserDto RegisterUserDto) : ICommand;