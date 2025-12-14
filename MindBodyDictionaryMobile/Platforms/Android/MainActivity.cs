namespace MindBodyDictionaryMobile;

using Android.App;
using Android.Content.PM;
using Android.OS;
using Firebase.Messaging;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
	public const string CHANNEL_ID = "mindbody_notifications";
	const int REQUEST_NOTIFICATION_PERMISSION = 1001;

	protected override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);

        // Register BackPressedCallback
        OnBackPressedDispatcher.AddCallback(this, new Platforms.Android.BackPressedCallback(this));

		CreateNotificationChannel();
		RequestNotificationPermission();
		RequestFirebaseToken();
	}

	void CreateNotificationChannel()
	{
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

	void RequestNotificationPermission()
	{
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

	public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
	{
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

	void RequestFirebaseToken()
	{
		try
		{
			Android.Util.Log.Info("FCM", "Requesting Firebase token...");

			FirebaseMessaging.Instance.GetToken().AddOnCompleteListener(new TokenCompleteListener(this));
		}
		catch (Exception ex)
		{
			Android.Util.Log.Error("FCM", $"Error requesting FCM token: {ex.Message}");
		}
	}

	class TokenCompleteListener(MainActivity activity) : Java.Lang.Object, Android.Gms.Tasks.IOnCompleteListener
	{
		readonly MainActivity _activity = activity;

		public void OnComplete(Android.Gms.Tasks.Task task)
		{
			if (!task.IsSuccessful)
			{
				Android.Util.Log.Error("FCM", $"❌ Token request failed: {task.Exception?.Message}");
				return;
			}

			var token = task.Result?.ToString();
			if (string.IsNullOrEmpty(token))
			{
				Android.Util.Log.Error("FCM", "❌ Token is null or empty");
				return;
			}

			Android.Util.Log.Info("FCM", $"✅ Token received: {token[..Math.Min(20, token.Length)]}...");

			// Store token in DeviceInstallationService
			var deviceInstallationService = IPlatformApplication.Current?.Services?.GetService<IDeviceInstallationService>();
			if (deviceInstallationService is Platforms.Android.DeviceInstallationService androidService)
			{
				androidService.Token = token;
				Android.Util.Log.Info("FCM", "✅ Token stored in DeviceInstallationService");
			}
			else
			{
				Android.Util.Log.Warn("FCM", "⚠️ Could not get DeviceInstallationService");
			}
		}
	}
}
