using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;
using WDA.Application.Interfaces;
using WDA.Application.Mappers;
using WDA.Application.Services;
using WDA.Shared.Errors;

namespace WDA.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IHandler<GetUserByIdQuery>
{
    private readonly IIdentityService _identityService;

    public GetUserByIdQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
    }

    public async Task<Response> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var response = await _identityService.GetUserByIdAsync(request.UserId);

        if (response is not Response<ApplicationUser> success) return response;

        var userDto = UserMapper.MapToDto(success.Data!);
        return Response<UserDto>.Success(userDto);
    }
}