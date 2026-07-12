using AccessHub.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccessHub.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ParameterStoreController : ControllerBase
{
    private readonly ParameterStoreService _parameterStoreService;

    public ParameterStoreController(ParameterStoreService parameterStoreService)
    {
        _parameterStoreService = parameterStoreService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var value = await _parameterStoreService.GetParameterAsync("/accesshub/jwt-key");

        return Ok(value);
    }
}
