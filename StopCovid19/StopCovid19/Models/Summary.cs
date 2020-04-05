using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace StopCovid19.Models
{
    public class Summary
    {
        public string Label { get; set; }
        public int Count { get; set; }
        public Color LabelBackgroundColor { get; set; } = Color.FromHex("#FCAC00");
        public Color LabelTextColor { get; set; } = Color.FromHex("#000000");
        public Color CountBackgroundColor { get; set; } = Color.FromHex("#FFFFFF");
        public Color CountTextColor { get; set; } = Color.FromHex("#FF0000");

        public string CountString => Count < 0 ? "?" : Count.ToString();
    }


}
