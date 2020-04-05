using HtmlAgilityPack;
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
    public partial class NCDCPage : ContentPage
    {
        public NCDCPage(NCDCViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;
        }

        public NCDCViewModel ViewModel { get; } 
    }
}