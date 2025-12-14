namespace MindBodyDictionaryMobile.Pages;

public partial class ImageCachePage : ContentPage
{
  private bool _hasLoadedOnce = false;

  public ImageCachePage(ImageCachePageModel model) {
    InitializeComponent();
    System.Diagnostics.Debug.WriteLine("ImageCachePage: Constructor called");
    BindingContext = model;
    System.Diagnostics.Debug.WriteLine($"ImageCachePage: BindingContext set to {model?.GetType().Name}");
  }

  protected override void OnAppearing() {
    base.OnAppearing();
    System.Diagnostics.Debug.WriteLine("ImageCachePage: OnAppearing called");

    if (!_hasLoadedOnce && BindingContext is ImageCachePageModel model)
    {
      _hasLoadedOnce = true;
      System.Diagnostics.Debug.WriteLine("ImageCachePage: First load - invoking LoadCacheStats");

      // Fire and forget - the command will update bindings
      _ = MainThread.InvokeOnMainThreadAsync(async () => {
        try
        {
          await model.LoadCacheStatsCommand.ExecuteAsync(null);
          System.Diagnostics.Debug.WriteLine("ImageCachePage: LoadCacheStats completed");
        }
        catch (Exception ex)
        {
          System.Diagnostics.Debug.WriteLine($"ImageCachePage: Error: {ex}");
        }
      });
    }
  }

  protected override void OnDisappearing() {
    base.OnDisappearing();
    System.Diagnostics.Debug.WriteLine("ImageCachePage: OnDisappearing called");
  }
}
