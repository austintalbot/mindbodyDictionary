using CommunityToolkit.Maui;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using MindBodyDictionaryMobile.Services.billing;
using Plugin.AdMob; // Add this

namespace MindBodyDictionaryMobile;

public static class MauiProgram
{
	public static IServiceProvider Services { get; private set; }

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureSyncfusionToolkit()
            .ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddHandler(typeof(BannerAd), typeof(Plugin.AdMob.Handlers.BannerAdHandler));
#endif
            })
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
				fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
			});

#if DEBUG
		builder.Logging.AddDebug();
		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

		builder.Services.AddSingleton<ProjectRepository>();
		builder.Services.AddSingleton<TaskRepository>();
		builder.Services.AddSingleton<CategoryRepository>();
		builder.Services.AddSingleton<TagRepository>();
		builder.Services.AddSingleton<ConditionRepository>();
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
		builder.Services.AddSingleton<INotificationRegistrationService, NotificationRegistrationService>();

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

		builder.Services.AddTransientWithShellRoute<ProjectDetailPage, ProjectDetailPageModel>("project");
		builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");
		builder.Services.AddTransientWithShellRoute<ConditionDetailPage, ConditionDetailPageModel>("condition");
		builder.Services.AddSingleton<ConditionListPageModel>();
		builder.Services.AddSingleton<ConditionListPage>();

        // Register New Condition Pages and ViewModels
        builder.Services.AddSingleton<ConditionHomePageModel>();
        builder.Services.AddSingleton<ConditionHomePage>();

        builder.Services.AddTransient<ConditionSearchPageModel>();
        builder.Services.AddTransient<ConditionSearchPage>();

        builder.Services.AddTransientWithShellRoute<ConditionSummaryPage, ConditionSummaryPageModel>(nameof(ConditionSummaryPage));

        // Register DisclaimerPopup
        builder.Services.AddTransient<DisclaimerPopup>();

        // Register Sub-Views for Condition Details
        builder.Services.AddTransient<ConditionDetailsProblemView>();
        builder.Services.AddTransient<ConditionDetailsAffirmationsView>();
        builder.Services.AddTransient<RecommendationsView>();
        builder.Services.AddTransient<ConditionDetailsFoodView>();
        builder.Services.AddTransient<ConditionDetailsProductsView>();
        builder.Services.AddTransient<ConditionDetailsResourcesView>();

        // Register PageModels for Sub-Views
        builder.Services.AddTransient<RecommendationsPageModel>();

		var app = builder.Build();
		Services = app.Services;
		return app;
	}
}
