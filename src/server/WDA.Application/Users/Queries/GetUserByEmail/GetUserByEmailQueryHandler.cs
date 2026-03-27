using WDA.Application.Abstractions.Common;
using WDA.Application.Dtos;
using WDA.Application.Interfaces;
using WDA.Application.Mappers;
using WDA.Application.Services;
using WDA.Shared.Errors;

namespace WDA.Application.Users.Queries.GetUserByEmail;

public class GetUserByEmailQueryHandler : IHandler<GetUserByEmailQuery>
{
    private readonly IIdentityService _identityService;

    public GetUserByEmailQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
    }

    public async Task<Response> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var response =  await _identityService.GetUserByEmailAsync(request.Email);

        if (response is Response<ApplicationUser> success)
        {
            return Response<UserDto>.Success(UserMapper.MapToDto(success.Data!));
        }

        return response;
    }
}
