using AppIntgration.Shard.DTOs;
using AppIntgration.Shard.Responses;
using AppIntgration.Shared.Models;
using System.Text.Json;
using AppIntgration.Shard.Constants;

namespace AppIntgration.Maui.Services
{
    public interface IApiService
    {
        Task<string?> GetApiKeyAsync();
        Task<IEnumerable<ServiceDto>> GetServicesAsync(string apiKey);
        Task<IEnumerable<LogDto>> GetLogsAsync(string apiKey);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        // ✅ Fixed: Correct API URL
        private const string API_BASE_URL = "https://localhost:7039"; // Fixed port number!
        //private const string API_BASE_URL = "http://192.168.1.113:5176";


        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(API_BASE_URL),
                Timeout = TimeSpan.FromSeconds(30)
            };

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "MauiRestApiApp/1.0");
        }

        public async Task<string?> GetApiKeyAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiConstants.Endpoints.GetApiKey);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiKeyResponse>(jsonContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return apiResponse?.Success == true ? apiResponse.Data : null;
                }

                // 🔍 Add debugging info
                System.Diagnostics.Debug.WriteLine($"API Key request failed: {response.StatusCode}");
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Error content: {errorContent}");

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving key: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"API URL: {API_BASE_URL}");
                return null;
            }
        }

        public async Task<IEnumerable<ServiceDto>> GetServicesAsync(string apiKey)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                var response = await _httpClient.GetAsync(ApiConstants.Endpoints.GetServices);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<IEnumerable<ServiceDto>>>(jsonContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return apiResponse?.Success == true && apiResponse.Data != null
                        ? apiResponse.Data
                        : new List<ServiceDto>();
                }

                System.Diagnostics.Debug.WriteLine($"Services request failed: {response.StatusCode}");
                return new List<ServiceDto>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching services: {ex.Message}");
                return new List<ServiceDto>();
            }
        }

        public async Task<IEnumerable<LogDto>> GetLogsAsync(string apiKey)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                var response = await _httpClient.GetAsync($"{ApiConstants.Endpoints.GetLogs}?page=1&size=50");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<PaginatedResponse<LogDto>>(jsonContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return apiResponse?.Success == true && apiResponse.Data != null
                        ? apiResponse.Data
                        : new List<LogDto>();
                }

                System.Diagnostics.Debug.WriteLine($"Logs request failed: {response.StatusCode}");
                return new List<LogDto>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching logs: {ex.Message}");
                return new List<LogDto>();
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}