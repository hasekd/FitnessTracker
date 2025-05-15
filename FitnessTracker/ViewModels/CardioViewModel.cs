using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using FitnessTracker.Models;
using FitnessTracker.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using FitnessTracker.Views;
using CommunityToolkit.Mvvm.Messaging;
using FitnessTracker.Messages;

namespace FitnessTracker.ViewModels
{
    public partial class CardioViewModel : ObservableObject, IRecipient<CardioChangedMessage>
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private ObservableCollection<Cardio> cardioSessions;

        public CardioViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            WeakReferenceMessenger.Default.Register(this);
            LoadCardioSessions();
        }

        [RelayCommand]
        private async Task LoadCardioSessions()
        {
            var all = await _databaseService.GetCardioSessionsAsync();
            var filtered = all
                .Where(w => w.Type.ToLower().Contains("cardio"))
                .OrderByDescending(w => w.Date)
                .ToList();
            CardioSessions = new ObservableCollection<Cardio>(filtered);
        }

        public async void Receive(CardioChangedMessage message)
        {
            await LoadCardioSessions();
        }

        [RelayCommand]
        private async Task Add()
        {
            await Shell.Current.GoToAsync(nameof(AddCardioPage));
        }

        [RelayCommand]
        private async Task Edit(Cardio cardio)
        {
            if (cardio == null) return;
            var route = $"{nameof(EditCardioPage)}?CardioId={cardio.Id}";
            await Shell.Current.GoToAsync(route);
        }

        [RelayCommand]
        private async Task Delete(Cardio cardio)
        {
            if (cardio == null) return;

            bool confirm = await Shell.Current.DisplayAlert("Delete", "Delete this cardio?", "Yes", "No");
            if (!confirm) return;

            await _databaseService.DeleteCardioAsync(cardio);
            await Toast.Make("Cardio deleted.", ToastDuration.Short).Show();
            await LoadCardioSessions();
        }
    }
}
