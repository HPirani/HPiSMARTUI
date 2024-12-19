
namespace HPISMARTUI
    {
    public partial class App : Application
        {
        public App()
            {
            InitializeComponent();

          //  MainPage = new AppShell();
            
        }

        //protected override Window CreateWindow(IActivationState activationState) => base.CreateWindow(activationState);
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        protected override Window CreateWindow(IActivationState? activationState)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
            {
            return new Window(new AppShell());
            }
        }
    }