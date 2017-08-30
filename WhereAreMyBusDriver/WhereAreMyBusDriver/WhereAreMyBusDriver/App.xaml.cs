using WhereAreMyBusDriver.Views;
using Xamarin.Forms;

namespace WhereAreMyBusDriver
{
    public partial class App : Application
    {

        public static NavigationPage Navigator { get; internal set; }
        public App()
        {
            InitializeComponent();
            MainPage = new LoginPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
