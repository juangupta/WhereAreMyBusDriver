namespace WhereAreMyBusDriver.Services
{
    using System;
    using System.Threading.Tasks;
    using Plugin.Geolocator;

    public class GeolocatorService
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public async Task GetLocation()
        {
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                var location = await locator.GetPositionAsync(timeoutMilliseconds: 5000);
                Latitude = location.Latitude;
                Longitude = location.Longitude;

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
    }
}
