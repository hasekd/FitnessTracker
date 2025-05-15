using FitnessTracker.ViewModels;

namespace FitnessTracker.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage(HomeViewModel viewModel)
        {   
            InitializeComponent();
            BindingContext = viewModel;
        }
    }

}
