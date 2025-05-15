using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using FitnessTracker.Messages;
using FitnessTracker.Models;
using FitnessTracker.Services;

namespace FitnessTracker.ViewModels
{
    [QueryProperty(nameof(CardioId), "CardioId")]
    public partial class EditCardioViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;        

        [ObservableProperty] private string distance;
        [ObservableProperty] private DateTime date = DateTime.Today;
        [ObservableProperty] private string duration;
        [ObservableProperty] private string avgHeartRate;

        public IRelayCommand SaveCommand { get; }

        public EditCardioViewModel(DatabaseService dbService)
        {
            _databaseService = dbService;
            SaveCommand = new AsyncRelayCommand(UpdateCardio);
        }

        private int cardioId;
        public int CardioId
        {
            get => cardioId;
            set
            {
                SetProperty(ref cardioId, value);
                _ = LoadCardioAsync(value);
            }
        }

        private async Task LoadCardioAsync(int id)
        {
            var all = await _databaseService.GetCardioSessionsAsync();
            var cardio = all.FirstOrDefault(c => c.Id == id);
            if (cardio == null) return;

            Distance = cardio.Distance;
            Date = cardio.Date;
            Duration = cardio.Duration;
            AvgHeartRate = cardio.AvgHeartRate;
        }

        private async Task UpdateCardio()
        {
            var updatedCardio = new Cardio
            {
                Id = CardioId,
                Type = "Cardio",
                Distance = Distance,
                Date = Date,
                Duration = Duration,
                AvgHeartRate = AvgHeartRate
            };

            await _databaseService.UpdateCardioAsync(updatedCardio);
            WeakReferenceMessenger.Default.Send(new CardioChangedMessage());
            await Toast.Make("Cardio has been edited.", ToastDuration.Short).Show();
            await Shell.Current.GoToAsync("..");
        }
    }
}