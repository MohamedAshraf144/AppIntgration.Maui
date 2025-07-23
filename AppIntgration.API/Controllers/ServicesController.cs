using AppIntgration.Shard.Responses;
using Microsoft.AspNetCore.Mvc;
using AppIntgration.Shard;
using AppIntgration.Services;
using AppIntgration.API.Services;
using AppIntgration.Shared.Models;

namespace AppIntgration.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IDataService _dataService;

    public ServicesController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ServiceDto>>>> GetServices()
    {
        try
        {
            var services = await _dataService.GetServicesAsync();
            return Ok(new ApiResponse<IEnumerable<ServiceDto>>
            {
                Data = services,
                Success = true,
                Message = $"تم جلب {services.Count()} خدمة بنجاح"
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
}