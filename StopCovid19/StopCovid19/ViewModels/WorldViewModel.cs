using LivingThing.Core.Frameworks.Serialization;
using Newtonsoft.Json;
using StopCovid19.Models;
using StopCovid19.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace StopCovid19.ViewModels
{
    public class WorldViewModel:INotifyPropertyChanged
    {
        private JsonSerializer _serializer = new JsonSerializer();

        public WorldViewModel()
        {
            Load();
            Reload = new Command(Load);
        }

        public CountryViewModel[] Countries { get; set; } = new CountryViewModel[0];

        public Summary[] Summary { get; set; }

        public bool Loading { get; set; }
        public bool Loaded { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError => ErrorMessage != null;
        public ICommand Reload { get; }

        Covid19Model[] combined;
        public Covid19Model[] Combined
        {
            get
            {
                if (combined != null && combined.Length > 0)
                    return combined;
                var groups = Countries.SelectMany(v => v.Models).GroupBy(v => v.Date);
                List<Covid19Model> list = new List<Covid19Model>();
                foreach (var g in groups)
                {
                    Covid19Model newModel = new Covid19Model()
                    {
                        Date = g.Key,
                        Confirmed = g.Sum(v => v.Confirmed),
                        Deaths = g.Sum(v => v.Deaths),
                        Recovered = g.Sum(v => v.Recovered)
                    };
                    list.Add(newModel);
                }
                return combined = list.OrderBy(d=>d.Date).ToArray();
            }
        }

        public int ChartWidth => Combined.Length * 60;

        public int TotalConfirmed => Combined.LastOrDefault()?.Confirmed ?? 0;
        public int TotalRecovered => Combined.LastOrDefault()?.Recovered ?? 0;
        public int TotalDeaths => Combined.LastOrDefault()?.Deaths ?? 0;
        public int TotalActive => Combined.LastOrDefault()?.Active ?? 0;

        public event PropertyChangedEventHandler PropertyChanged;
        public Action<CountryViewModel> ToPage { get; set; }

        async void Load()
        {
            //Loading = true;
            try
            {
                combined = null;
                var client = new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
                });
                //client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                var response = await client.GetAsync("https://pomber.github.io/covid19/timeseries.json");
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var json = new JsonTextReader(reader))
                {
                    var data = _serializer.Deserialize<Dictionary<string, Covid19Model[]>>(json);
                    //var json = await response.Content.ReadAsStringAsync();
                    //data = json.DeSerialize<Dictionary<string, Covid19Model[]>>();
                    Countries = data.OrderBy(k => k.Key).Select(kv =>
                    {
                        CountryViewModel vm = null;
                        vm = new CountryViewModel(kv.Key, kv.Value)
                        {
                            Details = new Command(() =>
                            {
                                ToPage(vm);
                            })
                        };
                        return vm;
                    }).ToArray();
                    Summary = new Summary[]
                    {
                            new Summary(){ Label = "TOTAL CONFIRMED", Count = TotalConfirmed, CountTextColor=Color.Coral },
                            new Summary(){ Label = "ACTIVE CASES", Count = TotalActive, CountTextColor=Color.Coral },
                            new Summary(){ Label = "TOTAL RECOVERIES", Count = TotalRecovered, CountTextColor=Color.FromHex("#00AEEF") },
                            new Summary(){ Label = "TOTAL DEATHS", LabelBackgroundColor=Color.Red, LabelTextColor=Color.White, Count = TotalDeaths, CountBackgroundColor = Color.FromHex("#FCAC00"), CountTextColor = Color.Red }
                    };
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Combined)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalConfirmed)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalRecovered)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalDeaths)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalActive)));
                }
                ErrorMessage = null;
            }
            catch (Exception e)
            {
                ErrorMessage = "Network error";
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasError)));
            Loading = false;
            Loaded = true;
        }
    }
}
