using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;
public partial class EditWorkoutPage : ContentPage
{
	public EditWorkoutPage(EditWorkoutViewModel viewModel)
	{
        InitializeComponent();
        BindingContext = viewModel;
    }
}