﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        #region Constructors
        public DriverPage()
        {
            InitializeComponent();
            geolocatorService = new GeolocatorService();
            MoveToCurrentLocation();
        }
        #endregion

        #region Methods
        async void MoveToCurrentLocation()
        {
            await geolocatorService.GetLocation();
            if (geolocatorService.Latitude != 0 && geolocatorService.Longitude != 0)
            {
                var position = new Position(geolocatorService.Latitude, geolocatorService.Longitude);
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMiles(.3)));
            }
        }
        #endregion
    }
}