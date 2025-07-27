using AppIntgration.Shard.DTOs;
using AppIntgration.Shared.Models;
using AppIntgration.Shard.Constants;
using AppIntgration.Shard.Requests;
using System.Text.Json;

namespace AppIntgration.API.Services
{
    public class EnhancedDataService : IDataService
    {
        private readonly List<ServiceDto> _services;
        private readonly List<LogDto> _logs;
        private readonly Random _random = new();
        private readonly Timer _updateTimer;
        private readonly Timer _saveTimer;
        private readonly string _dataFolder = "Data";
        private readonly ILogger<EnhancedDataService> _logger;

        public EnhancedDataService(ILogger<EnhancedDataService> logger)
        {
            _logger = logger;

            // إنشاء مجلد البيانات إذا لم يكن موجوداً
            Directory.CreateDirectory(_dataFolder);

            // تحميل البيانات المحفوظة أو إنشاء جديدة
            _services = LoadServicesFromFile() ?? GenerateRealisticServices();
            _logs = LoadLogsFromFile() ?? new List<LogDto>();

            // إضافة logs جديدة كل دقيقة
            _updateTimer = new Timer(AddRandomRealisticLog, null,
                TimeSpan.Zero, TimeSpan.FromMinutes(1));

            // حفظ البيانات كل 5 دقائق
            _saveTimer = new Timer(SaveDataToFiles, null,
                TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5));

            _logger.LogInformation("Enhanced DataService initialized with {ServiceCount} services and {LogCount} logs",
                _services.Count, _logs.Count);
        }

        #region Public Methods

        public async Task<IEnumerable<ServiceDto>> GetServicesAsync()
        {
            await Task.Delay(200); // محاكاة تأخير قاعدة البيانات

            // تحديث حالة الخدمات بشكل عشوائي
            UpdateServiceStatuses();

            return _services.OrderBy(s => s.Name);
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

            // تطبيق الفلاتر
            if (!string.IsNullOrEmpty(request.Level))
                query = query.Where(l => l.Level == request.Level);

            if (!string.IsNullOrEmpty(request.Source))
                query = query.Where(l => l.Source.Contains(request.Source, StringComparison.OrdinalIgnoreCase));

            if (request.FromDate.HasValue)
                query = query.Where(l => l.Timestamp >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(l => l.Timestamp <= request.ToDate.Value);

            var totalCount = query.Count();

            // ترتيب وصفحات
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

        #endregion

        #region Data Generation

        private List<ServiceDto> GenerateRealisticServices()
        {
            return new List<ServiceDto>
            {
                new() { Id = 1, Name = "🌐 Web Server (Nginx)", Status = ApiConstants.ServiceStatuses.Active, Description = "Frontend web server handling HTTP requests", CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new() { Id = 2, Name = "🚪 API Gateway", Status = ApiConstants.ServiceStatuses.Active, Description = "Main API gateway routing requests", CreatedAt = DateTime.UtcNow.AddDays(-25) },
                new() { Id = 3, Name = "🔐 Authentication Service", Status = ApiConstants.ServiceStatuses.Active, Description = "User authentication and authorization", CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new() { Id = 4, Name = "📁 File Upload Service", Status = ApiConstants.ServiceStatuses.Maintenance, Description = "File upload and storage handling", CreatedAt = DateTime.UtcNow.AddDays(-15) },
                new() { Id = 5, Name = "📧 Email Service", Status = ApiConstants.ServiceStatuses.Error, Description = "Email notifications and messaging", CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new() { Id = 6, Name = "⚙️ Background Jobs", Status = ApiConstants.ServiceStatuses.Active, Description = "Scheduled background tasks", CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new() { Id = 7, Name = "📊 Analytics Service", Status = ApiConstants.ServiceStatuses.Active, Description = "Data analytics and reporting", CreatedAt = DateTime.UtcNow.AddDays(-3) },
                new() { Id = 8, Name = "💾 Cache Service (Redis)", Status = ApiConstants.ServiceStatuses.Active, Description = "In-memory caching system", CreatedAt = DateTime.UtcNow.AddDays(-2) }
            };
        }

        private void AddRandomRealisticLog(object? state)
        {
            try
            {
                var service = _services[_random.Next(_services.Count)];
                var logLevel = GetRandomLogLevel();
                var message = GetRandomMessage(logLevel, service.Name);

                var log = new LogDto
                {
                    Id = GetNextLogId(),
                    Timestamp = DateTime.UtcNow,
                    Level = logLevel,
                    Message = message,
                    Source = service.Name.Replace("🌐 ", "").Replace("🚪 ", "").Replace("🔐 ", "")
                        .Replace("📁 ", "").Replace("📧 ", "").Replace("⚙️ ", "").Replace("📊 ", "").Replace("💾 ", ""),
                    Exception = logLevel == ApiConstants.LogLevels.Error && _random.Next(3) == 0
                        ? GenerateRealisticException()
                        : null
                };

                lock (_logs)
                {
                    _logs.Add(log);

                    // الاحتفاظ بآخر 1000 log فقط
                    if (_logs.Count > 1000)
                    {
                        _logs.RemoveAt(0);
                    }
                }

                _logger.LogDebug("Generated new log: {Level} - {Message}", log.Level, log.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating random log");
            }
        }

        private int GetNextLogId()
        {
            lock (_logs)
            {
                return _logs.Any() ? _logs.Max(l => l.Id) + 1 : 1;
            }
        }

        private string GetRandomLogLevel()
        {
            // توزيع واقعي للـ log levels
            var levels = new[]
            {
                ApiConstants.LogLevels.Info, ApiConstants.LogLevels.Info, ApiConstants.LogLevels.Info, ApiConstants.LogLevels.Info, // 40% Info
                ApiConstants.LogLevels.Success, ApiConstants.LogLevels.Success, // 20% Success
                ApiConstants.LogLevels.Warning, ApiConstants.LogLevels.Warning, // 20% Warning
                ApiConstants.LogLevels.Error, // 10% Error
                ApiConstants.LogLevels.Debug // 10% Debug
            };
            return levels[_random.Next(levels.Length)];
        }

        private string GetRandomMessage(string level, string serviceName)
        {
            var cleanServiceName = serviceName.Split(' ')[^1]; // آخر كلمة من اسم الخدمة

            var messages = level switch
            {
                ApiConstants.LogLevels.Info => new[]
                {
                    $"{cleanServiceName}: Request processed successfully in {_random.Next(10, 500)}ms",
                    $"{cleanServiceName}: User session established",
                    $"{cleanServiceName}: Configuration reloaded",
                    $"{cleanServiceName}: Health check passed - CPU: {_random.Next(10, 80)}%",
                    $"{cleanServiceName}: Connection pool size: {_random.Next(5, 50)}",
                    $"{cleanServiceName}: Processing {_random.Next(1, 100)} pending requests"
                },
                ApiConstants.LogLevels.Warning => new[]
                {
                    $"{cleanServiceName}: High memory usage detected - {_random.Next(70, 95)}% used",
                    $"{cleanServiceName}: Slow response time: {_random.Next(1000, 5000)}ms",
                    $"{cleanServiceName}: Rate limit approaching - {_random.Next(80, 95)}% of quota used",
                    $"{cleanServiceName}: Disk space running low - {_random.Next(10, 25)}% free",
                    $"{cleanServiceName}: Connection pool exhaustion warning",
                    $"{cleanServiceName}: Queue depth increasing: {_random.Next(50, 200)} items"
                },
                ApiConstants.LogLevels.Error => new[]
                {
                    $"{cleanServiceName}: Database connection failed - timeout after {_random.Next(5, 30)}s",
                    $"{cleanServiceName}: Request timeout - client disconnected",
                    $"{cleanServiceName}: Invalid authentication token",
                    $"{cleanServiceName}: File not found: /uploads/{Guid.NewGuid()}.tmp",
                    $"{cleanServiceName}: Network unreachable - retrying in {_random.Next(30, 300)}s",
                    $"{cleanServiceName}: Payment processing failed - gateway error"
                },
                ApiConstants.LogLevels.Success => new[]
                {
                    $"{cleanServiceName}: Backup completed successfully - {_random.Next(100, 1000)}MB processed",
                    $"{cleanServiceName}: Update installed and verified",
                    $"{cleanServiceName}: Maintenance window completed",
                    $"{cleanServiceName}: Data migration successful - {_random.Next(1000, 50000)} records",
                    $"{cleanServiceName}: Certificate renewal completed",
                    $"{cleanServiceName}: Failover test passed"
                },
                ApiConstants.LogLevels.Debug => new[]
                {
                    $"{cleanServiceName}: Cache hit ratio: {_random.Next(70, 99)}%",
                    $"{cleanServiceName}: SQL query executed in {_random.Next(1, 50)}ms",
                    $"{cleanServiceName}: Thread pool stats: {_random.Next(1, 20)} active threads",
                    $"{cleanServiceName}: GC collected {_random.Next(100, 5000)}KB in Gen{_random.Next(0, 3)}",
                    $"{cleanServiceName}: WebSocket connection established from {GenerateRandomIP()}"
                },
                _ => new[] { $"{cleanServiceName}: General system message" }
            };

            return messages[_random.Next(messages.Length)];
        }

        private string GenerateRealisticException()
        {
            var exceptions = new[]
            {
                "System.TimeoutException: The operation has timed out",
                "System.Net.Http.HttpRequestException: Unable to connect to the remote server",
                "System.Data.SqlClient.SqlException: Cannot open database requested by the login",
                "System.UnauthorizedAccessException: Access to the path is denied",
                "System.ArgumentNullException: Value cannot be null. Parameter name: connectionString",
                "System.InvalidOperationException: The connection is already open",
                "System.OutOfMemoryException: Insufficient memory to continue the execution",
                "System.IO.FileNotFoundException: Could not find file or assembly"
            };

            return exceptions[_random.Next(exceptions.Length)];
        }

        private string GenerateRandomIP()
        {
            return $"{_random.Next(192, 255)}.{_random.Next(1, 255)}.{_random.Next(1, 255)}.{_random.Next(1, 255)}";
        }

        #endregion

        #region Service Status Updates

        private void UpdateServiceStatuses()
        {
            // تحديث حالة الخدمات بشكل عشوائي
            foreach (var service in _services)
            {
                if (_random.Next(100) < 5) // 5% احتمال تغيير الحالة
                {
                    service.Status = GetRandomServiceStatus(service.Status);
                    service.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        private string GetRandomServiceStatus(string currentStatus)
        {
            var statuses = new[]
            {
                ApiConstants.ServiceStatuses.Active,
                ApiConstants.ServiceStatuses.Maintenance,
                ApiConstants.ServiceStatuses.Error
            };

            // تجنب تكرار نفس الحالة
            var availableStatuses = statuses.Where(s => s != currentStatus).ToArray();
            return availableStatuses[_random.Next(availableStatuses.Length)];
        }

        #endregion

        #region File Operations

        private List<ServiceDto>? LoadServicesFromFile()
        {
            try
            {
                var filePath = Path.Combine(_dataFolder, "services.json");
                if (!File.Exists(filePath))
                    return null;

                var json = File.ReadAllText(filePath);

                // 🔧 Fixed: Use same serialization options for both save and load
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var services = JsonSerializer.Deserialize<List<ServiceDto>>(json, options);

                // 🔧 Validate loaded data
                if (services?.Any() == true && services.All(s => !string.IsNullOrEmpty(s.Name)))
                {
                    _logger.LogInformation("Loaded {Count} services from file successfully", services.Count);
                    return services;
                }

                // If data is corrupted, delete file and regenerate
                _logger.LogWarning("Loaded services data appears corrupted, deleting file and regenerating...");
                File.Delete(filePath);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load services from file, using defaults");
                return null;
            }
        }

        private List<LogDto>? LoadLogsFromFile()
        {
            try
            {
                var filePath = Path.Combine(_dataFolder, "logs.json");
                if (!File.Exists(filePath))
                    return null;

                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<LogDto>>(json);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load logs from file, starting fresh");
                return new List<LogDto>();
            }
        }

        private async void SaveDataToFiles(object? state)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                // حفظ الخدمات
                var servicesJson = JsonSerializer.Serialize(_services, options);
                await File.WriteAllTextAsync(Path.Combine(_dataFolder, "services.json"), servicesJson);

                // حفظ اللوجز (آخر 1000 فقط)
                List<LogDto> logsToSave;
                lock (_logs)
                {
                    logsToSave = _logs.TakeLast(1000).ToList();
                }

                var logsJson = JsonSerializer.Serialize(logsToSave, options);
                await File.WriteAllTextAsync(Path.Combine(_dataFolder, "logs.json"), logsJson);

                // حفظ إحصائيات
                var stats = new
                {
                    LastSaved = DateTime.UtcNow,
                    ServicesCount = _services.Count,
                    LogsCount = _logs.Count,
                    ServiceStatuses = _services.GroupBy(s => s.Status)
                        .ToDictionary(g => g.Key, g => g.Count())
                };

                var statsJson = JsonSerializer.Serialize(stats, options);
                await File.WriteAllTextAsync(Path.Combine(_dataFolder, "stats.json"), statsJson);

                _logger.LogInformation("Data saved successfully - {ServicesCount} services, {LogsCount} logs",
                    _services.Count, _logs.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save data to files");
            }
        }

        #endregion

        #region Cleanup

        public void Dispose()
        {
            _updateTimer?.Dispose();
            _saveTimer?.Dispose();

            // حفظ نهائي عند الإغلاق
            SaveDataToFiles(null);
        }

        #endregion
    }
}