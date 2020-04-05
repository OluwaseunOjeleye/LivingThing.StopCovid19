using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace StopCovid19.ViewModels
{
    public class AboutViewModel :INotifyPropertyChanged
    {
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://xamarin.com"));
        }

        public ICommand OpenWebCommand { get; }
        public string Title { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}