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
      Android.Util.Log.Error("Firebase", "FirebaseApp.InitializeApp returned null! Check google-services.json");
    }
    else
    {
      Android.Util.Log.Info("Firebase", "Firebase initialized successfully");
    }

    base.OnCreate();
  }
}
