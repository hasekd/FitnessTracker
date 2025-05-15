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
using Syncfusion.Maui.Charts;

namespace FitnessTracker.ViewModels
{
    public partial class CardioViewModel : ObservableObject, IRecipient<CardioChangedMessage>
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private ObservableCollection<Cardio> cardioSessions;
        [ObservableProperty]
        private ObservableCollection<Cardio> filteredCardioSessions;
        [ObservableProperty]
        private DateTime? fromDate;
        [ObservableProperty]
        private DateTime? toDate;

        private bool _suppressDateValidation = false;

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
            ApplyFilter();
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
            WeakReferenceMessenger.Default.Send(new CardioChangedMessage());
            await Toast.Make("Cardio deleted.", ToastDuration.Short).Show();
            await LoadCardioSessions();
        }

        private void ApplyFilter()
        {
            var filtered = CardioSessions.AsEnumerable();

            if (FromDate != null)
            {
                filtered = filtered.Where(c => c.Date >= FromDate.Value);
            }

            if (ToDate != null)
            {
                filtered = filtered.Where(c => c.Date <= ToDate.Value);
            }

            FilteredCardioSessions = new ObservableCollection<Cardio>(filtered);
        }

        partial void OnFromDateChanged(DateTime? value)
        {
            if (_suppressDateValidation)
                return;

            if (FromDate == null && ToDate != null)
            {
                _suppressDateValidation = true;
                FromDate = ToDate.Value.AddDays(-1);
                _suppressDateValidation = false;
                return;
            }

            if (FromDate != null && ToDate != null && FromDate > ToDate)
            {
                _suppressDateValidation = true;
                ToDate = FromDate;
                _suppressDateValidation = false;

                Toast.Make("Start date cannot be after end date.", ToastDuration.Short).Show();
            }

            ApplyFilter();

        }

        partial void OnToDateChanged(DateTime? value)
        {
            if (_suppressDateValidation)
                return;

            if (ToDate == null && FromDate != null)
            {
                _suppressDateValidation = true;
                ToDate = FromDate.Value.AddDays(1);
                _suppressDateValidation = false;
                return;
            }

            if (FromDate != null && ToDate != null && ToDate < FromDate)
            {
                _suppressDateValidation = true;
                ToDate = FromDate;
                _suppressDateValidation = false;

                Toast.Make("End date cannot be before start date.", ToastDuration.Short).Show();
            }

            ApplyFilter();

        }

        [RelayCommand]
        private void ClearFilters()
        {
            if (CardioSessions == null || CardioSessions.Count == 0)
            {
                FromDate = null;
                ToDate = null;
            }
            else
            {
                var min = CardioSessions.Min(c => c.Date);
                var max = CardioSessions.Max(c => c.Date);

                _suppressDateValidation = true;
                FromDate = min.Date;
                ToDate = max.Date;
                _suppressDateValidation = false;
            }

            ApplyFilter();
        }
    }
}
