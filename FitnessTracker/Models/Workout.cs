using SQLite;

namespace FitnessTracker.Models
{
    public class Workout
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Exercise { get; set; }
        public DateTime Date { get; set; }
        public string Reps { get; set; }
        public string Sets { get; set; }
    }
}
