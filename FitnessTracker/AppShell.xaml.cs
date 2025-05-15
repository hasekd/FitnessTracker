using FitnessTracker.Views;

namespace FitnessTracker
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AddWorkoutPage), typeof(AddWorkoutPage));
            Routing.RegisterRoute(nameof(EditWorkoutPage), typeof(EditWorkoutPage));
            Routing.RegisterRoute(nameof(AddCardioPage), typeof(AddCardioPage));
            Routing.RegisterRoute(nameof(EditCardioPage), typeof(EditCardioPage));
        }
    }
}
