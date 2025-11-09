using Foundation;
using UserNotifications;
using Microsoft.Maui.ApplicationModel;

namespace MindBodyDictionaryMobile;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	public override bool FinishedLaunching(UIKit.UIApplication application, NSDictionary? launchOptions)
	{
		// Request notification permissions
		UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, (approved, err) =>
		{
			if (approved)
			{
				// Must be on main UI thread
				Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
				{
					UIKit.UIApplication.SharedApplication.RegisterForRemoteNotifications();
				});
			}
		});

		return base.FinishedLaunching(application, launchOptions);
	}

	[Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
	public void DidRegisterForRemoteNotifications(UIKit.UIApplication application, NSData deviceToken)
	{
		// Convert token to string
		var token = BitConverter.ToString(deviceToken.ToArray()).Replace("-", "").ToLower();
		// Set the token in the service
		var service = IPlatformApplication.Current.Services.GetService<Platforms.iOS.DeviceInstallationService>();
		if (service != null)
		{
			service.Token = token;
		}
	}

	[Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
	public void DidFailToRegisterForRemoteNotifications(UIKit.UIApplication application, NSError error)
	{
		// Handle registration failure
		System.Diagnostics.Debug.WriteLine($"Failed to register for remote notifications: {error}");
	}
}
