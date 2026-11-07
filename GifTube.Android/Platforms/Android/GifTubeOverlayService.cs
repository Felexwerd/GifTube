using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using LinearLayout = global::Android.Widget.LinearLayout;
using TextView = global::Android.Widget.TextView;
using EditText = global::Android.Widget.EditText;
using ImageView = global::Android.Widget.ImageView;
using ScrollView = global::Android.Widget.ScrollView;
using Button = global::Android.Widget.Button;
using Orientation = global::Android.Widget.Orientation;
using AndroidImageDecoder = global::Android.Graphics.ImageDecoder;
using Android.Runtime;
using System.Text.Json;
using AndroidColor = Android.Graphics.Color;
using View = Android.Views.View;
using AndroidOS = global::Android.OS;

namespace GifTube.Android;

[Service(Exported = false)]
public class GifTubeOverlayService : Service
{
    const string Api = "https://gifsnap.com/api/v1/gifs";
    IWindowManager? wm;
    View? panel;
    int page = 1;
    string query = "";
    bool favoritesOnly;
    readonly HashSet<string> favorites = new();
    readonly HttpClient http = new() { Timeout = TimeSpan.FromSeconds(18) };
    LinearLayout? grid;
    TextView? status;
    ScrollView? scroll;
    bool loading;

    public override IBinder? OnBind(Intent? intent) => null;

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        if (intent?.Action == "close") { StopSelf(); return StartCommandResult.NotSticky; }
        if (panel == null) ShowPanel();
        return StartCommandResult.Sticky;
    }

    void ShowPanel()
    {
        wm = GetSystemService(WindowService)!.JavaCast<IWindowManager>();
        var root = new LinearLayout(this) { Orientation = Orientation.Vertical };
        root.SetPadding(14, 12, 14, 8);
        root.SetBackgroundColor(AndroidColor.Rgb(15, 11, 22));

        var header = new LinearLayout(this) { Orientation = Orientation.Horizontal };
        header.SetGravity(GravityFlags.CenterVertical);
        var logo = new TextView(this) { Text = " GIF ", TextSize = 16, Gravity = GravityFlags.Center };
        logo.SetTextColor(AndroidColor.White); logo.SetBackgroundColor(AndroidColor.Rgb(174, 62, 211));
        header.AddView(logo, new LinearLayout.LayoutParams(48, 42));
        var heading = new LinearLayout(this) { Orientation = Orientation.Vertical };
        var name = new TextView(this) { Text = "  GifTube  2.2.7", TextSize = 19, Typeface = Typeface.DefaultBold };
        name.SetTextColor(AndroidColor.White);
        var subtitle = new TextView(this) { Text = "  GIF-библиотека для YouTube", TextSize = 11 };
        subtitle.SetTextColor(AndroidColor.Rgb(183, 165, 203));
        heading.AddView(name); heading.AddView(subtitle);
        header.AddView(heading, new LinearLayout.LayoutParams(0, -2, 1));
        var close = new Button(this) { Text = "×", TextSize = 22 };
        close.SetTextColor(AndroidColor.White); close.SetBackgroundColor(AndroidColor.Transparent);
        close.Click += (_, _) => StopSelf();
        header.AddView(close, new LinearLayout.LayoutParams(48, 44));
        root.AddView(header);

        var searchRow = new LinearLayout(this) { Orientation = Orientation.Horizontal };
        searchRow.SetGravity(GravityFlags.CenterVertical);
        var input = new EditText(this) { Hint = "⌕  Найти GIF… например hello, wow, anime", TextSize = 14 };
        input.SetSingleLine(true);
        input.SetTextColor(AndroidColor.White); input.SetHintTextColor(AndroidColor.Rgb(143, 128, 158));
        input.SetBackgroundColor(AndroidColor.Rgb(31, 24, 40));
        searchRow.AddView(input, new LinearLayout.LayoutParams(0, 48, 1));
        var searchButton = new Button(this) { Text = "НАЙТИ", TextSize = 10 };
        searchButton.SetTextColor(AndroidColor.White); searchButton.SetBackgroundColor(AndroidColor.Rgb(116, 52, 158));
        searchRow.AddView(searchButton, new LinearLayout.LayoutParams(76, 48));
        root.AddView(searchRow);
        searchButton.Click += async (_, _) => { query = input.Text ?? ""; page = 1; favoritesOnly = false; await Load(true); };
        input.EditorAction += async (_, _) => { query = input.Text ?? ""; page = 1; await Load(true); };

        var tabs = new LinearLayout(this) { Orientation = Orientation.Horizontal };
        var popular = new Button(this) { Text = "Популярное" };
        var fav = new Button(this) { Text = "♡ Избранное" };
        foreach (var b in new[] { popular, fav }) { b.SetTextColor(AndroidColor.White); b.SetBackgroundColor(AndroidColor.Rgb(40, 29, 51)); }
        tabs.AddView(popular, new LinearLayout.LayoutParams(0, 44, 1));
        tabs.AddView(fav, new LinearLayout.LayoutParams(0, 44, 1));
        root.AddView(tabs);
        popular.Click += async (_, _) => { favoritesOnly = false; page = 1; await Load(true); };
        fav.Click += async (_, _) => { favoritesOnly = true; await Load(true); };

        status = new TextView(this) { Text = "Загружаем GIF…", TextSize = 11 };
        status.SetTextColor(AndroidColor.Rgb(199, 174, 220));
        root.AddView(status);
        scroll = new ScrollView(this);
        grid = new LinearLayout(this) { Orientation = Orientation.Vertical };
        scroll.AddView(grid);
        root.AddView(scroll, new LinearLayout.LayoutParams(-1, 0, 1));
        scroll.ScrollChange += async (_, e) =>
        {
            if (!favoritesOnly && e.ScrollY + scroll.Height >= grid.Height - 260 && !loading) { page++; await Load(false); }
        };

        panel = root;
        var p = new WindowManagerLayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            (int)(Resources!.DisplayMetrics!.HeightPixels * 0.78),
            Build.VERSION.SdkInt >= BuildVersionCodes.O ? WindowManagerTypes.ApplicationOverlay : WindowManagerTypes.Phone,
            WindowManagerFlags.LayoutInScreen | WindowManagerFlags.LayoutNoLimits,
            global::Android.Graphics.Format.Translucent)
        { Gravity = GravityFlags.Bottom, SoftInputMode = SoftInput.AdjustResize };
        wm!.AddView(panel, p);
        _ = Load(true);
    }

    async Task Load(bool clear)
    {
        if (grid == null || status == null || loading) return;
        loading = true;
        try
        {
            if (clear) grid.RemoveAllViews();
            status.Text = "Загружаем GIF…";
            if (favoritesOnly)
            {
                var saved = favorites.ToArray();
                if (clear) grid.RemoveAllViews();
                foreach (var url in saved) AddGif(url, "Избранное");
                status.Text = $"{saved.Length} в избранном";
                return;
            }
            var route = string.IsNullOrWhiteSpace(query) ? "trending" : "search";
            var urlApi = $"{Api}/{route}?page={page}&limit=30" + (string.IsNullOrWhiteSpace(query) ? "" : "&q=" + Uri.EscapeDataString(query));
            using var response = await http.GetAsync(urlApi);
            response.EnsureSuccessStatusCode();
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            if (!doc.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
            { status.Text = "GIF не найдены"; return; }
            var added = 0;
            foreach (var item in data.EnumerateArray())
            {
                var url = item.TryGetProperty("url", out var u) ? u.GetString() : null;
                var previewUrl = url;
                if (item.TryGetProperty("animated_preview", out var animated) &&
                    animated.ValueKind == JsonValueKind.Object &&
                    animated.TryGetProperty("animated", out var isAnimated) && isAnimated.ValueKind == JsonValueKind.True &&
                    animated.TryGetProperty("mime_type", out var mime) &&
                    new[] { "image/gif", "image/webp", "image/apng" }.Contains(mime.GetString()?.ToLowerInvariant()) &&
                    animated.TryGetProperty("url", out var animatedUrl))
                    previewUrl = animatedUrl.GetString() ?? url;
                if (Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme == "https")
                {
                    var title = item.TryGetProperty("title", out var t) ? t.GetString() ?? "GIF" : "GIF";
                    AddGif(uri.AbsoluteUri, title, previewUrl);
                    added++;
                }
            }
            status.Text = added == 0 && page == 1 ? "GIF не найдены" : "Выбери GIF — она вставится в комментарий YouTube";
        }
        catch { status.Text = "Не удалось загрузить GIF. Проверь подключение."; }
        finally { loading = false; }
    }

    void AddGif(string url, string title, string? previewUrl = null)
    {
        if (grid == null) return;
        var row = grid.ChildCount > 0 && grid.GetChildAt(grid.ChildCount - 1) is LinearLayout last && last.ChildCount < 3
            ? last : new LinearLayout(this) { Orientation = Orientation.Horizontal };
        if (row.Parent == null) grid.AddView(row, new LinearLayout.LayoutParams(-1, -2));
        var card = new LinearLayout(this) { Orientation = Orientation.Vertical };
        card.SetPadding(3, 3, 3, 5);
        var image = new ImageView(this) { LayoutParameters = new LinearLayout.LayoutParams(-1, 112) };
        image.SetScaleType(ImageView.ScaleType.CenterCrop);
        image.SetBackgroundColor(AndroidColor.Rgb(35, 29, 43));
        card.AddView(image);
        var caption = new TextView(this) { Text = title.Length > 23 ? title[..23] : title, TextSize = 10, Gravity = GravityFlags.Center };
        caption.SetMaxLines(1);
        caption.SetTextColor(AndroidColor.Rgb(222, 209, 235)); card.AddView(caption);
        var actions = new LinearLayout(this) { Orientation = Orientation.Horizontal };
        var send = new Button(this) { Text = "ВСТАВИТЬ", TextSize = 9 };
        send.SetTextColor(AndroidColor.White); send.SetBackgroundColor(AndroidColor.Rgb(117, 49, 160));
        send.Click += (_, _) => Insert(url);
        var heart = new Button(this) { Text = favorites.Contains(url) ? "♥" : "♡", TextSize = 14 };
        heart.SetTextColor(AndroidColor.White); heart.SetBackgroundColor(AndroidColor.Rgb(43, 33, 52));
        heart.Click += (_, _) => { if (!favorites.Add(url)) favorites.Remove(url); heart.Text = favorites.Contains(url) ? "♥" : "♡"; };
        actions.AddView(send, new LinearLayout.LayoutParams(0, 38, 1)); actions.AddView(heart, new LinearLayout.LayoutParams(42, 38));
        card.AddView(actions);
        row.AddView(card, new LinearLayout.LayoutParams(0, -2, 1));
        row.LayoutParameters = new LinearLayout.LayoutParams(-1, -2);
        _ = LoadPreview(image, previewUrl ?? url);
    }

    async Task LoadPreview(ImageView image, string url)
    {
        try
        {
            var bytes = await http.GetByteArrayAsync(url);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                try
                {
                    var source = AndroidImageDecoder.CreateSource(Java.Nio.ByteBuffer.Wrap(bytes));
                    var drawable = AndroidImageDecoder.DecodeDrawable(source);
                    image.Post(() =>
                    {
                        image.SetImageDrawable(drawable);
                        if (drawable is global::Android.Graphics.Drawables.AnimatedImageDrawable animated)
                            animated.Start();
                    });
                    return;
                }
                catch { /* Fall back to the legacy GIF decoder for unsupported formats. */ }
            }
            var movie = Movie.DecodeStream(new MemoryStream(bytes));
            if (movie != null)
            {
                image.Post(() => image.SetImageDrawable(new GifMovieDrawable(movie)));
                return;
            }
            var bmp = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
            if (bmp != null) image.Post(() => image.SetImageBitmap(bmp));
        }
        catch { }
    }

    void Insert(string url)
    {
        var payload = BuildPayload(url);
        var inserted = GifTubeAccessibilityService.Instance?.InsertIntoYouTube(payload) == true;
        if (!inserted)
        {
            var clipboard = (ClipboardManager)GetSystemService(ClipboardService)!;
            clipboard.PrimaryClip = ClipData.NewPlainText("GifTube", payload);
            status!.Text = "Скопировано. Нажми поле комментария YouTube и вставь содержимое.";
        }
        else status!.Text = "GIF вставлена в комментарий. Можно опубликовать!";
    }

    static string BuildPayload(string url)
    {
        var bytes = new byte[] { 0x47, 0x54, 0x32 }.Concat(System.Text.Encoding.UTF8.GetBytes(url)).ToArray();
        var b = new System.Text.StringBuilder();
        foreach (var value in bytes) for (var bit = 7; bit >= 0; bit--) b.Append(((value >> bit) & 1) == 0 ? '\u200B' : '\u200C');
        return "GIF \u2063\u2063" + b + "\u2063\u2063";
    }

    public override void OnDestroy()
    {
        if (panel != null) { try { wm?.RemoveView(panel); } catch { } panel = null; }
        http.Dispose();
        base.OnDestroy();
    }
}

public sealed class GifMovieDrawable : Drawable, Java.Lang.IRunnable
{
    readonly Movie movie;
    readonly Handler handler = new(Looper.MainLooper!);
    long started;
    public GifMovieDrawable(Movie movie) { this.movie = movie; started = AndroidOS.SystemClock.UptimeMillis(); }
    public override int Opacity => (int)Format.Translucent;
    public override void Draw(Canvas canvas)
    {
        var b = Bounds;
        if (movie.Width() <= 0 || movie.Height() <= 0) return;
        var scale = Math.Min((float)b.Width() / movie.Width(), (float)b.Height() / movie.Height());
        canvas.Save();
        canvas.Translate(b.Left + (b.Width() - movie.Width() * scale) / 2, b.Top + (b.Height() - movie.Height() * scale) / 2);
        canvas.Scale(scale, scale);
        var duration = Math.Max(1, movie.Duration());
        movie.SetTime((int)((AndroidOS.SystemClock.UptimeMillis() - started) % duration));
        movie.Draw(canvas, 0, 0);
        canvas.Restore();
        ScheduleSelf(this, AndroidOS.SystemClock.UptimeMillis() + 40);
    }
    public void Run() => InvalidateSelf();
    public override void SetAlpha(int alpha) { }
    public override void SetColorFilter(ColorFilter? colorFilter) { }
}
