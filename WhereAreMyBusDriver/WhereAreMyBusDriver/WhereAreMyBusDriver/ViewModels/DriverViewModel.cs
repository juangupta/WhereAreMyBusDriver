using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WhereAreMyBusDriver.Models;
using WhereAreMyBusDriver.Services;
using Xamarin.Forms;

namespace WhereAreMyBusDriver.ViewModels
{
    public class DriverViewModel: INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private ApiService apiService;
        private DialogService dialogService;
        private bool isEnabledStart;
        private bool isEnabledEnd;
        GeolocatorService geolocatorService;
        string key;
        #endregion

        #region Properties
        public ObservableCollection<Route> Rutas { get;set;}
        public string MyRoute { get; set; }
        public static CancellationTokenSource CancellationToken { get; set; }
        public bool IsEnabledStart
        {
            set
            {
                if (isEnabledStart != value)
                {
                    isEnabledStart = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEnabledStart"));
                }
            }
            get
            {
                return isEnabledStart;
            }
        }

        public bool IsEnabledEnd
        {
            set
            {
                if (isEnabledEnd != value)
                {
                    isEnabledEnd = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEnabledEnd"));
                }
            }
            get
            {
                return isEnabledEnd;
            }
        }


        #endregion

        #region Constructors
        public DriverViewModel()
        {
            apiService = new ApiService();
            dialogService = new DialogService();
            Rutas = new ObservableCollection<Route>();
            geolocatorService = new GeolocatorService();
            CancellationToken = new CancellationTokenSource();
            IsEnabledStart = true;
            IsEnabledEnd = false;
            GetRutas();
        }
        #endregion

        #region Commands
        public ICommand StartCommand
        {
            get { return new RelayCommand(Start); }
        }

        private async void Start()
        {
            if (string.IsNullOrEmpty(MyRoute))
            {
                await dialogService.ShowMessage("Error", "Seleccione una ruta.");
                return;
            }
            //IsEnabledStart = false;
            var mainViewModel = MainViewModel.GetInstance();
            //var checkConnection = await apiService.CheckConnection();
            //if (!checkConnection.IsSuccess)
            //{
            //    IsRunning = false;
            //    await dialogService.ShowMessage("Error", checkConnection.Message);
            //    return;

            //}

            var urlAPI = Application.Current.Resources["URLAPI"].ToString();
            var response = await apiService.GetByField<Location>(
                urlAPI,
                "/locations.json",
                "placa",
                mainViewModel.Driver.Placa,
                mainViewModel.Driver.Token);

            if (response.IsSuccess)
            {
                //IsEnabledStart = true;
                //await dialogService.ShowMessage("Error", "Problemas registrando el viaje.");
                var responseDelete = await apiService.Delete(
                urlAPI,
                "/locations",
                mainViewModel.Driver.Token,
                response.Extra);

            }

            await geolocatorService.GetLocation();
            var newLocation = new Location
            {
                Placa = mainViewModel.Driver.Placa,
                Vehiculo = mainViewModel.Driver.Vehiculo,
                Ruta = MyRoute,
                Latitud = geolocatorService.Latitude,
                Longitud = geolocatorService.Longitude

            };

            var responsePost = await apiService.Post<Location>(
                urlAPI,
                "/locations",
                ".json",
                newLocation,
                mainViewModel.Driver.Token);

            if (!responsePost.IsSuccess)
            {
                IsEnabledStart = true;
                await dialogService.ShowMessage("Error", "No se ha podido iniciar el viaje");
                return;
               

            }
            var postResponse = (PostResponse)responsePost.Result;
            key = postResponse.Name;
            IsEnabledEnd = true;
            IsEnabledStart = false;

            Task.Run(async () => backgroundThread());

        }

        public ICommand EndCommand
        {
            get { return new RelayCommand(End); }
        }

        private async void End()
        {
            IsEnabledEnd = false;
            IsEnabledStart = true;
            CancellationToken.Cancel();
            var urlAPI = Application.Current.Resources["URLAPI"].ToString();
            var mainViewModel = MainViewModel.GetInstance();
            var responseDelete = await apiService.Delete(
                urlAPI,
                "/locations",
                mainViewModel.Driver.Token,
                key);
        }
        #endregion

        #region Methods
        private async void GetRutas()
        {
            //var checkConnetion = await apiService.CheckConnection();
            //if (!checkConnetion.IsSuccess)
            //{
            //    await dialogService.ShowMessage("Error", checkConnetion.Message);
            //    return;
            //}

            var urlAPI = Application.Current.Resources["URLAPI"].ToString();
            var mainViewModel = MainViewModel.GetInstance();
            var response = await apiService.GetList<Route>(
                urlAPI,
                "/routes",
                ".json");

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", "Error recuperando las rutas");
                return;
            }

            ReloadRoutes((List<Route>)response.Result);
        }

        void ReloadRoutes(List<Route> routes)
        {
            Rutas.Clear();
            foreach (var route in routes)
            {
                if (route != null)
                {
                    Rutas.Add(route);
                }

            }
        }

        async Task backgroundThread()
        {
            CancellationToken = new CancellationTokenSource();
            while (!CancellationToken.IsCancellationRequested)
            {

                CancellationToken.Token.ThrowIfCancellationRequested();
                await Task.Delay(1000, CancellationToken.Token).ContinueWith(async (arg) => {
                    if (!CancellationToken.Token.IsCancellationRequested)
                    {
                        CancellationToken.Token.ThrowIfCancellationRequested();
                        await trackLocation();
                    }
                });

            }
        }

        async Task trackLocation()
        {

            var mainViewModel = MainViewModel.GetInstance();
            await geolocatorService.GetLocation();
            if (geolocatorService.Latitude != 0 && geolocatorService.Longitude != 0)
            {
                var location = new Location
                {
                    Vehiculo = mainViewModel.Driver.Vehiculo,
                    Latitud = (float)geolocatorService.Latitude,
                    Longitud = (float)geolocatorService.Longitude,
                    Ruta = MyRoute,
                    Placa = mainViewModel.Driver.Placa
                };
                //var urlAPI = Application.Current.Resources["URLAPI"].ToString();
                var urlAPI = Application.Current.Resources["URLAPI"].ToString();
                var response = await apiService.Put<Location>(
                    urlAPI,
                    "/locations",
                    key + ".json", 
                    location,
                    mainViewModel.Driver.Token);

            }

        }

        #endregion

    }
}
