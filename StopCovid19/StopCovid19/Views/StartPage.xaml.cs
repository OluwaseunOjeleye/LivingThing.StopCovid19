using LivingThing.Core.Frameworks.Common.String;
using LivingThing.Core.Frameworks.Xamarin.Startup;
using LivingThing.Core.IdentityManager.Models;
using LivingThing.Core.IdentityManager.Services;
using LivingThing.Core.Thing.Tracking.Common.Models;
using LivingThing.Core.Thing.Tracking.Common.Services;
using LivingThing.Interface.Common.Configuration;
using LivingThing.Interface.Common.RPC;
using StopCovid19.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StopCovid19.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartPage : ContentPage
    {
        public StartPage(App app)
        {
            InitializeComponent();
            BindingContext = ViewModel;
            ViewModel.Done = new Command(() =>
            {
                var page = new MainPage();
                app.MainPage = page;
            });
        }

        public StartViewModel ViewModel { get; } = new StartViewModel();
    }
}