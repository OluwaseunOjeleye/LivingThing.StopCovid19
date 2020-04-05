using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using StopCovid19.Models;

namespace StopCovid19.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsPage : ContentPage
    {
        public NewsPage(NewsAPIArticle article)
        {
            InitializeComponent();
            webView.Source = article.Url;
            BindingContext = article;
        }
    }
}