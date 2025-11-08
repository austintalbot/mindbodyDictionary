using Android.App;
using Android.Content.PM;
using Android.OS;
using Firebase.Messaging;

namespace MindBodyDictionaryMobile;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public const string CHANNEL_ID = "mindbody_notifications";
    
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        CreateNotificationChannel();
        RequestFirebaseToken();
    }
    
    void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(
                CHANNEL_ID,
                "MindBody Notifications",
                NotificationImportance.Default)
            {
                Description = "Push notifications for MindBody Dictionary"
            };
            
            var notificationManager = GetSystemService(NotificationService) as NotificationManager;
            notificationManager?.CreateNotificationChannel(channel);
        }
    }
    
    void RequestFirebaseToken()
    {
        try
        {
            global::Android.Util.Log.Info("FCM", "Requesting Firebase token...");
            
            FirebaseMessaging.Instance.GetToken().AddOnCompleteListener(new TokenCompleteListener(this));
        }
        catch (Exception ex)
        {
            global::Android.Util.Log.Error("FCM", $"Error requesting FCM token: {ex.Message}");
        }
    }
    
    class TokenCompleteListener : Java.Lang.Object, Android.Gms.Tasks.IOnCompleteListener
    {
        readonly MainActivity _activity;
        
        public TokenCompleteListener(MainActivity activity)
        {
            _activity = activity;
        }
        
        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            if (!task.IsSuccessful)
            {
                global::Android.Util.Log.Error("FCM", $"Token request failed: {task.Exception?.Message}");
                return;
            }
            
            var token = task.Result?.ToString();
            if (string.IsNullOrEmpty(token))
            {
                global::Android.Util.Log.Error("FCM", "Token is null or empty");
                return;
            }
            
            global::Android.Util.Log.Info("FCM", $"Token received: {token.Substring(0, Math.Min(20, token.Length))}...");
            
            // Store token in DeviceInstallationService
            var deviceInstallationService = IPlatformApplication.Current?.Services?.GetService<IDeviceInstallationService>();
            if (deviceInstallationService is Platforms.Android.DeviceInstallationService androidService)
            {
                androidService.Token = token;
                global::Android.Util.Log.Info("FCM", "Token stored in DeviceInstallationService");
            }
            else
            {
                global::Android.Util.Log.Warn("FCM", "Could not get DeviceInstallationService");
            }
        }
    }
}
