using System;
using System.Threading;
using System.Threading.Tasks;
using WhereAreMyBusDriver.Models;
using WhereAreMyBusDriver.Services;
using WhereAreMyBusDriver.ViewModels;
using WhereAreMyBusDriver.Views;
using Xamarin.Forms;

namespace WhereAreMyBusDriver
{
    public partial class App : Application
    {

        string key;
        public static NavigationPage Navigator { get; internal set; }
        public static CancellationTokenSource CancellationToken { get; set; }
        public int contador { get; set; }
        private ApiService apiService;
        private DialogService dialogService;
        GeolocatorService geolocatorService;

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

            Task.Factory.StartNew(() => backgroundThread(), TaskCreationOptions.LongRunning);

        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        async Task backgroundThread()
        {
            CancellationToken = new CancellationTokenSource();
            while (!CancellationToken.IsCancellationRequested)
            {

                CancellationToken.Token.ThrowIfCancellationRequested();
                await Task.Delay(500, CancellationToken.Token).ContinueWith(async (arg) => {
                    if (!CancellationToken.Token.IsCancellationRequested)
                    {
                        CancellationToken.Token.ThrowIfCancellationRequested();
                        contador = contador + 1;
                        await postHora();
                    }
                });

            }
        }

        async Task postHora()
        {
            var urlAPI = Application.Current.Resources["URLAPI"].ToString();
            var responsePost = await apiService.Post<int>(
                urlAPI,
                "/hours",
                ".json",
                contador);


        }


    }
}
