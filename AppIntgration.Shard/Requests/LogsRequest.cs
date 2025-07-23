using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppIntgration.Shard.Requests
{
    public class LogsRequest
    {
        public string? ServiceName { get; set; }
        public string? LogLevel { get; set; }
        public string? Level { get; set; }
        public string? Source { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
    }
}
