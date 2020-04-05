using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace StopCovid19.Models
{
    public class NewsAPIArticle
    {
        public NewsAPISource Source { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string UrlToImage { get; set; }
        public DateTime? PublishedAt { get; set; }

        public ICommand Click { get; set; }
    }
}
