using WebWindowNetCore;

static class Program
{
    [STAThread]
    static void Main()
    {
        var window = new WebWindow();
        window.Initialize(new() { 
            Title = "Web View 😎😎👌", 
            Url="https://www.microsoft.com", 
            Organization = "uriegel.de",
            Application="WebWindowNetCore", 
            DebuggingEnabled = true,
            FullscreenEnabled = true,
            SaveWindowSettings = true
        });
        window.Execute();    
    }
}
