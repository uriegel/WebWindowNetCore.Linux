using System.Runtime.InteropServices;
using System.Text.Json;

using GtkDotNet;
using WebWindowNetCore.Data;

using static AspNetExtensions.Core;
using GtkDotNet.SafeHandles;
using CsTools.Extensions;

namespace WebWindowNetCore;

enum Action
{
    DevTools = 1,
}

record ScriptAction(Action Action, int? Width, int? Height, bool? IsMaximized);

public class WebView : Base.WebView
{
    public static WebViewBuilder Create() => new();

    public override int Run()
        => Application
            .New(settings.AppId)
            .OnActivate(app => app
                .NewWindow()
                .Title(settings.Title)
                .SideEffectIf(setTitlebar != null,
                    w => w.Titlebar(setTitlebar!()).SideEffect(_ => setTitlebar = null))
                .SideEffectIf(settings.ResourceIcon != null,
                    w => w.ResourceIcon(settings.ResourceIcon!))
                .SideEffectChoose(settings.SaveBounds,
                    w =>
                    {
                        var bounds = Bounds.Retrieve(settings.AppId!, new Bounds(null, null, settings.Width, settings.Height, null));
                        w.DefaultSize(bounds.Width ?? 800, bounds.Height ?? 600);
                        if (bounds.IsMaximized == true)
                            w.Maximize();
                    },
                    w => w.DefaultSize(settings.Width, settings.Height))
                .Child(
                    WebKit.New()
                        .SideEffect(webview => webview.GetSettings()
                            .SideEffectIf(settings.DevTools == true,
                                s => s.EnableDeveloperExtras = true))
                        .SideEffectIf(settings.DefaultContextMenuEnabled != true, webview => webview
                            .DisableContextMenu())
                        .SideEffect(wk => 
                            wk.AddController(
                            EventControllerKey
                                .New()
                                .OnKeyPressed((k, kc, m) => {
                                    if (kc == 73)
                                    {
                                        // prevent blink_cb crash!
                                        wk.RunJavascript(
"""
    document.dispatchEvent(new KeyboardEvent('keydown', {
        key: "F7",
        code: "F7"
    })) 
""");
                                        return true;
                                    }
                                    else
                                        return false;
                                })))
                        .LoadUri(WebViewSettings.GetUri(settings))
                        .OnAlert((wk, msg) =>
                        {
                            var action = JsonSerializer.Deserialize<ScriptAction>(msg ?? "", JsonWebDefaults);
                            switch (action?.Action)
                            {
                                case Action.DevTools:
                                    wk.GetInspector().Show();
                                    break;
                            }
                        })
                        .OnLoadChanged((wk, ls) =>
                        {
                            if (ls == WebViewLoad.Committed)
                            {
                                if (settings.DevTools == true)
                                    WebKit.RunJavascript(wk,
                                        """ 
                                            function webViewShowDevTools() {
                                                alert(JSON.stringify({action: 1}))
                                            }
                                        """);
                                if ((settings.HttpSettings?.RequestDelegates?.Length ?? 0) > 0)
                                    WebKit.RunJavascript(wk,
                                        """ 
                                            async function webViewRequest(method, input) {
                                                const msg = {
                                                    method: 'POST',
                                                    headers: { 'Content-Type': 'application/json' },
                                                    body: JSON.stringify(input)
                                                }

                                                const response = await fetch(`/request/${method}`, msg) 
                                                return await response.json() 
                                            }
                                        """);
                                settings.OnStarted?.Invoke();
                            }
                        }
                    )
                )
                .SideEffectIf(settings.SaveBounds == true,
                    w => w.OnClose(_ =>
                        false.SideEffect(_ =>
                            (Bounds.Retrieve(settings.AppId, new Bounds(null, null, settings.Width, settings.Height, null))
                                with { IsMaximized = w.IsMaximized(), Width = w.GetWidth(), Height = w.GetHeight() })
                                    .Save(settings.AppId))
                    ))
                .Show())
            .Run(0, 0);

    internal WebView(WebViewBuilder builder)
    {
        settings = builder.Data;
        setTitlebar = builder.setTitlebar;
    }
        

    Func<WidgetHandle>? setTitlebar;
    delegate bool CloseDelegate(IntPtr z1, IntPtr z2);
    delegate bool BoolFunc();
    readonly WebViewSettings settings;
}