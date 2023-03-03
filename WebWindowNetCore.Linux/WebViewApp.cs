using GtkDotNet;

namespace WebWindow;

public static class WebViewApp
{
    public static void Run()
    {
        var app = new Application("de.uriegel.WebWindow");
        var ret = app.Run(() =>
        {
            var window = new Window();
            window.SetTitle("Web View");
            app.AddWindow(window);
            var webView = new WebView();
            webView.LoadUri("https://www.google.de");
            window.Add(webView);
            window.ShowAll();
        });
    }
}
