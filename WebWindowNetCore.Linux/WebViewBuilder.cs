using CsTools.Extensions;
using GtkDotNet.SafeHandles;
using WebWindowNetCore;
using WebWindowNetCore.Data;

public class WebViewBuilder : WebWindowNetCore.Base.WebViewBuilder
{
    public override WebView Build() => new WebView(this);

    public WebViewBuilder TitleBar(Func<WidgetHandle> setTitlebar)
        => this.SideEffect(n => this.setTitlebar = setTitlebar);

    internal Func<WidgetHandle>? setTitlebar;

    internal new WebViewSettings Data { get => base.Data; }
}