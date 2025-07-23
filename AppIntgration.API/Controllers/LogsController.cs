using AppIntgration.Shard.DTOs;
using AppIntgration.Shard.Requests;
using AppIntgration.Shard.Responses;
using AppIntgration.Shard.Constants;
using AppIntgration.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppIntgration.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    [HttpGet]
    public ActionResult<PaginatedResponse<LogDto>> GetLogs([FromQuery] LogsRequest request)
    {
        // Validate API key
        if (!ValidateApiKey())
        {
            return Unauthorized("Valid API key required");
        }

        // Generate mock log data
        var random = new Random();
        var levels = new[] { ApiConstants.LogLevels.Info, ApiConstants.LogLevels.Warning, ApiConstants.LogLevels.Error, ApiConstants.LogLevels.Success };
        var sources = new[] { "Web Application", "Database", "API Service", "Authentication" };
        var messages = new[]
        {
            "User login successful",
            "Database connection failed",
            "Data saved successfully",
            "Warning: High memory usage",
            "Maintenance operation completed"
        };

        var logs = Enumerable.Range(1, request.Size).Select(i => new LogDto
        {
            Id = i,
            Timestamp = DateTime.UtcNow.AddMinutes(-random.Next(1, 1440)),
            Level = levels[random.Next(levels.Length)],
            Message = messages[random.Next(messages.Length)],
            Source = sources[random.Next(sources.Length)]
        }).ToList();

        return Ok(new PaginatedResponse<LogDto>
        {
            Data = logs,
            Success = true,
            Message = $"Retrieved {logs.Count} log entries",
            Pagination = new PaginationInfo
            {
                Page = request.Page,
                Size = request.Size,
                TotalPages = 5,
                TotalItems = 100
            }
        });
    }

    private bool ValidateApiKey()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer "))
            return false;

        var token = authHeader.Substring("Bearer ".Length).Trim();
        return !string.IsNullOrEmpty(token);
    }
}