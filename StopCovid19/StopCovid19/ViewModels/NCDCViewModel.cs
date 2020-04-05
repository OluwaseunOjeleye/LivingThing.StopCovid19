using HtmlAgilityPack;
using StopCovid19.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace StopCovid19.ViewModels
{
    public class State
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
    public class NCDCViewModel : INotifyPropertyChanged
    {
        public NCDCViewModel()
        {
            Load();
            Reload = new Command(() => Load());
        }

        public ICommand Reload { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Loaded { get; set; }
        public bool Loading { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError => ErrorMessage != null;
        public Summary[] Summary { get; set; } = new Summary[] 
        {
            new Summary(){ Label = "TOTAL CONFIRMED", Count = -1 },
            new Summary(){ Label = "ACTIVE CASES", Count = -1 },
            new Summary(){ Label = "TOTAL DISCHARGED", Count = -1 },
            new Summary(){ Label = "TOTAL DEATH", LabelBackgroundColor=Color.Red, LabelTextColor=Color.White, Count = -1, CountBackgroundColor = Color.FromHex("#FCAC00"), CountTextColor = Color.Black }
        };

        public State[] States { get; set; } = new State[]
        {
            new State(){ Name = "Abuja", Count=1  }
        };

        void Load()
        {
            Task.Run(async () =>
            {
                try
                {
                    Loading = true;
                    var web = new HtmlWeb();
                    HttpClient client = new HttpClient(new HttpClientHandler
                    {
                        AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
                    });
                    var page = await client.GetAsync("https://covid19.ncdc.gov.ng/");
                    //var doc = web.Load("https://covid19.ncdc.gov.ng/");
                    var doc = new HtmlDocument();
                    doc.Load(await page.Content.ReadAsStreamAsync());
                    var nodes = doc.DocumentNode.SelectNodes("//table[@id='custom1']//td");
                    string key = null;
                    foreach (var node in nodes)
                    {
                        if (key == null)
                        {
                            key = node.InnerText.Trim().ToLower();
                        }
                        else
                        {
                            var val = node.InnerText.Trim();
                            int count;
                            if (!int.TryParse(val, out count))
                            {
                                break;
                            }
                            if (key.Contains("confirmed"))
                            {
                                Summary[0].Count = count;
                            }else if (key.Contains("discharge"))
                            {
                                Summary[2].Count = count;
                            }
                            else if (key.Contains("death"))
                            {
                                Summary[3].Count = count;
                            }
                            key = null;
                        }
                    }
                    Summary[1].Count = Summary[0].Count - Summary[2].Count - Summary[3].Count;
                    Summary = Summary.ToArray();

                    nodes = doc.DocumentNode.SelectNodes("//table[@id='custom3']//td");
                    string name = null;
                    string value = null;
                    List<State> states = new List<State>();
                    foreach(var node in nodes)
                    {
                        if (name == null)
                        {
                            name = node.InnerText.Trim();
                        }
                        else
                        {
                            value = node.InnerText.Trim();
                            int count;
                            if (!int.TryParse(value, out count))
                            {
                                break;
                            }
                            State s = new State() { Name = name, Count = count };
                            states.Add(s);
                            name = null;
                            value = null;
                        }
                    }
                    States = states.ToArray();
                    Loaded = true;
                    ErrorMessage = null;
                }catch(Exception e)
                {
                    ErrorMessage = "Network error";
                }
                finally
                {
                    Loading = false;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasError)));
            });
        }

    }
}
