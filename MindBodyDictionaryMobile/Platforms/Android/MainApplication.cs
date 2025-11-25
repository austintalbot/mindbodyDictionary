using Android.App;
using Android.Runtime;

namespace MindBodyDictionaryMobile;

[Application]
public class MainApplication(IntPtr handle, JniHandleOwnership ownership) : MauiApplication(handle, ownership)
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	public override void OnCreate()
	{
		base.OnCreate();

		// Initialize Firebase
		Firebase.FirebaseApp.InitializeApp(this);
        Android.Util.Log.Info("Firebase", "Firebase initialized");
	}
}
