using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using StopCovid19.Models;
using LivingThing.Core.Frameworks.Xamarin.Startup;

namespace StopCovid19.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            MenuPages.Add((int)MenuItemType.World, (NavigationPage)Detail);
        }

        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.World:
                        MenuPages.Add(id, new NavigationPage(ServiceRegistration.Provider.GetService<WorldPage>()));
                        break;
                    case (int)MenuItemType.NCDC:
                        MenuPages.Add(id, new NavigationPage(ServiceRegistration.Provider.GetService<NCDCPage>()));
                        break;
                    case (int)MenuItemType.News:
                        MenuPages.Add(id, new NavigationPage(ServiceRegistration.Provider.GetService<NewsListPage>()));
                        break;
                    case (int)MenuItemType.About:
                        MenuPages.Add(id, new NavigationPage(ServiceRegistration.Provider.GetService<AboutPage>()));
                        break;
                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}