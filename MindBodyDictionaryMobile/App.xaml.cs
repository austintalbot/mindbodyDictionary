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
		// Don't seed here - seed only when the user navigates to the Conditions page
		System.Diagnostics.Debug.WriteLine("=== App.OnStart: Deferring seed data loading ===");
	}

	protected override Window CreateWindow(IActivationState? activationState) => new(new AppShell());
}
