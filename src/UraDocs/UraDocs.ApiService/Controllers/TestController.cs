using Microsoft.AspNetCore.Mvc;

namespace UraDocs.ApiService.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return new JsonResult(new { Message = "Hello World!" });
    }
}
