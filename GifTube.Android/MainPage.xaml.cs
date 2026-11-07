namespace GifTube.Android;
public partial class MainPage : ContentPage
{
    public MainPage() { InitializeComponent(); }
    private async void OnEnableKeyboard(object sender, EventArgs e)
    {
#if ANDROID
        try { Android.Content.Intent intent = new(Android.Provider.Settings.ActionInputMethodSettings); intent.AddFlags(Android.Content.ActivityFlags.NewTask); Android.App.Application.Context.StartActivity(intent); }
        catch (Exception ex) { await DisplayAlert("GifTube", ex.Message, "OK"); }
#else
        await DisplayAlert("GifTube", "Эта настройка доступна на Android.", "OK");
#endif
    }
    private async void OnSelectKeyboard(object sender, EventArgs e)
    {
#if ANDROID
        try { var imm = (Android.Views.InputMethods.InputMethodManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.InputMethodService)!; imm.ShowInputMethodPicker(); }
        catch (Exception ex) { await DisplayAlert("GifTube", ex.Message, "OK"); }
#else
        await DisplayAlert("GifTube", "Эта настройка доступна на Android.", "OK");
#endif
    }
}