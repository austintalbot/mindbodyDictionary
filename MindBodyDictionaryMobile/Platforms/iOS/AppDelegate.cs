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
		
		System.Diagnostics.Debug.WriteLine($"APNS Token Received: {token}");
		
		// Set the token in the service
		var currentApp = IPlatformApplication.Current;
		if (currentApp != null)
		{
			try
			{
				var service = currentApp.Services.GetService<IDeviceInstallationService>();
				if (service is not null)
				{
					if (service is Platforms.iOS.DeviceInstallationService iosService)
					{
						iosService.Token = token;
						System.Diagnostics.Debug.WriteLine("APNS Token successfully set in DeviceInstallationService");
					}
					else
					{
						System.Diagnostics.Debug.WriteLine($"Warning: Service is not iOS DeviceInstallationService, it's {service.GetType().Name}");
					}
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("Warning: DeviceInstallationService not found. Device token will not be registered, and push notifications may not work.");
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error setting APNS token: {ex.Message}");
			}
		}
		else
		{
			System.Diagnostics.Debug.WriteLine("Warning: IPlatformApplication.Current is null. Device token will not be registered, and push notifications may not work.");
		}
	}

	[Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
	public void DidFailToRegisterForRemoteNotifications(UIKit.UIApplication application, NSError error)
	{
		// Handle registration failure
		System.Diagnostics.Debug.WriteLine($"Failed to register for remote notifications: {error}");
	}
}
