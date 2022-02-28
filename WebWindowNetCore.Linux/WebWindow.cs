namespace WebWindowNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;

using GtkDotNet;

public class WebWindow : IWebWindow
{
    // TODO: Configuration: x, y width height, isMaximized
    // TODO: Resource loading
    // TODO: Drag and Drop internal
    // TODO: Drag and Drop Files
    // TODO: Events
    
    record Settings(int x, int y, int width, int height, bool isMaximized);

    public void Initialize(Configuration configuration) => this.configuration = configuration;
    public void Execute()
    {
        // TODO: to configuration
        // TODO: Drag and Drop: Windows, Linux mit Files
        // TODO: Commander mit Menu versteckbar
        // TODO: In Gtk set progress from background thread


        // TODO or -1 for not set
        var settings = new Settings(1800, 200, 800, 600, false);
        string jsonString = JsonSerializer.Serialize(settings);
        var savedSettings = JsonSerializer.Deserialize<Settings>(jsonString);
        // TODO or get from saved Settings

        var organisation = "test.uriegel.de";
        var application = "test-app";


        var app = new Application(organisation);
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
            // TODO if not saved settings
            window.SetDefaultSize(settings.width, settings.height);
            window.Move(settings.x, settings.y);
            if (settings.isMaximized)
                window.Maximize();

            var appData = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), organisation, application);


            window.ShowAll();
        });
    }

    Configuration? configuration;
}
