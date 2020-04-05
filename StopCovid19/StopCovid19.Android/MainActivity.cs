using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using StopCovid19.Droid.Services;
using Android;
using LivingThing.Core.Frameworks.Xamarin.Startup;
using LivingThing.Interface.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using LivingThing.Core.Frameworks.Xamarin.Configuration;
using Android.Telephony;
using AndroidX.Work;
using Java.Util.Concurrent;
using StopCovid19.Startup;

namespace StopCovid19.Droid
{
    [Activity(Label = "Stop Covid-19", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        const int RequestLocationId = 0;

        readonly string[] LocationPermissions =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            //var listener = Covid19LocationListener.Instance;
            //var worker = OneTimeWorkRequest.Builder.From<Covid19Worker>().SetInitialDelay(30, TimeUnit.Seconds).Build();
            var worker = PeriodicWorkRequest.Builder.From<Covid19Worker>(TimeSpan.FromMinutes(15)).Build();
            WorkManager.Instance.Enqueue(worker);

            //var intent = new Intent(ApplicationContext, typeof(CovidService));
            //StartService(intent);

            var serviceProvider = ServiceRegistration.Initialize((services) =>
            {
                services.AddSingleton<App>().AddSingleton<IConfigurationOwner>(s=> s.GetRequiredService<App>());
                services.RegisterAppFrameworkServices();
                services.RegisterAppServices();
            });

            //            new PhoneStateIntentReceiver(this, new ServiceStateHandler());
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(ServiceRegistration.Provider.GetRequiredService<App>());
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnStart()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    RequestPermissions(LocationPermissions, RequestLocationId);
                }
                else
                {
                    // Permissions already granted - display a message.
                }
            }
            base.OnStart();
        }
    }
}