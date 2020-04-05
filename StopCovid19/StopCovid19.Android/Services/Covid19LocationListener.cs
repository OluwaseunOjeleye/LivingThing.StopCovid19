using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Android.Gms.Common;
using Android.Gms.Location;
using System.Threading.Tasks;

namespace StopCovid19.Droid.Services
{
    public class Covid19LocationListener : LocationCallback, Android.Locations.ILocationListener
    {
        static Covid19LocationListener _instance;
        public static Covid19LocationListener Instance => _instance ??= new Covid19LocationListener();

        string locationProvider;
        LocationManager locationManager;
        FusedLocationProviderClient fusedLocationProviderClient;
        Covid19LocationListener()
        {
            if (IsGooglePlayServicesInstalled())
            {
                fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(Application.Context);
                fusedLocationProviderClient.GetLastLocationAsync().ContinueWith(task =>
                {
                    //Location = task.Result;
                });
                LocationRequest locationRequest = new LocationRequest()
                                  .SetPriority(LocationRequest.PriorityHighAccuracy)
                                  .SetInterval(60 * 1000 * 5)
                                  .SetFastestInterval(60 * 1000 * 2);
                try
                {
                    fusedLocationProviderClient.RequestLocationUpdatesAsync(locationRequest, this);
                }catch(Exception e)
                {

                }
            }
            else
            {
                locationManager = Application.Context.GetSystemService(Context.LocationService) as LocationManager;
                var locationCriteria = new Criteria();

                locationCriteria.Accuracy = Accuracy.Fine;
                locationCriteria.PowerRequirement = Power.Medium;

                locationProvider = locationManager.GetBestProvider(locationCriteria, true);
                if (locationManager.IsProviderEnabled(locationProvider))
                {
                    Location = locationManager.GetLastKnownLocation(locationProvider);
                    try
                    {
                        locationManager.RequestLocationUpdates(locationProvider, 10000, 20, this);
                    }catch(Exception e)
                    {

                    }
                }
            }
        }

        bool IsGooglePlayServicesInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Application.Context);
            if (queryResult == ConnectionResult.Success)
            {
                //Log.Info("MainActivity", "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                // Check if there is a way the user can resolve the issue
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                //Log.Error("MainActivity", "There is a problem with Google Play Services on this device: {0} - {1}",
                //          queryResult, errorString);

                // Alternately, display the error to the user.
            }

            return false;
        }
        public Location Location { get; set; }

        public Task<Location> GetLocationAsync()
        {
            if (fusedLocationProviderClient != null)
            {
                return fusedLocationProviderClient.GetLastLocationAsync();
            }
            else
            {
                return Task.FromResult(locationManager.GetLastKnownLocation(locationProvider));
            }
        }

        public EventHandler<Location> LocationChanged;
        public void OnLocationChanged(Location location)
        {
            if (location != null)
            {
                Location  = location;
                LocationChanged?.Invoke(this, location);
            }
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {

        }

        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {
        }

        public override void OnLocationResult(LocationResult result)
        {
            if (result.Locations.Any())
            {
                Location = result.Locations.Aggregate((r1, r2) =>
                {
                    if (r1.Accuracy < r2.Accuracy)
                        return r1;
                    return r2;
                });
            }
            else
            {
            }
        }
    }
}