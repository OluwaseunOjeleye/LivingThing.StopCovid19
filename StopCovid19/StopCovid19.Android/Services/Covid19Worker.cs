using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Work;
using Java.Lang;
using Java.Util.Concurrent;
using Xamarin.Essentials;

namespace StopCovid19.Droid.Services
{
    public class Covid19Worker : Worker//, IRunnable
    {
        static Android.Locations.Location lastLocation;
        static int workerCount;

        bool isPrimaryWorker;
        public Covid19Worker(Context context, WorkerParameters workerParameters) : base(context, workerParameters)
        {
            isPrimaryWorker = workerCount == 0;
            workerCount++;
            //var listener = Covid19LocationListener.Instance;
        }

        void UpdateLocation(Android.Locations.Location location)
        {
            var id = SecureStorage.GetAsync("clientId").Result;
            if (!string.IsNullOrEmpty(id) && location != null && (lastLocation == null || location.DistanceTo(lastLocation) > 20.0))
            {
                //Toast.MakeText(Application.Context, "Hmm", ToastLength.Short);
                var address = $"http://192.168.1.102:5166?id={id}&timestamp={location.Time/1000}&lat={location.Latitude}&lon={location.Longitude}&altitude={location.Altitude}&speed={location.Speed}&bearing={location.Bearing}&accuracy={location.Accuracy}";
                //var address = $"http://192.168.1.103:5166?id=1234&timestamp={DateTime.UtcNow}&lat=1.2&lon=2.3&altitude=1&speed=0&bearing=0&accuracy=0";
                var client = new HttpClient();
                var content = new StringContent("", Encoding.UTF8, "application/json");
                client.PostAsync(address, content);
                //lastLocation = location;
            }
        }
        public override Result DoWork()
        {
            try
            {
                var location = Covid19LocationListener.Instance.Location;
                if (location != null)
                {
                    UpdateLocation(location);
                }
                else
                {
                    Covid19LocationListener.Instance.GetLocationAsync().ContinueWith(task =>
                    {
                        if (task.Result != null)
                        {
                            UpdateLocation(task.Result);
                        }
                    });
                }
            }
            catch (System.Exception e)
            {

            }
            var worker = OneTimeWorkRequest.Builder.From<Covid19Worker>().SetInitialDelay(30, TimeUnit.Seconds).Build();
            WorkManager.Instance.Enqueue(worker);// .EnqueueUniqueWork("interleave", ExistingWorkPolicy.Keep, worker);
            return Result.InvokeSuccess();
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
    }
}