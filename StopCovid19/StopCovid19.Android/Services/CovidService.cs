using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace StopCovid19.Droid.Services
{
    [Service]
    public class CovidService : Service
    {
        //Covid19LocationListener locationListener = new Covid19LocationListener();
        CancellationTokenSource _cts;
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Toast.MakeText(this, "Started", ToastLength.Long).Show();
            var t = new Java.Lang.Thread(async () =>
            {
                while (true)
                {
                    try
                    {
                        var address = $"http://192.168.1.103:5166?id=1234&timestamp={DateTime.UtcNow}&lat=1.2&lon=2.3&altitude=1&speed=0&bearing=0&accuracy=0";
                        var client = new HttpClient();
                        //client.BaseAddress = new Uri(address);
                        var content = new StringContent("", Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync(address, content);
                        var result = await response.Content.ReadAsStringAsync();
                    }catch(Exception e)
                    {

                    }
                    Thread.Sleep(30000);
                }
                //_cts = new CancellationTokenSource();

                //Task.Run(() =>
                //{
                //    try
                //    {
                //        var cloud = new CloudUploader();
                //        cloud.ExecutePost(_cts.Token).Wait();
                //    }
                //    catch (Android.Accounts.OperationCanceledException)
                //    {
                //    }
                //    finally
                //    {
                //        if (_cts.IsCancellationRequested)
                //        {

                //        }
                //    }

                //}, _cts.Token);
            });
            t.Start();
            return StartCommandResult.Sticky;
        }

        public override void OnTaskRemoved(Intent rootIntent)
        {
            Intent restartServiceIntent = new Intent(this, typeof(CovidService));
            //restartServiceIntent.setPackage(getPackageName());

            PendingIntent restartServicePendingIntent = PendingIntent.GetService(this, 1, restartServiceIntent, PendingIntentFlags.OneShot);
            AlarmManager alarmService = (AlarmManager)GetSystemService(Context.AlarmService);
            alarmService.Set(
                    AlarmType.ElapsedRealtime,
                    SystemClock.ElapsedRealtime() + 1000,
                    restartServicePendingIntent);

            base.OnTaskRemoved(rootIntent);
        }
        public override void OnDestroy()
        {
            Toast.MakeText(this, "Stopped", ToastLength.Long).Show();
            var intent = new Intent(ApplicationContext, typeof(CovidService));
            StartService(intent);
            base.OnDestroy();
        }
    }
}