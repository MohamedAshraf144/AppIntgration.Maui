using AppIntgration.Shard.Responses;


namespace AppIntgration.Services;

public class AuthService : IAuthService
{
    private readonly Dictionary<string, DateTime> _apiKeys = new();

    public async Task<ApiKeyResponse> GenerateApiKeyAsync()
    {
        await Task.Delay(100); // محاكاة عملية غير متزامنة

        var apiKey = GenerateRandomKey();
        var expiresAt = DateTime.UtcNow.AddHours(24);

        // حفظ المفتاح مع تاريخ انتهاء الصلاحية
        _apiKeys[apiKey] = expiresAt;

        // تنظيف المفاتيح المنتهية الصلاحية
        CleanExpiredKeys();

        return new ApiKeyResponse
        {
            Data = apiKey,
            Success = true,
            Message = "Key generated successfully",
            ExpiresAt = expiresAt
        };
    }

    public async Task<bool> ValidateApiKeyAsync(string apiKey)
    {
        await Task.Delay(50);

        if (string.IsNullOrEmpty(apiKey) || !_apiKeys.ContainsKey(apiKey))
            return false;

        var expiresAt = _apiKeys[apiKey];
        if (DateTime.UtcNow > expiresAt)
        {
            _apiKeys.Remove(apiKey);
            return false;
        }

        return true;
    }

    private string GenerateRandomKey()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 32)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private void CleanExpiredKeys()
    {
        var expiredKeys = _apiKeys
            .Where(kv => DateTime.UtcNow > kv.Value)
            .Select(kv => kv.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _apiKeys.Remove(key);
        }
    }
}