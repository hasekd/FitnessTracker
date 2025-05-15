using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;
public partial class AddCardioPage : ContentPage
{
	public AddCardioPage(AddCardioViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}