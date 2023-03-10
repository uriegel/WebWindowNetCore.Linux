WebView
    .Create()
    .InitialBounds(600, 800)
    .Title("Commander")
    .SaveBounds()
    .Url($"file://{Directory.GetCurrentDirectory()}/webroot/index.html")
#if DEBUG            
    .DebuggingEnabled()
#endif            
    .Build()
    .Run("de.uriegel.Commander");    

// TODO xml doku in nuget
// TODO https://webkitgtk.org/reference/webkit2gtk/2.28.2/WebKitURISchemeRequest.html
// TODO Windows Version: https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2.webresourcerequested?view=webview2-dotnet-1.0.1587.40
// TODO Favicon in Webroot, Property obsolet
