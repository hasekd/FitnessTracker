using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using FitnessTracker.Models;
using FitnessTracker.Services;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using FitnessTracker.Messages;

namespace FitnessTracker.ViewModels
{
    public partial class HomeViewModel : ObservableObject,
    IRecipient<WorkoutChangedMessage>,
    IRecipient<CardioChangedMessage>
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty] private Cardio latestCardio;
        [ObservableProperty] private Workout latestWorkout;
        [ObservableProperty]
        private ObservableCollection<ChartData> workoutVolumeOverTimePoints = new();
        [ObservableProperty]
        private ObservableCollection<string> exerciseList = new();
        [ObservableProperty]
        private string selectedExercise;
        [ObservableProperty]
        private ObservableCollection<ChartData> selectedExerciseVolumePoints = new();
        [ObservableProperty]
        private ObservableCollection<string> cardioMetrics = new()
        {
            "Distance",
            "Duration",
            "AvgHeartRate"
        };
        [ObservableProperty]
        private string selectedMetric;

        [ObservableProperty]
        private ObservableCollection<ChartData> cardioMetricPoints = new();

        public HomeViewModel(DatabaseService dbService)
        {
            _databaseService = dbService;
            WeakReferenceMessenger.Default.Register<WorkoutChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<CardioChangedMessage>(this);
            LoadLatest();
            LoadCharts();
        }

        public void Receive(WorkoutChangedMessage message)
        {
            LoadLatest();
            LoadCharts();
            if (!string.IsNullOrWhiteSpace(SelectedExercise))
                GenerateExerciseVolumeChart(SelectedExercise);
        }

        public void Receive(CardioChangedMessage message)
        {
            LoadLatest();
            LoadCharts();
        }

        public async Task LoadLatest()
        {
            if (_databaseService == null)
            {
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
        public async void LoadCharts()
        {
            var workouts = await _databaseService.GetWorkoutsAsync();

            WorkoutVolumeOverTimePoints = new ObservableCollection<ChartData>(
            workouts
                .Where(w => int.TryParse(w.Reps, out _) && int.TryParse(w.Sets, out _))
                .GroupBy(w => w.Date.Date)
                .OrderBy(g => g.Key)
                .Select(g => new ChartData(
                    g.Key.ToString("MM/dd"),
                    g.Sum(w =>
                        (int.TryParse(w.Sets, out var sets) ? sets : 1) *
                        (int.TryParse(w.Reps, out var reps) ? reps : 0)
                    )
                )));

            ExerciseList = new ObservableCollection<string>(
            workouts
                .Select(w => w.Exercise)
                .Where(desc => !string.IsNullOrWhiteSpace(desc))
                .Distinct()
                .OrderBy(d => d));

            GenerateCardioMetricChart(SelectedMetric);
        }

        partial void OnSelectedExerciseChanged(string value)
        {
            GenerateExerciseVolumeChart(value);
        }

        private async void GenerateExerciseVolumeChart(string exercise)
        {
            var workouts = await _databaseService.GetWorkoutsAsync();

            if (string.IsNullOrWhiteSpace(exercise)) return;

            SelectedExerciseVolumePoints = new ObservableCollection<ChartData>(
                workouts
                    .Where(w => w.Exercise == exercise
                                && int.TryParse(w.Reps, out _)
                                && int.TryParse(w.Sets, out _))
                    .GroupBy(w => w.Date.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new ChartData(
                        g.Key.ToString("MM/dd"),
                        g.Sum(w =>
                            (int.TryParse(w.Sets, out var sets) ? sets : 1) *
                            (int.TryParse(w.Reps, out var reps) ? reps : 0)
                        )
                    )));
        }
        partial void OnSelectedMetricChanged(string value)
        {
            GenerateCardioMetricChart(value);
        }

        private async void GenerateCardioMetricChart(string metric)
        {
            var cardioSessions = await _databaseService.GetCardioSessionsAsync();

            if (cardioSessions == null || cardioSessions.Count == 0)
            {
                CardioMetricPoints = new();
                return;
            }

            var ordered = cardioSessions
                .OrderBy(c => c.Date)
                .Select(c =>
                {
                    double y = 0;

                    switch (metric)
                    {
                        case "Distance":
                            y = double.TryParse(c.Distance, out var distance) ? distance : 0;
                            break;
                        case "Duration":
                            y = TryParseMinutes(c.Duration);
                            break;
                        case "AvgHeartRate":
                            y = double.TryParse(c.AvgHeartRate, out var avgHeartRate) ? avgHeartRate : 0;
                            break;
                    }

                    return new ChartData(c.Date.ToString("MM/dd"), y);
                });

            CardioMetricPoints = new ObservableCollection<ChartData>(ordered);
        }


        private double TryParseMinutes(string? duration)
        {
            if (string.IsNullOrWhiteSpace(duration)) return 0;
            if (TimeSpan.TryParse(duration, out var ts)) return ts.TotalMinutes;

            if (double.TryParse(duration.Split(' ')[0], out var val))
                return val;

            return 0;
        }


    }
}