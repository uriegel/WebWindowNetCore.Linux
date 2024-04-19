using CsTools.Extensions;
using GtkDotNet.SafeHandles;
using WebWindowNetCore;
using WebWindowNetCore.Data;

public class WebViewBuilder : WebWindowNetCore.Base.WebViewBuilder
{
    public override WebView Build() => new(this);

    public WebViewBuilder TitleBar(Func<ObjectRef<WebViewHandle>, WidgetHandle> setTitlebar)
        => this.SideEffect(n => this.setTitlebar = setTitlebar);

    internal Func<ObjectRef<WebViewHandle>, WidgetHandle>? setTitlebar;

    internal new WebViewSettings Data { get => base.Data; }
}