using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;
public partial class WorkoutPage : ContentPage
{
    public WorkoutPage(WorkoutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}   