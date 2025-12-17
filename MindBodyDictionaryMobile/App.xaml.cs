namespace MindBodyDictionaryMobile;

using Microsoft.Extensions.DependencyInjection;
using MindBodyDictionaryMobile.Services;

public partial class App : Application
{
  public static string CopyrightText { get; } = "© Mind Body Dictionary, LLC";

  public App() {
    InitializeComponent();
  }

  protected override void OnStart() {
    base.OnStart();
    System.Diagnostics.Debug.WriteLine("=== App.OnStart: Kicking off background data preloader ===");
    var preloader = MauiProgram.Services.GetService<AppDataPreloaderService>();
    preloader?.PreloadData();

    // Optimize search page loading by pre-fetching data in the background
    var searchPageModel = MauiProgram.Services.GetService<MindBodyDictionaryMobile.PageModels.SearchPageModel>();
    Task.Run(async () => {
      try
      {
        if (searchPageModel != null)
        {
          await searchPageModel.GetConditionShortList();
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"[App.OnStart] Error pre-loading search data: {ex.Message}");
      }
    });
  }

  protected override Window CreateWindow(IActivationState? activationState) => new(MauiProgram.Services.GetRequiredService<AppShell>());
}
