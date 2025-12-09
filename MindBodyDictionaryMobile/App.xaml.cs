using Microsoft.Extensions.DependencyInjection;
using MindBodyDictionaryMobile.Data;
using MindBodyDictionaryMobile.Models;
using System.Text.Json;

namespace MindBodyDictionaryMobile;

public partial class App : Application
{
	public static bool IsSeedingComplete { get; private set; } = false;

	public App()
	{
		InitializeComponent();
	}

	protected override void OnStart()
	{
		base.OnStart();
		// Start seeding conditions in the background on app startup
		System.Diagnostics.Debug.WriteLine("=== App.OnStart: Seeding conditions in background ===");
		Task.Run(async () =>
		{
			try
			{
				var seedService = MauiProgram.Services.GetService<SeedDataService>();
				if (seedService != null)
				{
					await seedService.SeedConditionsAsync();
					System.Diagnostics.Debug.WriteLine("=== App.OnStart: Condition seeding complete ===");
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("=== App.OnStart: SeedDataService not found ===");
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"=== App.OnStart: Condition seeding error: {ex.Message} ===");
			}
		});
	}

	protected override Window CreateWindow(IActivationState? activationState) => new(new AppShell());
}
