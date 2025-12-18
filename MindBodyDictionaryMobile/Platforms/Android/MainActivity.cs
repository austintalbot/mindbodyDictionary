namespace MindBodyDictionaryMobile;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Firebase.Messaging;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(new[] { Android.Content.Intent.ActionView },
    Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
    DataScheme = "mindbodydictionary")]
public class MainActivity : MauiAppCompatActivity
{
  public const string CHANNEL_ID = "mindbody_notifications";
  const int REQUEST_NOTIFICATION_PERMISSION = 1001;

  protected override void OnCreate(Bundle? savedInstanceState) {
    base.OnCreate(savedInstanceState);

    // Register BackPressedCallback
    OnBackPressedDispatcher.AddCallback(this, new Platforms.Android.BackPressedCallback(this));

    CreateNotificationChannel();
    RequestNotificationPermission();

    // Handle deep link when app is launched from a notification
    HandleIntent(Intent);
  }

  protected override void OnNewIntent(Android.Content.Intent? intent) {
    base.OnNewIntent(intent);
    // Handle deep link when app is already running and a new notification arrives
    HandleIntent(intent);
  }

  private void HandleIntent(Android.Content.Intent? intent) {
    if (intent == null)
    {
      Android.Util.Log.Warn("DeepLink", "⚠️ HandleIntent called with null intent.");
      return;
    }

    if (intent.HasExtra("deep_link"))
    {
      var deepLink = intent.GetStringExtra("deep_link");
      if (!string.IsNullOrEmpty(deepLink))
      {
        Android.Util.Log.Info("DeepLink", $"✅ Deep link received: {deepLink}");
        // Use MainThread.BeginInvokeOnMainThread to ensure UI operations are on the main thread
        MainThread.BeginInvokeOnMainThread(async () => {
          // Navigate to the specified deep link route
          // Assuming the deep links are structured as Shell routes (e.g., //ailments/some-id)
          // You might need to adjust the format or add logic to map deep links to actual Shell routes
          await Shell.Current.GoToAsync($"//{deepLink}");
          Android.Util.Log.Info("DeepLink", $"Navigated to: //{deepLink}");
        });
      }
      else
      {
        Android.Util.Log.Warn("DeepLink", "⚠️ Deep link extra found but value is empty.");
      }
    }
    else
    {
      Android.Util.Log.Info("DeepLink", "No deep link extra found in intent.");
    }
  }

  void CreateNotificationChannel() {
    Android.Util.Log.Info("Notifications", "Creating notification channel...");

    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
    {
      var importance = NotificationImportance.High;
      var channel = new NotificationChannel(
          CHANNEL_ID,
          "MindBody Notifications",
          importance)
      {
        Description = "Push notifications for MindBody Dictionary"
      };

      channel.EnableVibration(true);
      channel.EnableLights(true);

      var notificationManager = GetSystemService(NotificationService) as NotificationManager;
      notificationManager?.CreateNotificationChannel(channel);

      Android.Util.Log.Info("Notifications", $"✅ Channel created: {CHANNEL_ID}");
    }
  }

  void RequestNotificationPermission() {
    // Android 13 (API 33) and above requires runtime permission for notifications
    if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
    {
      Android.Util.Log.Info("Permissions", "Checking notification permission (Android 13+)...");

      if (CheckSelfPermission(Android.Manifest.Permission.PostNotifications) != Permission.Granted)
      {
        Android.Util.Log.Info("Permissions", "Requesting POST_NOTIFICATIONS permission...");
        RequestPermissions([Android.Manifest.Permission.PostNotifications], REQUEST_NOTIFICATION_PERMISSION);
      }
      else
      {
        Android.Util.Log.Info("Permissions", "✅ POST_NOTIFICATIONS already granted");
      }
    }
    else
    {
      Android.Util.Log.Info("Permissions", "Notification permission not required (< Android 13)");
    }
  }

  public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults) {
    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

    if (requestCode == REQUEST_NOTIFICATION_PERMISSION)
    {
      if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
      {
        Android.Util.Log.Info("Permissions", "✅ Notification permission granted");
      }
      else
      {
        Android.Util.Log.Warn("Permissions", "⚠️ Notification permission denied");
      }
    }
  }
}
