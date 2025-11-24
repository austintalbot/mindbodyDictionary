using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using MindBodyDictionaryMobile.Services;

namespace MindBodyDictionaryMobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureSyncfusionToolkit()
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
		builder.Services.AddSingleton<ModalErrorHandler>();
		builder.Services.AddSingleton<MainPageModel>();
		builder.Services.AddSingleton<ProjectListPageModel>();
		builder.Services.AddSingleton<ManageMetaPageModel>();

#if ANDROID
		builder.Services.AddSingleton<IDeviceInstallationService, Platforms.Android.DeviceInstallationService>();
		builder.Services.AddSingleton<IBillingService, Platforms.Android.BillingService>();
#elif IOS
		var iosDeviceService = new Platforms.iOS.DeviceInstallationService();
		builder.Services.AddSingleton<IDeviceInstallationService>(iosDeviceService);
		builder.Services.AddSingleton(iosDeviceService);
		builder.Services.AddSingleton<IBillingService, Platforms.iOS.BillingService>();
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

		builder.Services.AddTransientWithShellRoute<ProjectDetailPage, ProjectDetailPageModel>("project");
		builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");
		builder.Services.AddTransientWithShellRoute<ConditionDetailPage, ConditionDetailPageModel>("condition");
		builder.Services.AddSingleton<ConditionListPageModel>();
		
		return builder.Build();
	}
}
