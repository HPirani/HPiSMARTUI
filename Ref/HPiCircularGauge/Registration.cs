//CircularGauge




using SkiaSharp.Views.Maui.Handlers;

namespace HPiCircularGauge;

public static class Registration
{
    public static MauiAppBuilder UseCircularGauge(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(h =>
        {
            h.AddHandler<CircularGauge, SKCanvasViewHandler>();
        });

        return builder;
    }
}