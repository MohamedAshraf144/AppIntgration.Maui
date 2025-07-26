using AppIntgration.Shard.Responses;
using AppIntgration.Services;
using AppIntgration.API.Services;
using AppIntgration.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppIntgration.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IDataService _dataService;
    private readonly IAuthService _authService;

    public ServicesController(IDataService dataService, IAuthService authService)
    {
        _dataService = dataService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ServiceDto>>>> GetServices()
    {
        // تحقق من المفتاح
        if (!await IsValidRequest())
        {
            return Unauthorized(new { message = "Invalid or missing API key" });
        }

        try
        {
            var services = await _dataService.GetServicesAsync();
            return Ok(new ApiResponse<IEnumerable<ServiceDto>>
            {
                Data = services,
                Success = true,
                Message = $"Retrieved {services.Count()} services successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<IEnumerable<ServiceDto>>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ServiceDto>>> GetService(int id)
    {
        if (!await IsValidRequest())
        {
            return Unauthorized(new { message = "Invalid or missing API key" });
        }

        try
        {
            var service = await _dataService.GetServiceByIdAsync(id);
            if (service == null)
            {
                return NotFound(new ApiResponse<ServiceDto>
                {
                    Success = false,
                    Message = "Service not found"
                });
            }

            return Ok(new ApiResponse<ServiceDto>
            {
                Data = service,
                Success = true,
                Message = "Service found"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ServiceDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    // دالة مساعدة للتحقق من المفتاح
    private async Task<bool> IsValidRequest()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();

        if (authHeader == null || !authHeader.StartsWith("Bearer "))
            return false;

        var token = authHeader.Substring("Bearer ".Length).Trim();
        return await _authService.ValidateApiKeyAsync(token);
    }
}