using AppIntgration.Controllers;
using AppIntgration.Services;
using AppIntgration.Shard.DTOs;
using AppIntgration.Shared.Models;
using Elastic.Apm.Api;
using Microsoft.AspNetCore.Identity.Data;
using AppIntgration.Shard.Constants;
using AppIntgration.Shard.Requests;

namespace AppIntgration.API.Services
{
    public class DataService : IDataService
    {
        private readonly List<ServiceDto> _services;
        private readonly List<LogDto> _logs;

        public DataService()
        {
            _services = GenerateMockServices();
            _logs = GenerateMockLogs();
        }

        public async Task<IEnumerable<ServiceDto>> GetServicesAsync()
        {
            await Task.Delay(200); // Simulate database delay
            return _services;
        }

        public async Task<ServiceDto?> GetServiceByIdAsync(int id)
        {
            await Task.Delay(100);
            return _services.FirstOrDefault(s => s.Id == id);
        }

        public async Task<(IEnumerable<LogDto> logs, int totalCount)> GetLogsAsync(LogsRequest request)
        {
            await Task.Delay(300);

            var query = _logs.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(request.Level))
                query = query.Where(l => l.Level == request.Level);

            if (!string.IsNullOrEmpty(request.Source))
                query = query.Where(l => l.Source.Contains(request.Source));

            if (request.FromDate.HasValue)
                query = query.Where(l => l.Timestamp >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(l => l.Timestamp <= request.ToDate.Value);

            var totalCount = query.Count();

            // Paging and sorting
            var logs = query
                .OrderByDescending(l => l.Timestamp)
                .Skip((request.Page - 1) * request.Size)
                .Take(request.Size)
                .ToList();

            return (logs, totalCount);
        }

        public async Task<IEnumerable<LogDto>> SearchLogsAsync(string query, string? level)
        {
            await Task.Delay(150);

            var searchQuery = _logs.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                searchQuery = searchQuery.Where(l =>
                    l.Message.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    l.Source.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(level))
                searchQuery = searchQuery.Where(l => l.Level == level);

            return searchQuery.OrderByDescending(l => l.Timestamp).Take(100);
        }

        private List<ServiceDto> GenerateMockServices()
        {
            return new List<ServiceDto>
        {
            new() { Id = 1, Name = "Main Web Service", Status = AppIntgration.Shard.Constants.ApiConstants.ServiceStatuses.Active, Description = "Core API service for the app", CreatedAt = DateTime.UtcNow.AddDays(-30) },
            new() { Id = 2, Name = "Database", Status = AppIntgration.Shard.Constants.ApiConstants.ServiceStatuses.Active, Description = "Central database", CreatedAt = DateTime.UtcNow.AddDays(-45) },
            new() { Id = 3, Name = "Storage Service", Status = AppIntgration.Shard.Constants.ApiConstants.ServiceStatuses.Maintenance, Description = "File and document storage", CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new() { Id = 4, Name = "Notification Service", Status = AppIntgration.Shard.Constants.ApiConstants.ServiceStatuses.Active, Description = "Send notifications to users", CreatedAt = DateTime.UtcNow.AddDays(-15) },
            new() { Id = 5, Name = "Backup Service", Status = AppIntgration.Shard.Constants.ApiConstants.ServiceStatuses.Error, Description = "Data backup", CreatedAt = DateTime.UtcNow.AddDays(-10) }
        };
        }

        private List<LogDto> GenerateMockLogs()
        {
            var random = new Random();
            var levels = new[] { AppIntgration.Shard.Constants.ApiConstants.LogLevels.Info, AppIntgration.Shard.Constants.ApiConstants.LogLevels.Warning, AppIntgration.Shard.Constants.ApiConstants.LogLevels.Error, AppIntgration.Shard.Constants.ApiConstants.LogLevels.Success };
            var sources = new[] { "Web App", "Database", "API Service", "Authentication System", "Storage Service" };
            var messages = new[]
            {
            "New user login successful",
            "Database connection failed",
            "Backup created successfully",
            "Warning: High memory usage",
            "Data saved successfully",
            "Error processing request",
            "System settings updated",
            "Failed to send notification",
            "Maintenance operation completed",
            "Warning: Low storage space"
        };

            return Enumerable.Range(1, 500).Select(i => new LogDto
            {
                Id = i,
                Timestamp = DateTime.UtcNow.AddMinutes(-random.Next(1, 10080)), // Last week
                Level = levels[random.Next(levels.Length)],
                Message = messages[random.Next(messages.Length)],
                Source = sources[random.Next(sources.Length)],
                Exception = random.Next(10) < 2 ? "System.Exception: Detailed error here" : null
            }).ToList();
        }

        // إضافة Dispose method لحل مشكلة IDisposable
        public void Dispose()
        {
            // مفيش حاجة محتاجة cleanup في الـ DataService العادي
            // بس لازم نعمل implement للـ interface
        }
    }
}