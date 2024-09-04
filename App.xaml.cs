
namespace HPISMARTUI
    {
    public partial class App : Application
        {
        public App()
            {
            InitializeComponent();

            MainPage = new AppShell();
             



        }
       // protected override Window CreateWindow(IActivationState activationState) => base.CreateWindow(activationState);
        }
    }