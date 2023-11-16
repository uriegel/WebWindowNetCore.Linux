using GtkDotNet;
using WebWindowNetCore.Data;
using LinqTools;
using System.Diagnostics;

namespace WebWindowNetCore;

enum Action
{
    DevTools = 1,
    Show,
}

record ScriptAction(Action Action, int? Width, int? Height, bool? IsMaximized);

public class WebView : WebWindowNetCore.Base.WebView
{
    public static WebViewBuilder Create() => new();

    public override int Run(string gtkId = "de.uriegel.WebViewNetCore")
        => Application.Run(gtkId, app =>
            Application
                .NewWindow(app)
                .SideEffect(w => w.SetTitle(settings?.Title))
                .SideEffect(w => w.SetDefaultSize(settings!.Width, settings!.Height))
                .SideEffectIf(settings?.ResourceIcon != null,
                    w => Window.SetIconFromDotNetResource(w, settings?.ResourceIcon))
                .SideEffect(w => w.SetChild(
                    WebKit
                        .New()
                        .SideEffect(wk => 
                            wk
                                .GetSettings()
                                .SideEffectIf(settings?.DevTools == true,
                                    s => s.SetBool("enable-developer-extras", true))
                        )
                        .SideEffect(wk => wk.LoadUri((Debugger.IsAttached && !string.IsNullOrEmpty(settings?.DebugUrl)
                                                        ? settings?.DebugUrl
                                                        : settings?.Url != null
                                                        ? settings.Url
                                                        : $"http://localhost:{settings?.HttpSettings?.Port ?? 80}{settings?.HttpSettings?.WebrootUrl}/{settings?.HttpSettings?.DefaultHtml}")
                                                            + settings?.Query ?? ""))
                ))
                .Show()
        );

// GtkDotNet.Timer? timer = null;
// saveBounds = settings?.SaveBounds == true;



//             if (!saveBounds)
//                 window.ShowAll();
//             else
//             {
//                 var w = settings?.Width;
//                 var h = settings?.Height;
//                 window.Configure += (s, e) =>
//                 {
//                     timer?.Dispose();
//                     timer = new(() =>
//                     {
//                         if (!window.IsMaximized())
//                             webView.RunJavascript(
//                                 $$"""
//                                 localStorage.setItem('window-bounds', JSON.stringify({width: {{e.Width}}, height: {{e.Height}}}))
//                                 localStorage.setItem('isMaximized', false)
//                             """);
//                         else
//                             webView.RunJavascript($"localStorage.setItem('isMaximized', true)");
//                     }, TimeSpan.FromMilliseconds(400), Timeout.InfiniteTimeSpan);
//                 };
//             }
            
//             var showDevTools = settings?.DevTools == true;
//             var withFetch = (settings?.HttpSettings?.RequestDelegates?.Length ?? 0) > 0;
//             webView.LoadChanged += (s, e) =>
//             {
//                 if (e.LoadEvent == WebKitLoadEvent.WEBKIT_LOAD_COMMITTED)
//                 {
//                     if (saveBounds)
//                         webView.RunJavascript(
//                         """ 
//                             const bounds = JSON.parse(localStorage.getItem('window-bounds') || '{}')
//                             const isMaximized = localStorage.getItem('isMaximized')
//                             if (bounds.width && bounds.height)
//                                 alert(JSON.stringify({action: 2, width: bounds.width, height: bounds.height, isMaximized: isMaximized == 'true'}))
//                             else
//                                 alert(JSON.stringify({action: 2}))
//                         """);
//                     if (showDevTools == true)
//                         webView.RunJavascript(
//                         """ 
//                             function webViewShowDevTools() {
//                                 alert(JSON.stringify({action: 1}))
//                             }
//                         """);
//                     if (withFetch)
//                         webView.RunJavascript(
//                         """ 
//                             async function webViewRequest(method, input) {

//                                 const msg = {
//                                     method: 'POST',
//                                     headers: { 'Content-Type': 'application/json' },
//                                     body: JSON.stringify(input)
//                                 }

//                                 const response = await fetch(`/request/${method}`, msg) 
//                                 return await response.json() 
//                             }
//                         """);
//                 }
//             };

//             webView.ScriptDialog += (s, e) =>
//             {
//                 Console.WriteLine(e.Message);
//                 var action = JsonSerializer.Deserialize<ScriptAction>(e.Message, JsonDefault.Value);
//                 switch (action?.Action)
//                 {
//                     case Action.DevTools:
//                         webView.Inspector.Show();
//                         break;
//                     case Action.Show:
//                         if (action.Width.HasValue && action.Height.HasValue)
//                             window.Resize(action.Width.Value, action.Height.Value);                            
//                         if (action.IsMaximized.GetOrDefault(false))
//                            window.Maximize();
//                         window.ShowAll();   
//                         break;
//                 }
//             };

//             settings = null;
//         });

    internal WebView(WebViewBuilder builder)
        => settings = builder.Data;

    WebViewSettings? settings;

    bool saveBounds;
}

static class Schrott
{
    public static T SideEffectIf<T>(this T t, bool condition, Action<T> action)
     {
        if (condition)
            action(t);
        return t;
     }
}
