using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppIntgration.Shard.Responses
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? ErrorCode { get; set; }
    }

    public class ApiKeyResponse : ApiResponse<string>
    {
        public DateTime? ExpiresAt { get; set; }
    }

    public class PaginatedResponse<T> : ApiResponse<IEnumerable<T>>
    {
        public PaginationInfo? Pagination { get; set; }
    }

    public class PaginationInfo
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalPages { get; set; }
        public long TotalItems { get; set; }
    }
}
