using Microsoft.Extensions.DependencyInjection;
using MindBodyDictionaryMobile.Services;

namespace MindBodyDictionaryMobile;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
	}

	protected override void OnStart()
	{
		base.OnStart();
		System.Diagnostics.Debug.WriteLine("=== App.OnStart: Kicking off background data preloader ===");
		var preloader = MauiProgram.Services.GetService<AppDataPreloaderService>();
		preloader?.PreloadData();
	}

	protected override Window CreateWindow(IActivationState? activationState) => new(MainPage);
}
