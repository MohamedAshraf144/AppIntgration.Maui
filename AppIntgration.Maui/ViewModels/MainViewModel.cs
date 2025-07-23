using AppIntgration.Maui.Services;
using AppIntgration.Shard.DTOs;
using AppIntgration.Shared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppIntgration.Maui.ViewModels
{

    public partial class MainViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        private string selectedOption = string.Empty;

        [ObservableProperty]
        private string statusMessage = "Select data type to display";

        [ObservableProperty]
        private string apiKey = string.Empty;

        public ObservableCollection<ServiceDto> Services { get; }
        public ObservableCollection<LogDto> Logs { get; }

        public MainViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Services = new ObservableCollection<ServiceDto>();
            Logs = new ObservableCollection<LogDto>();
            Title = "REST API App";
        }

        [RelayCommand]
        private async Task LoadServicesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                StatusMessage = "Retrieving access key...";
                Services.Clear();

                // Retrieve the key
                var apiKey = await _apiService.GetApiKeyAsync();
                ApiKey = apiKey ?? string.Empty;
                if (string.IsNullOrEmpty(apiKey))
                {
                    StatusMessage = "Failed to retrieve access key";
                    return;
                }

                StatusMessage = "Loading services...";

                // Fetch service data
                var services = await _apiService.GetServicesAsync(apiKey);

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
                StatusMessage = "Retrieving access key...";
                Logs.Clear();

                var apiKey = await _apiService.GetApiKeyAsync();
                ApiKey = apiKey ?? string.Empty;
                if (string.IsNullOrEmpty(apiKey))
                {
                    StatusMessage = "Failed to retrieve access key";
                    return;
                }

                StatusMessage = "Loading logs...";

                var logs = await _apiService.GetLogsAsync(apiKey);

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
