using Microsoft.AspNetCore.Mvc;

namespace WDA.WebApi.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    [HttpGet]
    public ActionResult<int> Get()
    {
        return Ok(4);
    }
}