using FitnessTracker.Models;
using FitnessTracker.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace FitnessTracker.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty] private Cardio latestCardio;
        [ObservableProperty] private Workout latestWorkout;

        public HomeViewModel(DatabaseService dbService)
        {
            _databaseService = dbService;
            LoadLatest();
        }

        public async Task LoadLatest()
        {
            if (_databaseService == null)
            {
                Debug.WriteLine("DatabaseService is null");
                return;
            }

            var allWorkouts = await _databaseService.GetWorkoutsAsync();
            var allCardioSessions = await _databaseService.GetCardioSessionsAsync();

            if (allWorkouts == null || allWorkouts.Count == 0 || allCardioSessions == null || allCardioSessions.Count == 0)
                return;

            LatestCardio = allCardioSessions
                .Where(c => !string.IsNullOrWhiteSpace(c.Type) && c.Type.ToLower().Contains("cardio"))
                .OrderByDescending(c => c.Date)
                .FirstOrDefault();

            LatestWorkout = allWorkouts
                .Where(w => !string.IsNullOrWhiteSpace(w.Type) && w.Type.ToLower().Contains("workout"))
                .OrderByDescending(w => w.Date)
                .FirstOrDefault();
        }

    }
}