using Microsoft.Extensions.DependencyInjection;
using MindBodyDictionaryMobile.Services;

namespace MindBodyDictionaryMobile;

public partial class App : Application
{
	public static string CopyrightText { get; } = "© Mind Body Dictionary, LLC";

	public App()
	{
		InitializeComponent();
	}

	protected override void OnStart()
	{
		base.OnStart();
		System.Diagnostics.Debug.WriteLine("=== App.OnStart: Kicking off background data preloader ===");
		var preloader = MauiProgram.Services.GetService<AppDataPreloaderService>();
		preloader?.PreloadData();
	}

	protected override Window CreateWindow(IActivationState? activationState) => new(new AppShell());
}
