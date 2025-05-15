using SQLite;

namespace FitnessTracker.Models
{
    public class Cardio
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Duration { get; set; }
        public string AvgHeartRate { get; set; }
        public string Distance { get; set; }
    }
}
