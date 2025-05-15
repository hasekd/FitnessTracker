namespace FitnessTracker;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF1cWWhMYVF0WmFZfVtgdV9CZVZQTWY/P1ZhSXxWdkBiXH5ddHFUT2lfWEx9XUs=");
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}