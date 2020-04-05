using Microsoft.Extensions.DependencyInjection;
using StopCovid19.ViewModels;
using StopCovid19.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace StopCovid19.Startup
{
    public static class Registration
    {
        public static void RegisterAppServices(this IServiceCollection services)
        {
            services.AddSingleton<NCDCViewModel>();
            services.AddSingleton<NewsViewModel>();
            services.AddSingleton<WorldViewModel>();
            services.AddSingleton<NCDCPage>();
            services.AddSingleton<WorldPage>();
            services.AddSingleton<NewsListPage>();
            services.AddSingleton<AboutPage>();
        }
    }
}
