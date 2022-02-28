namespace WebWindowNetCore;
using System.Text.Json;

using GtkDotNet;

public abstract class WebWindowBase
{
    public record Settings(int X, int Y, int Width, int Height, bool IsMaximized);

    public WebWindowBase(Configuration configuration) 
        => this.configuration = configuration;

    public void Execute()
    {
        var settings = new Settings(configuration.InitialPosition?.X ?? -1, configuration.InitialPosition?.Y ?? -1, 
            configuration.InitialSize?.Width ?? 800, configuration.InitialSize?.Height ?? 600, configuration.IsMaximized == true);

        if (configuration.SaveWindowSettings == true)
        {
            var appData = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                configuration.Organization!, configuration.Application!);
            if (!Directory.Exists(appData))
                Directory.CreateDirectory(appData);

            settingsFile = Path.Combine(appData, "settings.json");
            if (File.Exists(settingsFile))
            {
                using var reader = new StreamReader(File.OpenRead(settingsFile));
                var settingsString = reader.ReadToEnd();
                if (settingsString?.Length > 0)
                {
                    var s = JsonSerializer.Deserialize<Settings>(settingsString);
                    if (s != null)
                        settings = s;
                }
            }
        }  
        Run(settings);
    }

    protected abstract void Run(Settings settings);

    protected void SaveSettings(WebWindowBase.Settings settings)
    {
        var json = JsonSerializer.Serialize(settings);
        using var writer = new StreamWriter(File.Create(settingsFile));
        writer.Write(json);
    }

    protected Configuration configuration;
    string settingsFile = "";      
}

public class WebWindow : WebWindowBase
{
    // TODO: Resource loading
    // TODO: Resource loading glade file
    // TODO: Resource homepage per ResourceStrings
    // TODO: Drag and Drop internal
    // TODO: Drag and Drop Files
    // TODO: Events
    // TODO: Show Dev Tools
    // TODO: Show Fullscreen
    
    public WebWindow(Configuration configuration) : base(configuration) {}
    protected override void Run(Settings settings)
    {
        // TODO: Commander mit Menu versteckbar
        // TODO: In Gtk set progress from background thread

        var app = new Application(configuration.Organization);
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
            window.SetDefaultSize(settings.Width, settings.Height);
            window.Move(settings.X, settings.Y);
            if (settings.IsMaximized)
                window.Maximize();

            var recentSettings = settings;            
            if (configuration.SaveWindowSettings == true)
            {
                window.Delete += (s, _) =>
                {
                    if (s is Window win)
                    {
                        var (w, h) = win.Size;
                        var (x, y) = win.GetPosition();
                        if (window.IsMaximized())
                            SaveSettings(recentSettings with { IsMaximized = true});
                        else
                            SaveSettings(new Settings(x, y, w, h, false));
                    }
                };
                window.Configure += (s, e) => 
                {
                    if (!window.IsMaximized() && s is Window win)
                    {
                        var (x, y) = win.GetPosition();
                        var (w, h) = win.Size;
                        recentSettings = new Settings(x, y, w, h, false);
                    }
                };
            }
            window.ShowAll();
        });
    }
}
