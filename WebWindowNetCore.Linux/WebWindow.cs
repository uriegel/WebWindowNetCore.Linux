namespace WebWindowNetCore;
using GtkDotNet;
public class WebWindow : IWebWindow
{
    public void Initialize(Configuration configuration) => this.configuration = configuration;
    public void Execute()
    {
        var app = new Application("de.uriegel.test");
        var ret = app.Run(() => 
        {
            app.EnableSynchronizationContext();
            var webView = new WebView();
            var window = new Window();
            window.Add(webView);
            webView.LoadUri(configuration.Url);
            webView.Settings.EnableDeveloperExtras = true;
            app.AddWindow(window);
            window.SetTitle(configuration.Title);
            window.SetDefaultSize(800, 600);
            window.ShowAll();
        });
    }

    Configuration? configuration;
}
