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
    public partial class WorkoutViewModel : ObservableObject, IRecipient<WorkoutChangedMessage>
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private ObservableCollection<Workout> workouts;

        [ObservableProperty]
        private ObservableCollection<string> exercises =
        new ObservableCollection<string>
        {
            "All",
            "Pull-ups",
            "Push-ups",
            "Dips",
            "Squats",
            "Plank",
            "Leg Raises",
            "Handstand Hold"
        };

        [ObservableProperty]
        private string selectedExerciseFilter;
        [ObservableProperty]
        private ObservableCollection<Workout> filteredWorkouts;
        [ObservableProperty]
        private DateTime? fromDate;
        [ObservableProperty]
        private DateTime? toDate;

        private bool _suppressDateValidation = false;

        public WorkoutViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            WeakReferenceMessenger.Default.Register(this);
            LoadWorkouts();
        }

        [RelayCommand]
        private async Task LoadWorkouts()
        {
            var all = await _databaseService.GetWorkoutsAsync();
            var filtered = all
                .Where(w => w.Type.ToLower().Contains("workout"))
                .OrderByDescending(w => w.Date)
                .ToList();
            Workouts = new ObservableCollection<Workout>(filtered);
            ApplyFilter();
        }

        public async void Receive(WorkoutChangedMessage message)
        {
            await LoadWorkouts();
        }

        [RelayCommand]
        private async Task Add()
        {
            await Shell.Current.GoToAsync(nameof(AddWorkoutPage));
        }

        [RelayCommand]
        private async Task Edit(Workout workout)
        {
            if (workout == null) return;
            var route = $"{nameof(EditWorkoutPage)}?WorkoutId={workout.Id}";
            await Shell.Current.GoToAsync(route);
        }

        [RelayCommand]
        private async Task Delete(Workout workout)
        {
            if (workout == null) return;

            bool confirm = await Shell.Current.DisplayAlert("Delete", "Delete this workout?", "Yes", "No");
            if (!confirm) return;

            await _databaseService.DeleteWorkoutAsync(workout);
            WeakReferenceMessenger.Default.Send(new WorkoutChangedMessage());
            await Toast.Make("Workout deleted.", ToastDuration.Short).Show();
            await LoadWorkouts();
        }

        partial void OnSelectedExerciseFilterChanged(string value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = Workouts.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SelectedExerciseFilter) && SelectedExerciseFilter != "All")
            {
                filtered = filtered.Where(w => w.Exercise == SelectedExerciseFilter);
            }

            if (FromDate != null)
            {
                filtered = filtered.Where(w => w.Date >= FromDate.Value);
            }

            if (ToDate != null)
            {
                filtered = filtered.Where(w => w.Date <= ToDate.Value);
            }

            FilteredWorkouts = new ObservableCollection<Workout>(filtered);
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
            _suppressDateValidation = true;

            SelectedExerciseFilter = "All";

            if (Workouts == null || Workouts.Count == 0)
            {
                FromDate = null;
                ToDate = null;
            }
            else
            {
                var minDate = Workouts.Min(w => w.Date);
                var maxDate = Workouts.Max(w => w.Date);

                FromDate = minDate.Date;
                ToDate = maxDate.Date;
            }

            _suppressDateValidation = false;

            ApplyFilter();
        }

    }
}