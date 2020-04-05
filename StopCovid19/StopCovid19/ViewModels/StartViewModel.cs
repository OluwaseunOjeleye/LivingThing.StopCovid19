using LivingThing.Core.Frameworks.Common.String;
using LivingThing.Core.Frameworks.Xamarin.Startup;
using LivingThing.Core.IdentityManager.Models;
using LivingThing.Core.IdentityManager.Services;
using LivingThing.Core.Thing.Tracking.Common.Models;
using LivingThing.Core.Thing.Tracking.Common.Services;
using LivingThing.Interface.Common.Configuration;
using LivingThing.Interface.Common.Message;
using LivingThing.Interface.Common.RPC;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace StopCovid19.ViewModels
{
    public class StartViewModel : INotifyPropertyChanged
    {
        public string EmailAddress { get; set; }
        public string Token { get; set; }
        string ExpectedToken { get; set; }
        DateTime TokenExpiry { get; set; }
        public bool HasValidationError => ErrorMessage != null;
        public bool IsLoading { get; set; }

        string errorMessage;
        public string ErrorMessage
        {
            get => errorMessage;
            set {
            if (value != errorMessage)
                {
                    errorMessage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasValidationError)));
                }
            }
        }
        public ICommand Continue { get; }
        public ICommand Done { get; set; }
        int _stage;

        public StartViewModel()
        {
            Continue = new Command(() =>
            {
                Next();
            }, () => !IsLoading);
        }

        public int Stage
        {
            get => _stage;
            set
            {
                _stage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCollectingEmail)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCollectingToken)));
            }
        }
        public bool IsCollectingEmail => Stage == 0;
        public bool IsCollectingToken => Stage == 1|| Stage == 2;

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task Next()
        {
            IsLoading = true;
            do
            {
                switch (Stage)
                {
                    case 0:
                        if (!string.IsNullOrEmpty(EmailAddress))
                        {
                            if (EmailAddress.IsValidEmail())
                            {
                                var remoteServiceFactory = ServiceRegistration.Provider.GetService<IServerRemoteService>();
                                var coreService = remoteServiceFactory.GetRemoteService<ICoreRemoteService>();
                                try
                                {
                                    var random = new Random();
                                    string token = "";
                                    for(int i = 0; i < 6; i++)
                                    {
                                        int n = random.Next(0, 9);
                                        token += n.ToString();
                                    }
                                    var verification = await coreService.SendMessage(new MessageConfiguration()
                                    {
                                        Channel = MessengerChannelTypes.Email,
                                        To = new string[] { EmailAddress },
                                        Title = "Stop Covid-19, Verify you Email",
                                        Body = $"Please provide this token <b>{token}</b> to validate your email.",
                                        FromEmail = "no-reply@sake.org.ng",
                                        FromName = "Stop Covid-19"
                                    });
                                    if (verification.Success)
                                    {
                                        ErrorMessage = null;
                                        ExpectedToken = token;
                                        TokenExpiry = DateTime.UtcNow.AddMinutes(5);
                                        Stage++;
                                    }
                                    else
                                    {
                                        ErrorMessage = verification.Message;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ErrorMessage = ex.Message;
                                }
                            }
                        }
                        break;
                    case 1:
                        if (DateTime.UtcNow < TokenExpiry)
                        {
                            if (ExpectedToken == Token)
                            {
                                ErrorMessage = null;
                                Stage++;
                                continue;
                            }
                            else
                            {
                                ErrorMessage = "Invalid Token";
                            }
                        }
                        else
                        {
                            ErrorMessage = "Token Expired. Please restart!!!";
                            Stage = 0;
                        }
                        break;
                    case 2:
                        {
                            var remoteServiceFactory = ServiceRegistration.Provider.GetService<IServerRemoteService>();
                            var trackingService = remoteServiceFactory.GetRemoteService<ITrackingRemoteService>();
                            var config = ServiceRegistration.Provider.GetService<IConfiguration>();
                            var dashboardId = config.GetValue<string>("Server/DashboardId");
                            try
                            {
                                var creation = await trackingService.AddDevice(new TrackingDeviceModel()
                                {
                                    Name = EmailAddress,
                                    UniqueId = Guid.NewGuid().ToString()
                                }, dashboardId);
                                if (creation.Success)
                                {
                                    await SecureStorage.SetAsync("clientId", creation.Data.UniqueId);
                                    Done.Execute(null);
                                    //Stage++;
                                    continue;
                                }
                                else
                                {
                                    ErrorMessage = creation.Message;
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                ErrorMessage = ex.Message;
                            }
                        }
                        break;
                    //case 3:
                    //    Done.Execute(null);
                    //    break;
                }
            } while (false);
            IsLoading = false;
        }
    }
}
