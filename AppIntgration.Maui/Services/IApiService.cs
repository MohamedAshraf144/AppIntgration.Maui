using AppIntgration.Shard.DTOs;
using AppIntgration.Shard.Responses;
using AppIntgration.Shared.Models;
using Elastic.Apm.Api;
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

        // ⚠️ Set the server address here
        private const string API_BASE_URL = "https://localhost:7001"; // Change this address

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
                var response = await _httpClient.GetAsync(AppIntgration.Shard.Constants.ApiConstants.Endpoints.GetApiKey);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiKeyResponse>(jsonContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return apiResponse?.Success == true ? apiResponse.Data : null;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving key: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<ServiceDto>> GetServicesAsync(string apiKey)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                var response = await _httpClient.GetAsync(AppIntgration.Shard.Constants.ApiConstants.Endpoints.GetServices);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<IEnumerable<ServiceDto>>>(jsonContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return apiResponse?.Success == true && apiResponse.Data != null
                        ? apiResponse.Data
                        : new List<ServiceDto>();
                }

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

                var response = await _httpClient.GetAsync($"{AppIntgration.Shard.Constants.ApiConstants.Endpoints.GetLogs}?page=1&size=50");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<PaginatedResponse<LogDto>>(jsonContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return apiResponse?.Success == true && apiResponse.Data != null
                        ? apiResponse.Data
                        : new List<LogDto>();
                }

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
