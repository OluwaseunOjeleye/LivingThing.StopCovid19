using LivingThing.Core.Frameworks.Xamarin.Startup;
using StopCovid19.Models;
using StopCovid19.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StopCovid19.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorldPage : ContentPage
    {
        public WorldPage(WorldViewModel viewModel)
        {
            ViewModel = viewModel;
            BindingContext = viewModel;
            ViewModel.ToPage = (page) =>
            {
                var vpage = new CountryPage(page);
                Navigation.PushAsync(vpage);
            };
            InitializeComponent();
        }

        public WorldPage()
        {
            ViewModel = ServiceRegistration.Provider.GetService< WorldViewModel>();
            ViewModel.ToPage = (page) =>
            {
                var vpage = new CountryPage(page);
                Navigation.PushAsync(vpage);
            };
            BindingContext = ViewModel;
            InitializeComponent();
        }

        public WorldViewModel ViewModel { get; }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            CountryViewModel vm = e.Item as CountryViewModel;
            var vpage = new CountryPage(vm);
            Navigation.PushAsync(vpage);
            (sender as ListView).SelectedItem = null;
        }
    }
}