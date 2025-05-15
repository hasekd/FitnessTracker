using Microsoft.Extensions.Logging;
using FitnessTracker.Services;
using FitnessTracker.ViewModels;
using FitnessTracker.Views;
using CommunityToolkit.Maui;
using Syncfusion.Maui.Core.Hosting;

namespace FitnessTracker;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionCore()
            .ConfigureFonts(fonts =>
			{
                fonts.AddFont("InterVariable.ttf", "Inter");
            });
#if DEBUG
        builder.Logging.AddDebug();
#endif
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "fitness_tracker_app.db");

        // Register services
        builder.Services.AddSingleton(new DatabaseService(dbPath));

        // Register view models
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<AddWorkoutViewModel>();
        builder.Services.AddTransient<EditWorkoutViewModel>();
        builder.Services.AddTransient<AddCardioViewModel>();
        builder.Services.AddTransient<WorkoutViewModel>();
        builder.Services.AddTransient<CardioViewModel>();
        builder.Services.AddTransient<EditCardioViewModel>();

        // Register pages
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<AddWorkoutPage>();
        builder.Services.AddTransient<EditWorkoutPage>();
        builder.Services.AddTransient<AddCardioPage>();
        builder.Services.AddTransient<WorkoutPage>();
        builder.Services.AddTransient<CardioPage>();
        builder.Services.AddTransient<EditCardioPage>();

        return builder.Build();
	}
}
