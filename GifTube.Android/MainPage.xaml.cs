using Microsoft.Maui.Controls;

namespace GifTube.Android;

public partial class MainPage : ContentPage
{
    public MainPage() => InitializeComponent();

    private async void OnStartOverlay(object sender, EventArgs e)
    {
#if ANDROID
        try
        {
            if (!global::Android.Provider.Settings.CanDrawOverlays(global::Android.App.Application.Context))
            {
                await DisplayAlert("GifTube", "Сначала разреши отображение поверх других приложений.", "OK");
                await OnOverlaySettingsInternal();
                return;
            }
            var intent = new global::Android.Content.Intent(global::Android.App.Application.Context, typeof(GifTubeOverlayService));
            global::Android.App.Application.Context.StartService(intent);
            await DisplayAlert("GifTube", "Плавающая GIF-панель запущена. Открой YouTube — панель будет поверх него.", "OK");
        }
        catch (Exception ex) { await DisplayAlert("GifTube", ex.Message, "OK"); }
#else
        await DisplayAlert("GifTube", "Панель доступна на Android.", "OK");
#endif
    }

    private async void OnOverlaySettings(object sender, EventArgs e) => await OnOverlaySettingsInternal();

    private Task OnOverlaySettingsInternal()
    {
#if ANDROID
        var context = global::Android.App.Application.Context;
        var intent = new global::Android.Content.Intent(global::Android.Provider.Settings.ActionManageOverlayPermission,
            global::Android.Net.Uri.Parse("package:" + context.PackageName));
        intent.AddFlags(global::Android.Content.ActivityFlags.NewTask);
        context.StartActivity(intent);
#endif
        return Task.CompletedTask;
    }

    private async void OnAccessibilitySettings(object sender, EventArgs e)
    {
#if ANDROID
        try
        {
            var intent = new global::Android.Content.Intent(global::Android.Provider.Settings.ActionAccessibilitySettings);
            intent.AddFlags(global::Android.Content.ActivityFlags.NewTask);
            global::Android.App.Application.Context.StartActivity(intent);
        }
        catch (Exception ex) { await DisplayAlert("GifTube", ex.Message, "OK"); }
#else
        await DisplayAlert("GifTube", "Эта настройка доступна на Android.", "OK");
#endif
    }
}