using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FitnessTracker.Messages;
using FitnessTracker.Models;
using FitnessTracker.Services;
using System.Collections.ObjectModel;

namespace FitnessTracker.ViewModels
{
    [QueryProperty(nameof(WorkoutId), "WorkoutId")]
    public partial class EditWorkoutViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty] private string type;
        [ObservableProperty] private string exercise;
        [ObservableProperty] private DateTime date = DateTime.Today;
        [ObservableProperty] private string reps;
        [ObservableProperty] private string sets;
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

        public IRelayCommand SaveCommand { get; }

        public EditWorkoutViewModel(DatabaseService dbService)
        {
            _databaseService = dbService;
            SaveCommand = new AsyncRelayCommand(UpdateWorkout);
        }

        private int workoutId;
        public int WorkoutId
        {
            get => workoutId;
            set
            {
                SetProperty(ref workoutId, value);
                _ = LoadWorkoutAsync(value);
            }
        }
        private async Task LoadWorkoutAsync(int id)
        {
            var all = await _databaseService.GetWorkoutsAsync();
            var workout = all.FirstOrDefault(c => c.Id == id);
            if (workout == null) return;

            Exercise = workout.Exercise;
            Date = workout.Date;
            Reps = workout.Reps;
            Sets = workout.Sets;
        }

        private async Task UpdateWorkout()
        {
            var updatedWorkout = new Workout
            {
                Id = WorkoutId,
                Exercise = SelectedExercise ?? Exercise,
                Date = Date,
                Reps = Reps,
                Sets = Sets,
            };

            await _databaseService.UpdateWorkoutAsync(updatedWorkout);
            WeakReferenceMessenger.Default.Send(new WorkoutChangedMessage());
            await Toast.Make("Workout has been edited.", ToastDuration.Short).Show();
            await Shell.Current.GoToAsync("..");
        }
    }
}