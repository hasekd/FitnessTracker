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
            ApplyExerciseFilter();
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
            await Toast.Make("Workout deleted.", ToastDuration.Short).Show();
            await LoadWorkouts();
        }

        partial void OnSelectedExerciseFilterChanged(string value)
        {
            ApplyExerciseFilter();
        }

        private void ApplyExerciseFilter()
        {
            if (string.IsNullOrWhiteSpace(SelectedExerciseFilter) || SelectedExerciseFilter == "All")
            {
                FilteredWorkouts = [.. Workouts];
            }
            else
            {
                FilteredWorkouts = [.. Workouts.Where(w => w.Exercise == SelectedExerciseFilter)];
            }
        }

    }
}