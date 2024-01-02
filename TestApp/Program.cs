using AspNetExtensions;
using GtkDotNet;
using WebWindowNetCore;
using CsTools.Extensions;

var sseEventSource = WebView.CreateEventSource<Event>();
StartEvents(sseEventSource.Send);

WebView
    .Create()
    .SetAppId("de.uriegel.test.linux")
    .InitialBounds(600, 800)
    .ResourceIcon("icon")
    .Title("Linux Tester")
    .DownCast<WebViewBuilder>()
    .TitleBar(() => HeaderBar.New()
                            .PackEnd(
                                ToggleButton.New()
                                .IconName("open-menu-symbolic")
                            ))
    .QueryString(() => $"?theme={Gtk.Dispatch(() => GtkSettings.GetDefault().GetString("gtk-theme-name")).Result}")
    .SaveBounds()
    .DefaultContextMenuEnabled()
    .OnStarted(() => Console.WriteLine("Now really started"))
    .DebugUrl("http://localhost:5173")
    //.Url($"file://{Directory.GetCurrentDirectory()}/webroot/index.html")
    .ConfigureHttp(http => http
        .ResourceWebroot("webroot", "/web")
        .UseSse("sse/test", sseEventSource)
        .MapGet("video", context => 
            context
                .SideEffect(c => Console.WriteLine("Range request"))
            .StreamRangeFile("/home/uwe/Videos/Buster Keaton - Sherlock Jr..mp4"))        
        .JsonPost<Msg, MsgResult>("request/cmd1", OnMsg)            
        .Build())
#if DEBUG            
    .DebuggingEnabled()
#endif            
    .Build()
    .Run();    

void StartEvents(Action<Event> onChanged)   
{
    var counter = 0;
    new Thread(_ =>
        {
            while (true)
            {
                Thread.Sleep(5000);
                onChanged(new($"Ein Event {counter++}"));
           }
        })
        {
            IsBackground = true
        }.Start();   
}

Task<MsgResult> OnMsg(Msg msg)
    => Task.FromResult(new MsgResult("The result"));

record Event(string Content);

record Msg(string Text, int Id);
record MsgResult(string Text);

// TODO https://webkitgtk.org/reference/webkit2gtk/2.28.2/WebKitURISchemeRequest.html
// TODO Windows Version: https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2.webresourcerequested?view=webview2-dotnet-1.0.1587.40
// TODO Favicon in Webroot, Property obsolet
