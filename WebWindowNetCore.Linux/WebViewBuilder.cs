using WebWindowNetCore.Data;

namespace WebWindowNetCore;

public class WebViewBuilder : WebWindowNetCore.Base.WebViewBuilder
{
    public override WebView Build() => new WebView(this);

    internal new WebViewSettings Data { get => base.Data; }
}