using WebWindowNetCore;
using GtkDotNet;

static class Program
{
    [STAThread]
    static void Main()
    {
        var window = new WebWindow(new() { 
            Title = "Web View 😎😎👌", 
            Url="https://www.microsoft.com", 
            Organization = "uriegel.de",
            Application="WebWindowNetCoreGlade", 
            DebuggingEnabled = true,
            FullscreenEnabled = true,
            SaveWindowSettings = true
        });
        window.BeforeWindowCreating += (s, e) => 
        {
            // TODO in another file
            using var builder = Builder.FromResource("/de/uriegel/main_window.glade");
            var wnd = new Window(builder.GetObject("window"));
            window.CreateWindow(wnd);

        };
        window.Execute();    
    }
}
