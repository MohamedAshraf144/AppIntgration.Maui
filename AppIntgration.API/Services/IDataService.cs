using Microsoft.AspNetCore.Identity.Data;
using AppIntgration.Shard.DTOs;
using AppIntgration.Controllers;
using AppIntgration.Shared.Models;
using AppIntgration.Shard.Requests;

namespace AppIntgration.API.Services;

public interface IDataService : IDisposable
{
    Task<IEnumerable<ServiceDto>> GetServicesAsync();
    Task<ServiceDto?> GetServiceByIdAsync(int id);
    Task<(IEnumerable<LogDto> logs, int totalCount)> GetLogsAsync(LogsRequest request);
    Task<IEnumerable<LogDto>> SearchLogsAsync(string query, string? level);
}