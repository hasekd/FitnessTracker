using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using FitnessTracker.Messages;
using FitnessTracker.Models;
using FitnessTracker.Services;
using System.Collections.ObjectModel;

namespace FitnessTracker.ViewModels
{
    public partial class AddWorkoutViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty] private string type;
        [ObservableProperty] private string description;
        [ObservableProperty] private DateTime date = DateTime.Today;
        [ObservableProperty] private string reps;
        [ObservableProperty] private string sets;
        [ObservableProperty] private int avgHeartRate;

        [ObservableProperty]
        private ObservableCollection<string> exercises =
        new ObservableCollection<string>
        {
            "Pull-ups",
            "Push-ups",
            "Dips",
            "Squats",
            "Plank",
            "Leg Raises",
            "Handstand Hold"
        };

        [ObservableProperty] private string selectedExercise;

        public AddWorkoutViewModel(DatabaseService dbService)
        {
            _databaseService = dbService;
        }

        [RelayCommand]
        private async Task Save()
        {
            var workout = new Workout
            {
                Type = "Workout",
                Exercise = SelectedExercise,
                Date = Date,
                Reps = Reps,
                Sets = Sets,
            };

            await _databaseService.SaveWorkoutAsync(workout);
            WeakReferenceMessenger.Default.Send(new WorkoutChangedMessage());
            await Toast.Make("Workout saved.", ToastDuration.Short).Show();
            await Shell.Current.GoToAsync("..");
        }
    }
}