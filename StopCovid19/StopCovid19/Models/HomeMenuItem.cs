using System;
using System.Collections.Generic;
using System.Text;

namespace StopCovid19.Models
{
    public enum MenuItemType
    {
        World,
        NCDC,
        News,
        About
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
