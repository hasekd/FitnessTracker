using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessTracker.Models;
using FitnessTracker.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;
using FitnessTracker.Messages;

namespace FitnessTracker.ViewModels
{
    public partial class AddCardioViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty] private DateTime date = DateTime.Today;
        [ObservableProperty] private string distance;
        [ObservableProperty] private string duration;
        [ObservableProperty] private string avgHeartRate;

        public AddCardioViewModel(DatabaseService dbService)
        {
            _databaseService = dbService;
        }

        [RelayCommand]
        private async Task Save()
        {
            var cardio = new Cardio
            {
                Type = "Cardio",
                Distance = Distance,
                Date = Date,
                Duration = Duration,
                AvgHeartRate = AvgHeartRate
            };

            await _databaseService.SaveCardioAsync(cardio);
            var all = await _databaseService.GetCardioSessionsAsync();
            foreach (var c in all)
            {
                Debug.WriteLine($"Distance saved: {c.Distance}");
            }
            WeakReferenceMessenger.Default.Send(new CardioChangedMessage());
            await Toast.Make("Cardio saved.", ToastDuration.Short).Show();
            await Shell.Current.GoToAsync("..");
        }
    }
}
