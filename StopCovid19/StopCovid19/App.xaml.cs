using Xamarin.Forms;
using StopCovid19.Views;
using LivingThing.Core.Frameworks.Xamarin.Configuration;
using Xamarin.Essentials;

namespace StopCovid19
{
    public partial class App : Application, IConfigurationOwner
    {
        public string Name => "StopCovid19";

        public App()
        {
            InitializeComponent();
            Load();
        }

        async void Load()
        {
            var clientId = await SecureStorage.GetAsync("clientId");
            if (string.IsNullOrEmpty(clientId))
                MainPage = new StartPage(this);
            else
                MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
