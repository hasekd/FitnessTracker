using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;
public partial class AddWorkoutPage : ContentPage
{
	public AddWorkoutPage(AddWorkoutViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}