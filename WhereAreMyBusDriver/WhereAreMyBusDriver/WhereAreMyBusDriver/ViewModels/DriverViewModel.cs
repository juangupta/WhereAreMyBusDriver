using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        #endregion

        #region Properties
        public ObservableCollection<Route> Rutas { get;set;}
        public string MyRoute { get; set; }
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
            IsEnabledStart = false;
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
                IsEnabledStart = true;
                await dialogService.ShowMessage("Error", "Problemas registrando el viaje.");

            }

            var location = (Location)response.Result;
        }

        public ICommand EndCommand
        {
            get { return new RelayCommand(End); }
        }

        private async void End()
        {

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
        #endregion

    }
}
