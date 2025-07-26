using AppIntgration.Shard.Responses;
using AppIntgration.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppIntgration.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("apikey")]
    public async Task<ActionResult<ApiKeyResponse>> GetApiKey()
    {
        var result = await _authService.GenerateApiKeyAsync();
        return Ok(result);
    }

    [HttpPost("validate")]
    public async Task<ActionResult> ValidateKey([FromBody] string apiKey)
    {
        var isValid = await _authService.ValidateApiKeyAsync(apiKey);
        return Ok(new { isValid, message = isValid ? "Valid key" : "Invalid key" });
    }
}
