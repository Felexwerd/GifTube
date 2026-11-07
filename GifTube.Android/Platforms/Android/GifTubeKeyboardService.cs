using global::Android.App;
using global::Android.Content;
using global::Android.InputMethodServices;
using global::Android.Views;
using global::Android.Views.InputMethods;
using global::Android.Widget;
using System.Text;
using System.Text.Json;
using AndroidButton = global::Android.Widget.Button;
using AndroidColor = global::Android.Graphics.Color;
using AndroidScrollView = global::Android.Widget.ScrollView;

namespace GifTube.Android;

[Service(Name="com.felexwerd.giftube.GifTubeKeyboardService", Label="GifTube", Permission="android.permission.BIND_INPUT_METHOD", Exported=true)]
[IntentFilter(new[] { "android.view.InputMethod" })]
[MetaData("android.view.im", Resource="@xml/giftube_method")]
public class GifTubeKeyboardService : InputMethodService
{
    private const string Api = "https://gifsnap.com/api/v1/gifs";
    private readonly HttpClient http = new() { Timeout = TimeSpan.FromSeconds(12) };
    private LinearLayout? root, grid;
    private EditText? search;
    private TextView? status;

    public override global::Android.Views.View OnCreateInputView()
    {
        root = new LinearLayout(this) { Orientation = Orientation.Vertical };
        root.SetPadding(12, 8, 12, 8);
        root.SetBackgroundColor(AndroidColor.Rgb(18, 11, 27));
        var top = new LinearLayout(this) { Orientation = Orientation.Horizontal };
        var title = new TextView(this) { Text = "GifTube  ✦  GIF-поиск", TextSize = 16 };
        title.SetTextColor(AndroidColor.Rgb(226, 190, 255));
        title.SetPadding(8, 8, 8, 8);
        top.AddView(title, new LinearLayout.LayoutParams(0, -2, 1));
        var close = new AndroidButton(this) { Text = "⌄" };
        close.Click += (_, _) => RequestHideSelf(0);
        top.AddView(close);
        root.AddView(top);

        search = new EditText(this) { Hint = "Поиск GIF…" };
        search.SetSingleLine(true);
        search.SetTextColor(AndroidColor.White);
        root.AddView(search);
        var find = new AndroidButton(this) { Text = "Найти GIF" };
        find.Click += async (_, _) => await Load(search?.Text ?? "");
        root.AddView(find);
        status = new TextView(this) { Text = "Загружаем…", TextSize = 11 };
        status.SetTextColor(AndroidColor.LightGray);
        root.AddView(status);
        var scroll = new AndroidScrollView(this);
        grid = new LinearLayout(this) { Orientation = Orientation.Vertical };
        scroll.AddView(grid);
        root.AddView(scroll, new LinearLayout.LayoutParams(-1, 260));
        _ = Load("");
        return root;
    }

    private async Task Load(string q)
    {
        if (grid == null || status == null) return;
        try
        {
            status.Text = "Загружаем…";
            grid.RemoveAllViews();
            var path = string.IsNullOrWhiteSpace(q) ? "trending" : "search";
            var url = $"{Api}/{path}?page=1&limit=24" + (string.IsNullOrWhiteSpace(q) ? "" : "&q=" + Uri.EscapeDataString(q));
            using var response = await http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            if (!doc.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
            {
                status.Text = "GIF не найдены";
                return;
            }
            var row = new LinearLayout(this) { Orientation = Orientation.Horizontal };
            var count = 0;
            foreach (var item in data.EnumerateArray())
            {
                var u = item.TryGetProperty("url", out var v) ? v.GetString() : "";
                if (!Uri.TryCreate(u, UriKind.Absolute, out var uri) || uri.Scheme != "https") continue;
                var label = item.TryGetProperty("title", out var t) ? t.GetString() : "GIF";
                var button = new AndroidButton(this) { Text = string.IsNullOrWhiteSpace(label) ? "GIF" : label, TextSize = 11 };
                button.SetMaxLines(2);
                button.SetTextColor(AndroidColor.White);
                button.SetBackgroundColor(AndroidColor.Rgb(112, 42, 155));
                var captured = uri.AbsoluteUri;
                button.Click += (_, _) => Insert(captured);
                row.AddView(button, new LinearLayout.LayoutParams(0, 100, 1));
                count++;
                if (count % 2 == 0) { grid.AddView(row); row = new LinearLayout(this) { Orientation = Orientation.Horizontal }; }
            }
            if (row.ChildCount > 0) grid.AddView(row);
            status.Text = count == 0 ? "GIF не найдены" : "Нажми GIF — она вставится в активное поле комментария.";
        }
        catch { status.Text = "Ошибка загрузки. Проверь интернет и повтори поиск."; }
    }

    private void Insert(string url)
    {
        try
        {
            CurrentInputConnection?.CommitText(BuildPayload(url), 1);
            if (status != null) status.Text = "GIF вставлена. Нажми «Опубликовать» в YouTube.";
        }
        catch { if (status != null) status.Text = "Не удалось вставить GIF."; }
    }

    private static string BuildPayload(string url)
    {
        var bytes = new byte[] { 0x47, 0x54, 0x32 }.Concat(Encoding.UTF8.GetBytes(url)).ToArray();
        var builder = new StringBuilder();
        foreach (var value in bytes)
            for (var bit = 7; bit >= 0; bit--) builder.Append(((value >> bit) & 1) == 0 ? '\u200B' : '\u200C');
        return "GIF \u2063\u2063" + builder + "\u2063\u2063";
    }
}
