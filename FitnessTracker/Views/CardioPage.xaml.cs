using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;
public partial class CardioPage : ContentPage
{
    public CardioPage(CardioViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}