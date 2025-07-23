using AppIntgration.Shard.Responses;

namespace AppIntgration.Services;

public interface IAuthService
{
    Task<ApiKeyResponse> GenerateApiKeyAsync();
    Task<bool> ValidateApiKeyAsync(string apiKey);
}