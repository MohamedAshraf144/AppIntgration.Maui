using AppIntgration.Maui.Services;
using AppIntgration.Shard.DTOs;
using AppIntgration.Shared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppIntgration.Maui.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;

        // Properties موجودة أصلاً
        [ObservableProperty]
        private string selectedOption = string.Empty;

        [ObservableProperty]
        private string statusMessage = "Select data type to display";

        [ObservableProperty]
        private string apiKey = string.Empty;

        // Properties جديدة - تعريف يدوي
        private string _manualApiKey = string.Empty;
        public string ManualApiKey
        {
            get => _manualApiKey;
            set => SetProperty(ref _manualApiKey, value);
        }

        private string _keyStatus = "No key loaded";
        public string KeyStatus
        {
            get => _keyStatus;
            set => SetProperty(ref _keyStatus, value);
        }

        private bool _isKeyValid = false;
        public bool IsKeyValid
        {
            get => _isKeyValid;
            set => SetProperty(ref _isKeyValid, value);
        }

        public ObservableCollection<ServiceDto> Services { get; }
        public ObservableCollection<LogDto> Logs { get; }

        public MainViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Services = new ObservableCollection<ServiceDto>();
            Logs = new ObservableCollection<LogDto>();
            Title = "REST API App";
        }

        // Command لاستخدام المفتاح اليدوي
        [RelayCommand]
        private async Task UseManualKeyAsync()
        {
            if (string.IsNullOrWhiteSpace(ManualApiKey))
            {
                StatusMessage = "Please enter a valid API key";
                return;
            }

            ApiKey = ManualApiKey.Trim();
            IsKeyValid = await ValidateKeyAsync(ApiKey);

            if (IsKeyValid)
            {
                KeyStatus = "✅ Manual key is valid";
                StatusMessage = "Manual API key applied successfully";
            }
            else
            {
                KeyStatus = "❌ Invalid API key";
                StatusMessage = "The entered API key is invalid";
            }
        }

        // Command لتوليد مفتاح جديد
        [RelayCommand]
        private async Task GenerateNewKeyAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                StatusMessage = "Generating new API key...";

                var newKey = await _apiService.GetApiKeyAsync();

                if (!string.IsNullOrEmpty(newKey))
                {
                    ApiKey = newKey;
                    ManualApiKey = newKey; // عشان يظهر في الـ input
                    IsKeyValid = true;
                    KeyStatus = "✅ New key generated successfully";
                    StatusMessage = "New API key generated and ready to use";
                }
                else
                {
                    KeyStatus = "❌ Failed to generate key";
                    StatusMessage = "Failed to generate new API key";
                    IsKeyValid = false;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                KeyStatus = "❌ Generation failed";
                IsKeyValid = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Command لنسخ المفتاح
        [RelayCommand]
        private async Task CopyKeyAsync()
        {
            if (!string.IsNullOrEmpty(ApiKey))
            {
                await Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.SetTextAsync(ApiKey);
                StatusMessage = "API key copied to clipboard!";
            }
        }

        // التحقق من صحة المفتاح
        private async Task<bool> ValidateKeyAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;

            try
            {
                // جرب تجيب بيانات بسيطة للتأكد من صحة المفتاح
                var services = await _apiService.GetServicesAsync(key);
                return services?.Any() == true;
            }
            catch
            {
                return false;
            }
        }

        [RelayCommand]
        private async Task LoadServicesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Services.Clear();

                // استخدم المفتاح الموجود أو جيب واحد جديد
                string currentKey = ApiKey;

                if (string.IsNullOrEmpty(currentKey))
                {
                    StatusMessage = "Retrieving access key...";
                    currentKey = await _apiService.GetApiKeyAsync();

                    if (!string.IsNullOrEmpty(currentKey))
                    {
                        ApiKey = currentKey;
                        ManualApiKey = currentKey;
                        IsKeyValid = true;
                        KeyStatus = "✅ Auto-generated key";
                    }
                }

                if (string.IsNullOrEmpty(currentKey))
                {
                    StatusMessage = "Failed to retrieve access key";
                    KeyStatus = "❌ No valid key";
                    IsKeyValid = false;
                    return;
                }

                StatusMessage = "Loading services...";

                var services = await _apiService.GetServicesAsync(currentKey);

                foreach (var service in services)
                {
                    Services.Add(service);
                }

                StatusMessage = services.Any()
                    ? $"{services.Count()} services loaded successfully"
                    : "No services available";
                SelectedOption = "Services";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                IsKeyValid = false;
                KeyStatus = "❌ Key validation failed";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LoadLogsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Logs.Clear();

                string currentKey = ApiKey;

                if (string.IsNullOrEmpty(currentKey))
                {
                    StatusMessage = "Retrieving access key...";
                    currentKey = await _apiService.GetApiKeyAsync();

                    if (!string.IsNullOrEmpty(currentKey))
                    {
                        ApiKey = currentKey;
                        ManualApiKey = currentKey;
                        IsKeyValid = true;
                        KeyStatus = "✅ Auto-generated key";
                    }
                }

                if (string.IsNullOrEmpty(currentKey))
                {
                    StatusMessage = "Failed to retrieve access key";
                    KeyStatus = "❌ No valid key";
                    IsKeyValid = false;
                    return;
                }

                StatusMessage = "Loading logs...";

                var logs = await _apiService.GetLogsAsync(currentKey);

                foreach (var log in logs)
                {
                    Logs.Add(log);
                }

                StatusMessage = logs.Any()
                    ? $"{logs.Count()} logs loaded successfully"
                    : "No logs available";
                SelectedOption = "Logs";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                IsKeyValid = false;
                KeyStatus = "❌ Key validation failed";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task RefreshDataAsync()
        {
            if (SelectedOption == "Services")
            {
                await LoadServicesAsync();
            }
            else if (SelectedOption == "Logs")
            {
                await LoadLogsAsync();
            }
        }
    }
}