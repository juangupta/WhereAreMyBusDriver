using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WhereAreMyBusDriver.Services;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace WhereAreMyBusDriver.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DriverPage : ContentPage
    {
        #region Attributes
        GeolocatorService geolocatorService;
        #endregion

        #region Properties
        public CancellationTokenSource CancellationToken { get; set; }
        #endregion

        #region Constructors
        public DriverPage()
        {
            InitializeComponent();
            geolocatorService = new GeolocatorService();
            MoveToCurrentLocation();
            //Task.Run(async () => backgroundThread());
        }
        #endregion

        #region Methods
        public async void MoveToCurrentLocation()
        {
            await geolocatorService.GetLocation();
            if (geolocatorService.Latitude != 0 && geolocatorService.Longitude != 0)
            {
                var position = new Position(geolocatorService.Latitude, geolocatorService.Longitude);
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMiles(.3)));
            }
        }

        public async Task backgroundThread()
        {
            CancellationToken = new CancellationTokenSource();
            while (!CancellationToken.IsCancellationRequested)
            {

                CancellationToken.Token.ThrowIfCancellationRequested();
                await Task.Delay(10000, CancellationToken.Token).ContinueWith((arg) =>
                {
                    if (!CancellationToken.Token.IsCancellationRequested)
                    {
                        CancellationToken.Token.ThrowIfCancellationRequested();
                        MoveToCurrentLocation();
                    }
                });

            }
        }
        #endregion
    }
}