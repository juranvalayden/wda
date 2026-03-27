using Microsoft.AspNetCore.Identity;
using WDA.Shared.Errors;

namespace WDA.Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static Response ToApplicationResult(this IdentityResult result)
    {
        var error = string.Join(", ", result.Errors.Select(e => e.Description));

        return result.Succeeded
            ? Response.Success()
            : Response.Failure(new Error(ErrorType.Identity, error));
    }
}
