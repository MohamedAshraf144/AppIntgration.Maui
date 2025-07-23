using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppIntgration.Shard.DTOs
{
    public class ApiKeyRequest
    {
        public string ClientId { get; set; } = "MauiClient";
        public string? ClientSecret { get; set; }
    }
}
