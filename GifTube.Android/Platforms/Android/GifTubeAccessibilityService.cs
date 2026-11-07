using Android.AccessibilityServices;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views.Accessibility;

namespace GifTube.Android;

[Service(Name = "com.felexwerd.giftube.GifTubeAccessibilityService",
    Permission = "android.permission.BIND_ACCESSIBILITY_SERVICE",
    Exported = true,
    Label = "GifTube — вставка GIF")]
[IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
[MetaData("android.accessibilityservice", Resource = "@xml/giftube_accessibility")]
public class GifTubeAccessibilityService : AccessibilityService
{
    public static GifTubeAccessibilityService? Instance { get; private set; }

    protected override void OnServiceConnected()
    {
        base.OnServiceConnected();
        Instance = this;
    }

    public bool InsertIntoYouTube(string payload)
    {
        try
        {
            foreach (var window in Windows ?? Array.Empty<AccessibilityWindowInfo>())
            {
                var root = window.Root;
                if (root == null || root.PackageName?.ToString() != "com.google.android.youtube") continue;
                var target = FindEditable(root);
                if (target == null) continue;
                var args = new Bundle();
                args.PutCharSequence(AccessibilityNodeInfo.ActionArgumentSetTextCharSequence, payload);
                if (target.PerformAction(AccessibilityNodeInfo.ActionSetText, args)) return true;
            }
        }
        catch { }
        return false;
    }

    static AccessibilityNodeInfo? FindEditable(AccessibilityNodeInfo node)
    {
        if (node.PackageName?.ToString() == "com.google.android.youtube" && node.IsEditable && node.IsEnabled)
            return node;
        for (var i = 0; i < node.ChildCount; i++)
        {
            var child = node.GetChild(i);
            if (child == null) continue;
            var found = FindEditable(child);
            if (found != null) return found;
        }
        return null;
    }

    public override void OnAccessibilityEvent(AccessibilityEvent? e) { }
    public override void OnInterrupt() { }
    public override void OnDestroy() { if (Instance == this) Instance = null; base.OnDestroy(); }
}
