using AppIntgration.Shard.Responses;

namespace AppIntgration.Services;

public class AuthService : IAuthService
{
    // ✅ خلي القائمة static علشان تفضل موجودة
    private static readonly List<string> _validKeys = new();

    public async Task<ApiKeyResponse> GenerateApiKeyAsync()
    {
        await Task.Delay(50);

        var newKey = $"API_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8]}";

        // إضافة للقائمة الثابتة
        lock (_validKeys)
        {
            _validKeys.Add(newKey);
        }

        return new ApiKeyResponse
        {
            Data = newKey,
            Success = true,
            Message = "API Key generated successfully",
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
    }

    public async Task<bool> ValidateApiKeyAsync(string apiKey)
    {
        await Task.Delay(10);

        if (string.IsNullOrEmpty(apiKey))
            return false;

        lock (_validKeys)
        {
            return _validKeys.Contains(apiKey);
        }
    }
}
