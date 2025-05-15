using SQLite;
using FitnessTracker.Models;
using System.Diagnostics;

namespace FitnessTracker.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);

            _database.CreateTableAsync<Workout>().Wait();
            _database.CreateTableAsync<Cardio>().Wait();
        }

        //Workout
        public Task<List<Workout>> GetWorkoutsAsync() => _database.Table<Workout>().ToListAsync();

        public Task<int> SaveWorkoutAsync(Workout workout) => _database.InsertAsync(workout);

        public Task<int> DeleteWorkoutAsync(Workout workout) => _database.DeleteAsync(workout);

        public Task<int> UpdateWorkoutAsync(Workout workout) => _database.UpdateAsync(workout);

        //Cardio
        public Task<List<Cardio>> GetCardioSessionsAsync() => _database.Table<Cardio>().ToListAsync();

        public Task<int> SaveCardioAsync(Cardio workout) => _database.InsertAsync(workout);

        public Task<int> DeleteCardioAsync(Cardio workout) => _database.DeleteAsync(workout);

        public Task<int> UpdateCardioAsync(Cardio workout) => _database.UpdateAsync(workout);
    }
}
