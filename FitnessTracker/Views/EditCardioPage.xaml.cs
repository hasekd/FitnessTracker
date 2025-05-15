using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;

public partial class EditCardioPage : ContentPage
{
	public EditCardioPage(EditCardioViewModel viewModel)
	{
        InitializeComponent();
        BindingContext = viewModel;
    }
}