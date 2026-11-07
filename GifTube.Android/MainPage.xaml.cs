using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
namespace GifTube.Android;
public partial class MainPage : ContentPage
{
    private const string ApiBase = "https://gifsnap.com/api/v1/gifs";
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };
    public ObservableCollection<GifItem> Gifs { get; } = new();
    private bool _busy;
    public MainPage() { InitializeComponent(); BindingContext = this; _ = LoadGifsAsync(false, ""); }
    private async Task LoadGifsAsync(bool search, string query)
    {
        if (_busy) return; _busy = true;
        try {
            StatusLabel.Text = "Загружаем GIF…";
            var endpoint = search ? "search" : "trending";
            var url = $"{ApiBase}/{endpoint}?page=1&limit=50";
            if (search) url += "&q=" + Uri.EscapeDataString(query.Trim());
            using var response = await Http.GetAsync(url); response.EnsureSuccessStatusCode();
            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            Gifs.Clear();
            if (doc.RootElement.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
            foreach (var item in data.EnumerateArray()) {
                var full = GetString(item, "url");
                if (!Uri.TryCreate(full, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps) continue;
                var preview = full;
                if (item.TryGetProperty("animated_preview", out var animated) && animated.ValueKind == JsonValueKind.Object) {
                    var candidate = GetString(animated, "url");
                    if (Uri.TryCreate(candidate, UriKind.Absolute, out var p) && p.Scheme == Uri.UriSchemeHttps) preview = candidate;
                }
                Gifs.Add(new GifItem { Title = string.IsNullOrWhiteSpace(GetString(item, "title")) ? "GIF" : GetString(item, "title"), Preview = preview, Full = uri.AbsoluteUri });
            }
            StatusLabel.Text = Gifs.Count == 0 ? "GIF не найдены. Попробуй другой запрос." : $"Найдено GIF: {Gifs.Count}. Выбери нужную.";
        }
        catch (Exception ex) { StatusLabel.Text = "Не удалось загрузить GIF. Проверь интернет и попробуй ещё раз."; await DisplayAlert("GifTube", ex.Message, "OK"); }
        finally { _busy = false; }
    }
    private static string GetString(JsonElement obj, string name) {
        if (!obj.TryGetProperty(name, out var value)) return "";
        return value.ValueKind switch { JsonValueKind.String => value.GetString() ?? "", JsonValueKind.True => "true", _ => "" };
    }
    private async void OnTrendingClicked(object sender, EventArgs e) { SearchEntry.Text = ""; await LoadGifsAsync(false, ""); }
    private async void OnSearchClicked(object sender, EventArgs e) => await LoadGifsAsync(true, SearchEntry.Text ?? "");
    private async void OnSearchCompleted(object sender, EventArgs e) => await LoadGifsAsync(true, SearchEntry.Text ?? "");
    private async void OnCopyGifClicked(object sender, EventArgs e) {
        if (sender is not Button { CommandParameter: GifItem gif }) return;
        await Clipboard.Default.SetTextAsync(BuildCompatiblePayload(gif.Full));
        await DisplayAlert("GifTube", "GIF скопирована! Открой YouTube, вставь её в комментарий и опубликуй.", "Понятно");
    }
    // Compatible with desktop GifTube GT2 decoder.
    private static string BuildCompatiblePayload(string httpsUrl) {
        var uri = new Uri(httpsUrl);
        if (uri.Scheme != Uri.UriSchemeHttps || !string.IsNullOrEmpty(uri.UserInfo)) throw new InvalidOperationException("Разрешены только безопасные HTTPS-ссылки на GIF.");
        var bytes = new byte[] { 0x47, 0x54, 0x32 }.Concat(Encoding.UTF8.GetBytes(uri.AbsoluteUri)).ToArray();
        var bits = string.Concat(bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
        var invisible = new StringBuilder(bits.Length);
        foreach (var bit in bits) invisible.Append(bit == '0' ? '\u200B' : '\u200C');
        return "GIF \u2063\u2063" + invisible + "\u2063\u2063";
    }
}
public sealed class GifItem { public string Title { get; set; } = "GIF"; public string Preview { get; set; } = ""; public string Full { get; set; } = ""; }