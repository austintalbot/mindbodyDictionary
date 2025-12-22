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

  protected override void OnCreate(Bundle? savedInstanceState) {
    base.OnCreate(savedInstanceState);

    // Register BackPressedCallback
    OnBackPressedDispatcher.AddCallback(this, new Platforms.Android.BackPressedCallback(this));

    CreateNotificationChannel();

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
}
