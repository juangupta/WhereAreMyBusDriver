using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using WhereAreMyBusDriver.Services;
using Xamarin.Forms;
using WhereAreMyBusDriver.Views;
using WhereAreMyBusDriver.Models;

namespace WhereAreMyBusDriver.ViewModels
{
    public class LoginViewModel: INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private string email;
        private string contraseña;
        private bool isRunning;
        private bool isEnabled;
        private bool isRemembered;

        private ApiService apiService;
        private DialogService dialogService;

        #endregion

        #region Properties
        public string Email
        {
            set
            {
                if (email != value)
                {
                    email = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Email"));
                }
            }
            get
            {
                return email;
            }
        }

        public string Contraseña
        {
            set
            {
                if (contraseña != value)
                {
                    contraseña = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Contraseña"));
                }
            }
            get
            {
                return contraseña;
            }
        }

        public bool IsRunning
        {
            set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRunning"));
                }
            }
            get
            {
                return isRunning;
            }
        }

        public bool IsEnabled
        {
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEnabled"));
                }
            }
            get
            {
                return isEnabled;
            }
        }

        public bool IsRemembered
        {
            set
            {
                if (isRemembered != value)
                {
                    isRemembered = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRemembered"));
                }
            }
            get
            {
                return isRemembered;
            }
        }
        #endregion

        #region Constructor
        public LoginViewModel()
        {
            Email = "juangupta@gmail.com";
            Contraseña = "123456";
            IsEnabled = true;
            IsRemembered = true;
            apiService = new ApiService();
            dialogService = new DialogService();
        }
        #endregion
        #region Commands
        public ICommand LoginCommand { get { return new RelayCommand(Login); } }

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await dialogService.ShowMessage("Error", "Ingrese su email.");
                return;
            }

            if (string.IsNullOrEmpty(Contraseña))
            {
                await dialogService.ShowMessage("Error", "Ingrese su contraseña.");
                return;
            }
            IsEnabled = false;
            IsRunning = true;

            //var checkConnection = await apiService.CheckConnection();
            //if (!checkConnection.IsSuccess)
            //{
            //    IsRunning = false;
            //    await dialogService.ShowMessage("Error", checkConnection.Message);
            //    return;

            //}

            var urlAPIAuth = Application.Current.Resources["URLAPIAUTH"].ToString();
            var apiKey = Application.Current.Resources["APIKEY"].ToString();
            var uri = "/identitytoolkit/v3/relyingparty/verifyPassword?key=" + apiKey;
            var token = await apiService.GetToken(
                urlAPIAuth,
                uri,
                Email,
                Contraseña);

            if (token == null)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error",
                    "Email o contraseña incorrectos.");
                Contraseña = null;
                return;
            }

            if (string.IsNullOrEmpty(token.IdToken))
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", "Error realizando la autenticaación, intente nuevamente");
                Contraseña = null;
                return;
            }

            var urlAPI = Application.Current.Resources["URLAPI"].ToString();
            var response = await apiService.GetByField<Driver>(
                urlAPI,
                "/drivers.json",
                "email",
                Email,
                token.IdToken);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", "Problemas consultando la información del usuario.");
                return;
            }

            IsRunning = false;
            IsEnabled = true;

            var driver = (Driver)response.Result;
            driver.Token = token.IdToken;
            //employee.AccessToken = token.AccessToken;
            //employee.IsRemembered = IsRemembered;
            //employee.Password = Password;
            //employee.TokenExpires = token.Expires.ToLocalTime();
            //employee.TokenType = token.TokenType;
            //dataService.DeleteAllAndInsert(employee);
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Driver = driver;
            //mainViewModel.RegisterDevice();
            //navigationService.SetMainPage("MasterPage");
            mainViewModel.myDriver = new DriverViewModel();
            App.Current.MainPage = new NavigationPage(new DriverPage());
            //await App.Navigator.PushAsync(new DriverPage());
        }

        //public ICommand LoginFacebookCommand { get { return new RelayCommand(LoginFacebook); } }

        //private void LoginFacebook()
        //{
        //    navigationService.SetMainPage("LoginFacebookPage");
        //}

        #endregion
    }
}
