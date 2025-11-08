using Android.App;
using Android.Runtime;

namespace MindBodyDictionaryMobile;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
	
	public override void OnCreate()
	{
		base.OnCreate();
		
		// Initialize Firebase
		Firebase.FirebaseApp.InitializeApp(this);
		global::Android.Util.Log.Info("Firebase", "Firebase initialized");
	}
}
