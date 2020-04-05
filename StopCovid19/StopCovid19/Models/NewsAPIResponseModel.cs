using System;
using System.Collections.Generic;
using System.Text;

namespace StopCovid19.Models
{
    public class NewsAPIResponseModel
    {
        public string Status { get; set; }
        public int TotalResults { get; set; }
        public NewsAPIArticle[] Articles { get; set; }
    }
}
