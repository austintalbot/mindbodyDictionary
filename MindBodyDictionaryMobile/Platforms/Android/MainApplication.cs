namespace MindBodyDictionaryMobile;

using Android.App;
using Android.Runtime;

[Application]
public class MainApplication(IntPtr handle, JniHandleOwnership ownership) : MauiApplication(handle, ownership)
{
  protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

  public override void OnCreate() {
    // Initialize Firebase before MAUI init
    var firebaseApp = Firebase.FirebaseApp.InitializeApp(this);
    
    if (firebaseApp == null)
    {
      Android.Util.Log.Warn("Firebase", "Automatic initialization failed. Attempting manual initialization...");
      
      // Fallback: Manually initialize with values from google-services.json
      // This protects against build issues where the json isn't processed correctly
      var options = new Firebase.FirebaseOptions.Builder()
          .SetApplicationId("1:533561443811:android:b7ba0d1964d9e3720b8cd6")
          .SetApiKey("AIzaSyBMMY3RlAb51yD3-N6jnRSOH1YANkwV6jk")
          .SetProjectId("mindbody-dictionary")
          .SetStorageBucket("mindbody-dictionary.firebasestorage.app")
          .Build();

      try 
      {
        firebaseApp = Firebase.FirebaseApp.InitializeApp(this, options);
        Android.Util.Log.Info("Firebase", "Manual initialization successful");
      }
      catch (Exception ex)
      {
        Android.Util.Log.Error("Firebase", $"Manual initialization failed: {ex.Message}");
      }
    }
    else
    {
      Android.Util.Log.Info("Firebase", "Firebase initialized successfully (Automatic)");
    }

    base.OnCreate();
  }
}
