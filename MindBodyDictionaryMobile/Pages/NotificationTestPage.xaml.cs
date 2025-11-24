namespace MindBodyDictionaryMobile.Pages;

public partial class NotificationTestPage : ContentPage
{
public NotificationTestPage()
{
InitializeComponent();
}

private async void OnTestLocalNotificationClicked(object sender, EventArgs e)
{
await Services.LocalNotificationService.SendTestNotification();
await DisplayAlert("Success", "Local notification sent", "OK");
}
}
