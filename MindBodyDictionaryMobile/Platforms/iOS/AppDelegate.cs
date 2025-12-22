namespace MindBodyDictionaryMobile;

using Foundation;
using Microsoft.Maui.ApplicationModel;
using UIKit;
using UserNotifications;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate, IUNUserNotificationCenterDelegate
{
  protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

  public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions) {
    // Set this AppDelegate as the notification center delegate
    UNUserNotificationCenter.Current.Delegate = this;

    // Handle notification launched from background
    if (launchOptions?.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey) == true)
    {
      if (launchOptions[UIApplication.LaunchOptionsRemoteNotificationKey] is NSDictionary remoteNotification)
      {
        System.Diagnostics.Debug.WriteLine("=== App launched from remote notification ===");
        ProcessNotification(remoteNotification);
      }
    }

    // Handle local notification launched from background
    if (launchOptions?.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey) == true)
    {
      if (launchOptions[UIApplication.LaunchOptionsLocalNotificationKey] is UILocalNotification localNotification)
      {
        System.Diagnostics.Debug.WriteLine("=== App launched from local notification ===");
        ProcessNotification(localNotification.UserInfo);
      }
    }

    return base.FinishedLaunching(application, launchOptions);
  }

  [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
  public void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken) {
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
        if (service is Platforms.iOS.DeviceInstallationService iosService)
        {
          iosService.SetDeviceToken(token);
          System.Diagnostics.Debug.WriteLine("APNS Token successfully set in DeviceInstallationService");

          // Trigger a re-registration with the new token
          var registrationService = currentApp.Services.GetService<INotificationRegistrationService>();
          if (registrationService != null)
          {
            _ = Task.Run(async () => {
              try
              {
                await registrationService.RegisterDeviceAsync();
                System.Diagnostics.Debug.WriteLine("iOS: Device re-registered with new token successfully.");
              }
              catch (Exception ex)
              {
                System.Diagnostics.Debug.WriteLine($"iOS: Failed to re-register with new token: {ex.Message}");
              }
            });
          }
        }
        else
        {
          System.Diagnostics.Debug.WriteLine($"Warning: DeviceInstallationService not found or wrong type. Device token will not be registered.");
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"Error setting APNS token: {ex.Message}");
      }
    }
  }

  [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
  public void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error) {
    System.Diagnostics.Debug.WriteLine($"Failed to register for remote notifications: {error}");
  }

  // MARK: UNUserNotificationCenterDelegate Methods

  [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
  public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler) {
    System.Diagnostics.Debug.WriteLine("=== WillPresentNotification called ===");

    var userInfo = notification.Request.Content.UserInfo;

    System.Diagnostics.Debug.WriteLine($"Notification title: {notification.Request.Content.Title}");
    System.Diagnostics.Debug.WriteLine($"Notification body: {notification.Request.Content.Body}");
    System.Diagnostics.Debug.WriteLine($"UserInfo count: {userInfo.Count}");

    foreach (var key in userInfo.Keys)
    {
      System.Diagnostics.Debug.WriteLine($"  {key} = {userInfo[key]}");
    }

    try
    {
      ProcessNotification(userInfo);
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Error processing notification: {ex.Message}");
    }

    // Show notification in foreground with all options
    completionHandler(UNNotificationPresentationOptions.List | UNNotificationPresentationOptions.Banner | UNNotificationPresentationOptions.Sound | UNNotificationPresentationOptions.Badge);
  }

  [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
  public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler) {
    System.Diagnostics.Debug.WriteLine("=== DidReceiveNotificationResponse called ===");

    var userInfo = response.Notification.Request.Content.UserInfo;

    System.Diagnostics.Debug.WriteLine($"Notification title: {response.Notification.Request.Content.Title}");
    System.Diagnostics.Debug.WriteLine($"Notification body: {response.Notification.Request.Content.Body}");
    System.Diagnostics.Debug.WriteLine($"UserInfo count: {userInfo.Count}");

    foreach (var key in userInfo.Keys)
    {
      System.Diagnostics.Debug.WriteLine($"  {key} = {userInfo[key]}");
    }

    try
    {
      ProcessNotification(userInfo);
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Error processing notification response: {ex.Message}");
    }

    completionHandler();
  }

  private void ProcessNotification(NSDictionary userInfo) {
    System.Diagnostics.Debug.WriteLine("=== ProcessNotification called ===");

    try
    {
      // Check for deep link first
      NSString? deepLinkKey = (NSString?)"deep_link";
      if (userInfo.TryGetValue(deepLinkKey, out var deepLinkValue))
      {
        var deepLink = deepLinkValue.ToString();
        if (!string.IsNullOrEmpty(deepLink))
        {
          System.Diagnostics.Debug.WriteLine($"Found deep link in notification: {deepLink}");
          Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(async () => {
            await Shell.Current.GoToAsync($"//{deepLink}");
            System.Diagnostics.Debug.WriteLine($"Navigated to: //{deepLink}");
          });
          return; // If deep link is handled, no need to process other actions for now
        }
      }
      else
      {
        System.Diagnostics.Debug.WriteLine("No 'deep_link' key found in notification userInfo");
      }

      // If no deep link, proceed with existing action handling (if any)
      var currentApp = IPlatformApplication.Current;
      if (currentApp == null)
      {
        System.Diagnostics.Debug.WriteLine("Warning: IPlatformApplication.Current is null");
        return;
      }

      var notificationActionService = currentApp.Services.GetService<INotificationActionServiceExtended>();
      if (notificationActionService == null)
      {
        System.Diagnostics.Debug.WriteLine("Warning: INotificationActionServiceExtended not found in services");
        return;
      }

      // Look for action key in userInfo
      NSString? actionKey = (NSString?)"action";
      if (userInfo.TryGetValue(actionKey, out var actionValue))
      {
        var action = actionValue.ToString();
        System.Diagnostics.Debug.WriteLine($"Found action in notification: {action}");
        notificationActionService.TriggerAction(action);
      }
      else
      {
        System.Diagnostics.Debug.WriteLine("No 'action' key found in notification userInfo");
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Error in ProcessNotification: {ex.Message}");
      System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
    }
  }
}
