using AppIntgration.Shard.Responses;
using System.Security.Cryptography;
using System.Text;


namespace AppIntgration.Services;

public class AuthService : IAuthService
{
    private readonly Dictionary<string, DateTime> _apiKeys = new();
    private readonly byte[] _aesKey = Encoding.UTF8.GetBytes("ThisIsASecretKeyForAES1234567890!!"); // 32 bytes for AES-256
    private readonly byte[] _aesIV = Encoding.UTF8.GetBytes("ThisIsASecretIV!!"); // 16 bytes for AES

    public async Task<ApiKeyResponse> GenerateApiKeyAsync()
    {
        await Task.Delay(100); // Simulate async operation

        var apiKey = GenerateRandomKey();
        var encryptedKey = EncryptApiKey(apiKey);
        var expiresAt = DateTime.UtcNow.AddHours(24);

        // Save the encrypted key with expiration
        _apiKeys[encryptedKey] = expiresAt;

        // Clean expired keys
        CleanExpiredKeys();

        return new ApiKeyResponse
        {
            Data = encryptedKey,
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

    private string EncryptApiKey(string key)
    {
        using var aes = Aes.Create();
        aes.Key = _aesKey;
        aes.IV = _aesIV;
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(key);
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    private string DecryptApiKey(string encryptedKey)
    {
        using var aes = Aes.Create();
        aes.Key = _aesKey;
        aes.IV = _aesIV;
        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var buffer = Convert.FromBase64String(encryptedKey);
        using var ms = new MemoryStream(buffer);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
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