using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Resources.Api.Controllers;

[Route("identity")]
[ApiController]
[Authorize(Policy = "ApiScope")]
public class IdentityController : ControllerBase
{
    private readonly ILogger<IdentityController> _logger;

    public IdentityController(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<IdentityController>();
    }

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("获取useinfo");
        return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
    }
}