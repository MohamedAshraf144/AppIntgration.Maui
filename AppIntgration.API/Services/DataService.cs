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
            await Task.Delay(200); // محاكاة تأخير قاعدة البيانات
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

            // تطبيق الفلاتر
            if (!string.IsNullOrEmpty(request.Level))
                query = query.Where(l => l.Level == request.Level);

            if (!string.IsNullOrEmpty(request.Source))
                query = query.Where(l => l.Source.Contains(request.Source));

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

        private List<ServiceDto> GenerateMockServices()
        {
            return new List<ServiceDto>
        {
            new() { Id = 1, Name = "خدمة الويب الرئيسية", Status = AppIntgration.Shard.Constants.ApiConstants.ServiceStatuses.Active, Description = "خدمة API الأساسية للتطبيق", CreatedAt = DateTime.UtcNow.AddDays(-30) },
            new() { Id = 2, Name = "قاعدة البيانات", Status = AppIntgration.Shard.Constants.ApiConstants.ServiceStatuses.Active, Description = "قاعدة البيانات المركزية", CreatedAt = DateTime.UtcNow.AddDays(-45) },
            new() { Id = 3, Name = "خدمة التخزين", Status = AppIntgration.Shard.Constants.ApiConstants.ServiceStatuses.Maintenance, Description = "تخزين الملفات والوثائق", CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new() { Id = 4, Name = "خدمة الإشعارات", Status = AppIntgration.Shard.Constants.ApiConstants.ServiceStatuses.Active, Description = "إرسال الإشعارات للمستخدمين", CreatedAt = DateTime.UtcNow.AddDays(-15) },
            new() { Id = 5, Name = "خدمة النسخ الاحتياطي", Status = AppIntgration.Shard.Constants.ApiConstants.ServiceStatuses.Error, Description = "نسخ احتياطي للبيانات", CreatedAt = DateTime.UtcNow.AddDays(-10) }
        };
        }

        private List<LogDto> GenerateMockLogs()
        {
            var random = new Random();
            var levels = new[] { AppIntgration.Shard.Constants.ApiConstants.LogLevels.Info, AppIntgration.Shard.Constants.ApiConstants.LogLevels.Warning, AppIntgration.Shard.Constants.ApiConstants.LogLevels.Error, AppIntgration.Shard.Constants.ApiConstants.LogLevels.Success };
            var sources = new[] { "تطبيق الويب", "قاعدة البيانات", "خدمة API", "نظام المصادقة", "خدمة التخزين" };
            var messages = new[]
            {
            "تم تسجيل دخول مستخدم جديد بنجاح",
            "فشل في الاتصال بقاعدة البيانات",
            "تم إنشاء نسخة احتياطية بنجاح",
            "تحذير: استهلاك ذاكرة عالي",
            "تم حفظ البيانات بنجاح",
            "خطأ في معالجة الطلب",
            "تم تحديث إعدادات النظام",
            "فشل في إرسال الإشعار",
            "تم تنفيذ عملية الصيانة",
            "تحذير: مساحة التخزين منخفضة"
        };

            return Enumerable.Range(1, 500).Select(i => new LogDto
            {
                Id = i,
                Timestamp = DateTime.UtcNow.AddMinutes(-random.Next(1, 10080)), // آخر أسبوع
                Level = levels[random.Next(levels.Length)],
                Message = messages[random.Next(messages.Length)],
                Source = sources[random.Next(sources.Length)],
                Exception = random.Next(10) < 2 ? "System.Exception: خطأ تفصيلي هنا" : null
            }).ToList();
        }
    }
}
