using HPISMARTUI.View;
namespace HPISMARTUI
    {
    public partial class AppShell : Shell
        {
        public AppShell()
            {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SplashPage), typeof(SplashPage));
            Routing.RegisterRoute(nameof(MainPage),typeof(MainPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            }
        }
    }