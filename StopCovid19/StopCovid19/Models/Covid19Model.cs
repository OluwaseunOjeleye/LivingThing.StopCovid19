using System;
using System.Collections.Generic;
using System.Text;

namespace StopCovid19.Models
{
    public class Covid19Model
    {
        public DateTime Date { get; set; }
        public int Confirmed { get; set; }
        public int Deaths { get; set; }
        public int Recovered { get; set; }
        public int Active => Confirmed - Recovered - Deaths;
        public string FormatDate => Date.ToString("yyyy/MM/dd");
    }
}
