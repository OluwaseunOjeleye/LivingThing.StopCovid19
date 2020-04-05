using StopCovid19.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StopCovid19.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CountryPage : ContentPage
    {
        public CountryPage(CountryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            ViewModel = viewModel;
        }

        CountryViewModel ViewModel { get; }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    var entries = ViewModel.Models.Select(m => new Microcharts.Entry(m.Confirmed)
        //    {
        //        Label = m.FormatDate
        //    }).ToArray();
        //    var chart = new BarChart() { Entries = entries,  };

        //    //this.chartView.Chart = chart;
        //}
    }
}