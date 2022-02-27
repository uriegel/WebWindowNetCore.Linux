using WebWindowNetCore;

static class Program
{
    [STAThread]
    static void Main()
    {
        var window = new WebWindow();
        window.Initialize(new() { Title = "Web View 😎😎👌", Url="https://www.microsoft.com"});
        window.Execute();    
    }
}
