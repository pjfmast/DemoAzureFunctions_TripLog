using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TripLog.Models;

namespace TripLog.Services {
    public class LocationService : ILocationService {
        public async Task<GeoCoords> GetGeoCoordinatesAsync() {
            var location = await Xamarin.Essentials.Geolocation.GetLocationAsync();

            return new GeoCoords {
                Latitude = location.Latitude,
                Longitude = location.Longitude
            };
        }
    }
}
