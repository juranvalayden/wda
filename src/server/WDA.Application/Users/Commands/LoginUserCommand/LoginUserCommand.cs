using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;

namespace WDA.Application.Users.Commands.LoginUserCommand;

public record LoginUserCommand(LoginUserDto LoginUserDto) : ICommand;