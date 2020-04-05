using StopCovid19.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace StopCovid19.ViewModels
{
    public class CountryViewModel : INotifyPropertyChanged
    {
        public CountryViewModel(string country, Covid19Model[] models)
        {
            Country = country;
            Models = models.Where(m => m.Confirmed > 0).ToArray();
            if (Models.Length == 0)
            {
                Models = new Covid19Model[] { models.Last() };
            }
            Summary = new Summary[]
            {
                new Summary(){ Label = "TOTAL CONFIRMED", Count = TotalConfirmed, CountTextColor=Color.Coral },
                new Summary(){ Label = "ACTIVE CASES", Count = TotalActive, CountTextColor=Color.Coral },
                new Summary(){ Label = "TOTAL RECOVERIES", Count = TotalRecovered, CountTextColor=Color.FromHex("#00AEEF") },
                new Summary(){ Label = "TOTAL DEATHS", LabelBackgroundColor=Color.Red, LabelTextColor=Color.White, Count = TotalDeaths, CountBackgroundColor = Color.FromHex("#FCAC00"), CountTextColor = Color.Red }
            };
        }

        public string Country { get; }
        public Covid19Model[] Models { get; }
        Covid19Model lastModel;
        public Covid19Model LastModel => lastModel ??= Models.LastOrDefault();

        public int TotalConfirmed => LastModel?.Confirmed ?? 0;
        public int TotalRecovered => LastModel?.Recovered ?? 0;
        public int TotalDeaths => LastModel?.Deaths ?? 0;
        public int TotalActive => LastModel?.Active ?? 0;

        public int ChartWidth => Models.Length * 60;

        public Summary[] Summary { get; set; }

        Covid19Model current;
        public Covid19Model Current => current ??= Models.LastOrDefault();
        public event PropertyChangedEventHandler PropertyChanged;
        public  ICommand Details { get; set; }
    }
}
