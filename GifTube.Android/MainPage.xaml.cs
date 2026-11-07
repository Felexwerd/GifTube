using Microsoft.Maui.Controls;

namespace GifTube.Android;

public partial class MainPage : ContentPage
{
    public MainPage() => InitializeComponent();

    private async void OnEnableKeyboard(object sender, EventArgs e)
    {
#if ANDROID
        try
        {
            var intent = new global::Android.Content.Intent(global::Android.Provider.Settings.ActionInputMethodSettings);
            intent.AddFlags(global::Android.Content.ActivityFlags.NewTask);
            global::Android.App.Application.Context.StartActivity(intent);
        }
        catch (Exception ex) { await DisplayAlertAsync("GifTube", ex.Message, "OK"); }
#else
        await DisplayAlertAsync("GifTube", "Эта настройка доступна на Android.", "OK");
#endif
    }

    private async void OnSelectKeyboard(object sender, EventArgs e)
    {
#if ANDROID
        try
        {
            var imm = (global::Android.Views.InputMethods.InputMethodManager)global::Android.App.Application.Context.GetSystemService(global::Android.Content.Context.InputMethodService)!;
            imm.ShowInputMethodPicker();
        }
        catch (Exception ex) { await DisplayAlertAsync("GifTube", ex.Message, "OK"); }
#else
        await DisplayAlertAsync("GifTube", "Эта настройка доступна на Android.", "OK");
#endif
    }
}
