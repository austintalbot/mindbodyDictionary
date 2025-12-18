namespace MindBodyDictionaryMobile;

using CommunityToolkit.Maui;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MindBodyDictionaryMobile.Services.billing;
using MindBodyDictionaryMobile.Services;
using MindBodyDictionaryMobile.Models;
using MindBodyDictionaryMobile.PageModels;
using MindBodyDictionaryMobile.Pages;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;

public static class MauiProgram
{
  public static IServiceProvider Services { get; private set; }

  public static MauiApp CreateMauiApp() {
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF1cXGtCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH1fc3ZURmJeVER+WERWYEg=");
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .UseMauiCommunityToolkit()
        .ConfigureSyncfusionToolkit()
        .ConfigureSyncfusionCore()
        .ConfigureMauiHandlers(handlers => {
          // Removed AdMob handler registration
        })
        .ConfigureFonts(fonts => {
          fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
          fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
          fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
          fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
        });

#if DEBUG
    builder.Logging.AddDebug();
    builder.Services.AddLogging(configure => configure.AddDebug());
#endif

    builder.Services.AddSingleton<AppShell>();
    builder.Services.AddSingleton<ProjectRepository>();
    builder.Services.AddSingleton<TaskRepository>();
    builder.Services.AddSingleton<CategoryRepository>();
    builder.Services.AddSingleton<TagRepository>();
    builder.Services.AddSingleton<MbdConditionRepository>();
    builder.Services.AddSingleton<UserListRepository>();
    builder.Services.AddSingleton<ImageCacheRepository>();
    builder.Services.AddSingleton<ImageCacheService>();
    builder.Services.AddSingleton<IImageCacheHelper, ImageCacheHelper>();
    builder.Services.AddSingleton<SeedDataService>();
    builder.Services.AddSingleton<MbdConditionApiService>();
    builder.Services.AddSingleton<AppDataPreloaderService>();
    builder.Services.AddSingleton<ModalErrorHandler>();
    builder.Services.AddSingleton<MainPageModel>();
    builder.Services.AddSingleton<ProjectListPageModel>();
    builder.Services.AddSingleton<ManageMetaPageModel>();
    builder.Services.AddTransient<IBillingService, BillingService>();

#if ANDROID
    builder.Services.AddSingleton<IDeviceInstallationService, Platforms.Android.DeviceInstallationService>();
#elif IOS
		var iosDeviceService = new Platforms.iOS.DeviceInstallationService();
		builder.Services.AddSingleton<IDeviceInstallationService>(iosDeviceService);
		builder.Services.AddSingleton(iosDeviceService);
#endif

    builder.Services.AddSingleton<INotificationActionServiceExtended, NotificationActionService>();

    // Direct Azure Notification Hub registration (no backend API)
    builder.Services.AddSingleton<INotificationRegistrationService>(s => new NotificationRegistrationService(
        s.GetRequiredService<ILogger<NotificationRegistrationService>>(),
        s.GetRequiredService<IDeviceInstallationService>()
    ));

    builder.Services.AddTransient<NotificationSettingsPageModel>();
    builder.Services.AddTransient<NotificationSettingsPage>();

#if DEBUG
    builder.Services.AddTransient<ImageCachePageModel>();
    builder.Services.AddTransient<ImageCachePage>();
#endif

    builder.Services.AddSingleton<UpgradePremiumPageModel>();
    builder.Services.AddSingleton<UpgradePremiumPage>();

    builder.Services.AddSingleton<AboutPageModel>();
    builder.Services.AddSingleton<AboutPage>();

    builder.Services.AddSingleton<FaqApiService>();
    builder.Services.AddSingleton<FaqPageModel>();
    builder.Services.AddSingleton<FaqPage>();

    builder.Services.AddSingleton<MovementLinkApiService>();
    builder.Services.AddSingleton<MovementLinksPageModel>();
    builder.Services.AddSingleton<MovementLinksPage>();

    builder.Services.AddTransientWithShellRoute<ProjectDetailPage, ProjectDetailPageModel>("project");
    builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");
    builder.Services.AddTransientWithShellRoute<MbdConditionDetailPage, MbdConditionDetailPageModel>("mbdcondition");

    // Register New Condition Pages and ViewModels
    builder.Services.AddSingleton<MbdConditionHomePageModel>();
    builder.Services.AddSingleton<MbdConditionHomePage>();
    builder.Services.AddSingleton<MbdConditionListPageModel>();
    builder.Services.AddSingleton<MbdConditionListPage>();

    builder.Services.AddSingleton<SearchPageModel>();
    builder.Services.AddSingleton<SearchPage>();

    builder.Services.AddTransientWithShellRoute<MbdConditionSummaryPage, MbdConditionSummaryPageModel>(nameof(MbdConditionSummaryPage));

    // Register DisclaimerPopup
    builder.Services.AddTransient<DisclaimerPopup>();

    // Register Sub-Views for Condition Details
    builder.Services.AddTransient<MbdConditionDetailsProblemView>();
    builder.Services.AddTransient<MbdConditionDetailsAffirmationsView>();
    builder.Services.AddTransient<MbdConditionDetailsFoodView>();
    builder.Services.AddTransient<MbdConditionDetailsProductsView>();
    builder.Services.AddTransient<MbdConditionDetailsResourcesView>();

    var app = builder.Build();
    var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    logger.LogInformation("App startup log test");
    Services = app.Services;

    // Load images into cache on startup
    var imageCacheService = Services.GetRequiredService<ImageCacheService>();
    imageCacheService.LoadImagesFromResourcesAsync().FireAndForgetSafeAsync(app.Services.GetRequiredService<ModalErrorHandler>());

    return app;
  }
}
