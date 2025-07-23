using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppIntgration.Shard.DTOs
{
    public class ApiKeyDto
    {
        public string Key { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string ClientId { get; set; } = string.Empty;
    }

}
