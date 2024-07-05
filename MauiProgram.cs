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
using Plugin.Maui.ScreenBrightness;
using MauiPageFullScreen;
using Microsoft.Maui.Hosting;
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
                //.UseMauiCommunityToolkitMarkup()
                .UseMauiCommunityToolkitMediaElement()
                .UseCircularGauge()
               .UseFullScreen()
                .UseRadialDial()
                
                
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("HPiSigns.ttf", "HPiSigns");
                    //
                    fonts.AddFont("2015Cruiser.ttf", "2015Cruiser");
                    fonts.AddFont("Abevel.otf", "Abevel");
                    fonts.AddFont("Coulson-Condensed.otf", "CoulsonCondensed");
                    fonts.AddFont("digital.ttf", "digital");
                    fonts.AddFont("DigitalDream.ttf", "DigitalDream");
                    fonts.AddFont("digitalism.ttf", "digitalism");
                    fonts.AddFont("ds-digi.ttf", "dsdigi");
                    fonts.AddFont("E1234.ttf", "E1234");
                    fonts.AddFont("fabesmellah2.ttf", "fabesmellah2");
                    fonts.AddFont("faBesmellah1.ttf", "fabesmellah1");
                    fonts.AddFont("faelm.ttf", "faelm");
                    fonts.AddFont("fakoodak.ttf", "fakoodak");
                    fonts.AddFont("faNadine.ttf", "faNadine");
                    fonts.AddFont("LiquidCrystal.otf", "LiquidCrystal");
                    fonts.AddFont("modern-lcd.ttf", "modernlcd");
                    fonts.AddFont("OpenDisplay.ttf", "OpenDisplay");
                    fonts.AddFont("phantom-stencil.ttf", "phantomstencil");
                    fonts.AddFont("Seven-Segment.ttf", "SevenSegment");
                    

                });

            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<SplashViewModel>();

            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<SerialPortHelper>();
            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddSingleton(ScreenBrightness.Default);
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