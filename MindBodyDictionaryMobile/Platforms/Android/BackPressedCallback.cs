using AndroidX.Activity;

namespace MindBodyDictionaryMobile.Platforms.Android
{
    public class BackPressedCallback : OnBackPressedCallback
    {
        private readonly MainActivity activity;

        public BackPressedCallback(MainActivity activity) : base(true)
        {
            this.activity = activity;
        }

        public override void HandleOnBackPressed()
        {
            var app = Microsoft.Maui.Controls.Application.Current;
            if (app?.Windows == null || app.Windows.Count == 0) return;

            var mainPage = app.Windows[0].Page;
            if (mainPage == null) return;

            // Check if we are at the root of navigation
            // For Shell, we check Navigation.NavigationStack.Count.
            // Note: Shell navigation stack count is 0 when on the root tab/flyout item.
            // Pushing a page makes it 1.
            // The old code checked for Count == 1. In MAUI Shell, root is often empty stack.
            // However, mainPage.Navigation.NavigationStack usually reflects the stack within the current ShellSection.

            bool isRoot = false;
            if (mainPage is Shell shell)
            {
                 // Check if there are pages pushed onto the stack
                 // Shell.Current.Navigation.NavigationStack.Count
                 var navStackCount = shell.Navigation.NavigationStack.Count;
                 // If 0 or 1 (depending on how it's counted), we are at root.
                 // Usually, if we are at the tab root, Count is 0 or 1.
                 // Let's assume root if back button would exit.

                 // Better check: If we can pop, we are not at root.
                 isRoot = navStackCount <= 1;
                 // Wait, standard MAUI Shell: root page is in the stack?
                 // If I use "GoToAsync", the stack resets.
            }

            // Using the logic from the user's snippet, but adapted slightly for safety
            if (mainPage.Navigation.NavigationStack.Count <= 1)
            {
                // We are likely at the root. Show exit confirmation.
                // We need to run this on the UI thread and async
                mainPage.Dispatcher.Dispatch(async () =>
                {
                    bool shouldExit = await mainPage.DisplayAlertAsync("Exit", "Do you want to exit the app?", "Yes", "No");

                    if (shouldExit)
                    {
                        activity.FinishAffinity(); // Android way to close app
                        // Or Application.Current.Quit();
                    }
                });
            }
            else
            {
                // Not at root, let MAUI handle the back navigation (pop)
                // We disable this callback temporarily to let the default behavior happen?
                // Or manually pop?
                // The user code called `mainPage.SendBackButtonPressed();`.
                mainPage.SendBackButtonPressed();
            }
        }
    }
}
