using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using CommunityToolkit.Maui;
using HPiCircularGauge;
using epj.RadialDial.Maui;
using HPISMARTUI.Helper;
using HPISMARTUI.ViewModel;
using HPISMARTUI.View;
using Plugin.Maui.Audio;
using HPISMARTUI.Services;
using HPISMARTUI.Model;

namespace HPISMARTUI
{
    public static class MauiProgram
        {
        
        public static MauiApp CreateMauiApp()
            {

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseCircularGauge()
                .UseRadialDial()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("HPiSigns.ttf", "HPiSigns");
                });

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<SerialPortHelper>();
            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddSingleton<SmsManagerTestService>();
            builder.Services.AddSingleton<PersistentService>();
            builder.Services.AddSingleton<SendStatusReceiver>();
            builder.Services.AddSingleton<AndroidLocationManager>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
            }
        }
    
}