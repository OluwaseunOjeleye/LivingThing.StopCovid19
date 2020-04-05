using LivingThing.Core.Frameworks.Serialization;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using StopCovid19.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace StopCovid19.ViewModels
{
    public class NewsViewModel : INotifyPropertyChanged
    {
        static string API_KEY = "33362daa31574616a5acb36cf70e0108";

        public NewsViewModel()
        {
            Load();
            Reload = new Command(() =>
            {
                page++;
                Load();
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Loading { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError => ErrorMessage != null;

        public ICommand OnClicked { get; set; }
        public Action<NewsAPIArticle> OnTap { get; set; }

        public NewsAPIArticle[] Articles { get; set; }
        public ICommand Reload { get; set; }
        int page = 1;
        int total = 0;
        async void Load()
        {
            Loading = true;
            try
            {
                var address = $"https://newsapi.org/v2/top-headlines?country=NG&q=covid-19&apiKey={API_KEY}&pageSize=100&page={page}";
                var httpClient = new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
                });
                var response = await httpClient.GetAsync(address);
                var bytes = await response.Content.ReadAsByteArrayAsync();
                var json = Encoding.ASCII.GetString(bytes);
                var model = json.DeSerialize<NewsAPIResponseModel>();
                if (model.Status == "ok")
                {
                    foreach (var a in model.Articles)
                    {
                        a.Click = new Command(() =>
                        {
                            OnTap(a);
                        });
                    }
                    Articles = model.Articles;
                    total = model.TotalResults;
                }
                //var newsApiClient = new NewsApiClient(API_KEY);
                //var articlesResponse = await newsApiClient.GetEverythingAsync(new EverythingRequest
                //{
                //    Q = "Coronavirus",
                //    SortBy = SortBys.Popularity,
                //    Language = Languages.EN,
                //    From = DateTime.Today,
                //    Page = page,
                //    //PageSize = 100
                //});
                //Articles = articlesResponse.Articles.ToArray();
                ErrorMessage = null;
            }
            catch (Exception e)
           {
                ErrorMessage = "Network error";
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasError)));
            Loading = false;
            //if (articlesResponse.Status == Statuses.Ok)
            //{
            //    // total results found
            //    Console.WriteLine(articlesResponse.TotalResults);
            //    // here's the first 20
            //    foreach (var article in articlesResponse.Articles)
            //    {
            //        // title
            //        Console.WriteLine(article.Title);
            //        // author
            //        Console.WriteLine(article.Author);
            //        // description
            //        Console.WriteLine(article.Description);
            //        // url
            //        Console.WriteLine(article.Url);
            //        // image
            //        Console.WriteLine(article.UrlToImage);
            //        // published at
            //        Console.WriteLine(article.PublishedAt);
            //    }
            //}
        }
    }
}
