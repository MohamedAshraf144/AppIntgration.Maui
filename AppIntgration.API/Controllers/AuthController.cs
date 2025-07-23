using AppIntgration.Shard.Responses;
using Microsoft.AspNetCore.Mvc;
using AppIntgration.Shard;
using AppIntgration.Services;

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
        try
        {
            var result = await _authService.GenerateApiKeyAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiKeyResponse
            {
                Success = false,
                Message = $"Error generating key: {ex.Message}"
            });
        }
    }

    [HttpPost("validate")]
    public async Task<ActionResult<ApiResponse<bool>>> ValidateKey([FromBody] string apiKey)
    {
        try
        {
            var isValid = await _authService.ValidateApiKeyAsync(apiKey);
            return Ok(new ApiResponse<bool>
            {
                Data = isValid,
                Success = true,
                Message = isValid ? "Key is valid" : "Key is invalid"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }
}