using Microsoft.AspNetCore.Mvc;

namespace WDA.WebApi.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    [HttpGet("{email}", Name = "GetUserByEmail")]
    public ActionResult<int> GetUserByEmailAsync(string email)
    {
        return 10;
    }
}