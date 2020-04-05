using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace StopCovid19.Droid.Services
{
    public class CloudUploader
    {
        //Covid19LocationListener locationListener;
        //AndroidNotificationManager notifier = new AndroidNotificationManager();

        //public CloudUploader(Covid19LocationListener listener)
        //{
        //    locationListener.LocationChanged += (s, e) =>
        //    {
        //        UploadLocation(e);
        //    };
        //}

        public async Task ExecutePost(CancellationToken token)
        {
            await Task.Run(async () => {
                while (1 < 2)
                {
                    GetLocation();
                    await Task.Delay(30000);
                }

            }, token);
        }

        Xamarin.Essentials.Location lastLocation;

        public CloudUploader()
        {
            //notifier.Initialize();
            //try
            //{
            //    notifier.ScheduleNotification("Note", "Something");
            //}
            //catch (Exception e)
            //{

            //}
        }

        //int x = 0;
        public async void UploadLocation(Xamarin.Essentials.Location location)
        {
            try
            {
                var id = await SecureStorage.GetAsync("clientId");
                var address = $"http://192.168.1.103:5166?id=1234{id}&timestamp={location.Timestamp.UtcDateTime}&lat={location.Latitude}&lon={location.Longitude}&altitude={location.Altitude}&speed={location.Speed}&bearing={location.Course}&accuracy={location.Accuracy}";
//                var address = $"http://sake.org.ng:5166?id={id}&timestamp={location.Timestamp.UtcDateTime}&lat={location.Latitude}&lon={location.Longitude}&altitude={location.Altitude}&speed={location.Speed}&bearing={location.Course}&accuracy={location.Accuracy}";
                //notifier.ScheduleNotification("Location", x.ToString());
                //x++;
                var client = new HttpClient();
                //client.BaseAddress = new Uri(address);
                var content = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(address, content);
                var result = await response.Content.ReadAsStringAsync();
                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                }
                lastLocation = location;
            }catch(Exception e)
            {

            }
        }

        public async void GetLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);
                //if (location != null && (lastLocation == null || location.CalculateDistance(lastLocation, DistanceUnits.Kilometers) > 20.0/1000))
                {
                    UploadLocation(location);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }
    }
}