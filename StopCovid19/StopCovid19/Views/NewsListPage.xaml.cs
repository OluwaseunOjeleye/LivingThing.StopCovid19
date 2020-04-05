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
    public partial class NewsListPage : ContentPage
    {
        public NewsListPage(NewsViewModel viewModel)
        {
            ViewModel = viewModel;
            BindingContext = ViewModel;
            ViewModel.OnClicked = new Command(() =>
            {
                //var page = new NewsPage(article.Url);
                //Navigation.PushAsync(page);
            });
            ViewModel.OnTap = (m) =>
            {
                var page = new NewsPage(m);
                Navigation.PushAsync(page);
            };
            InitializeComponent();
        }

        public NewsViewModel ViewModel { get; }

    }
}