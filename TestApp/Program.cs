var sseEventSource = WebView.CreateEventSource<Event>();
StartEvents(sseEventSource.Send);

WebView
    .Create()
    .InitialBounds(600, 800)
    .ResourceIcon("icon")
    .Title("Commander")
    .SaveBounds()
    //.DebugUrl("https://www.google.de")
    //.Url($"file://{Directory.GetCurrentDirectory()}/webroot/index.html")
    .ConfigureHttp(http => http
        .ResourceWebroot("webroot", "/web")
        .UseSse("sse/test", sseEventSource)
        .Build())
#if DEBUG            
    .DebuggingEnabled()
#endif            
    .Build()
    .Run("de.uriegel.Commander");    

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

record Event(string Content);

// TODO https://webkitgtk.org/reference/webkit2gtk/2.28.2/WebKitURISchemeRequest.html
// TODO Windows Version: https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2.webresourcerequested?view=webview2-dotnet-1.0.1587.40
// TODO Favicon in Webroot, Property obsolet
