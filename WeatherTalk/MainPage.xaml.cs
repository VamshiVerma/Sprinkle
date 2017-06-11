using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using Windows.Media.SpeechSynthesis;
using Windows.System.Profile;
using System.ComponentModel;
using Windows.UI.ViewManagement;
using Windows.Devices.Geolocation;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WeatherTalk
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {

        public string DeviceFamily = "";
        public string Dimensions { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // Define a member variable for storing the signed-in user. 
        //private MobileServiceUser user;

        // Define the OpenWeatherMap service object for REST weather data calls
        OpenWeatherMapService owms;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            owms = new OpenWeatherMapService();
            
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons.BackPressed"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                //UIViewSettings uiv = new UIViewSettings();
                string uim = UIViewSettings.GetForCurrentView().UserInteractionMode.ToString();
                DeviceFamily = "Device Family: " + AnalyticsInfo.VersionInfo.DeviceFamily + " (" + uim + ")";
                this.SizeChanged += MainPage_SizeChanged;
            }
        }
        
       

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var currentWidth = Window.Current.Bounds.Width;
            var currentHeight = Window.Current.Bounds.Height;
            Dimensions = string.Format(
              "Current Window Size: {0} x {1}",
              (int)currentWidth,
              (int)currentHeight);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Dimensions)));
            }
        }

        private async void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                // Do something with the back button here
                var dlg = new Windows.UI.Popups.MessageDialog("You tapped the Back button!!!.");
                await dlg.ShowAsync();
                rootFrame.GoBack();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           
            string location = e.Parameter as string;
            if (location.Length > 0)
            {
                ShowWeather(location);
            }
        }

        private async void ButtonLookup_Click(object sender, RoutedEventArgs e)
        {
            string location = txtLocation.Text.Trim();

            if (location.Length == 0)
            {
                var dlg = new Windows.UI.Popups.MessageDialog("Make sure you provide a city name and state before asking for a weather report.");
                await dlg.ShowAsync();
            }

            ShowWeather(location);
        }

        public async void ShowWeather(string location)
        {
            prgActivity.IsActive = true;
            try
            {

                App.TelemetryClient.TrackEvent("ShowWeather", new Dictionary<string, string>()
                {
                    { "Location", location }
                });

                var wr = await owms.GetWeather(location);
                if (wr != null)
                {
                    var weatherText = "The current temperature in {0} is {1}°C, with a high today of {2}°C and a low of {3}°C.";
                    string weatherMessage = string.Format(weatherText, wr.name, (int)wr.main.temp, (int)wr.main.temp_max, (int)wr.main.temp_min);
                    lblTempHigh.Text = string.Format("High: {0}°C", (int)wr.main.temp_max);
                    lblTempLow.Text = string.Format("Low: {0}°C", (int)wr.main.temp_min);
                    lblTemp.Text = string.Format("{0}°", (int)wr.main.temp);

                    hi.Text = string.Format("{0}",wr.weather[0].description);
                    lblLocation.Text = wr.name;
                    ReadText(weatherMessage);

                    // Save this as a favorite city in Azure
                    //Location favorite = new Location { Name = wr.name };
                    //Location favorite = new Location { Name = wr.name, UserId = user.UserId };
                    //await App.MobileService.GetTable<Location>().InsertAsync(favorite);
                }
            }
            catch (Exception exc)
            {
                var dlg = new Windows.UI.Popups.MessageDialog("Opps! Something went wrong getting the latest weather info. That can't be good...");
                await dlg.ShowAsync();
            }
            prgActivity.IsActive = false;
        }

       

        // Quickly adds Text-to-Speech to the app using Cortana's default voice
        private async void ReadText(string mytext)
        {
            //Reminder: You need to enable the Microphone capabilitiy in Windows Phone projects
            //Reminder: Add this namespace in your using statements
            //using Windows.Media.SpeechSynthesis;

            // The media object for controlling and playing audio.
            MediaElement mediaplayer = new MediaElement();

            // The object for controlling the speech synthesis engine (voice).
            using (var speech = new SpeechSynthesizer())
            {
                //Retrieve the first female voice
                speech.Voice = SpeechSynthesizer.AllVoices
                    .First(i => (i.Gender == VoiceGender.Female && i.Description.Contains("United States")));
                // Generate the audio stream from plain text.
                SpeechSynthesisStream stream = await speech.SynthesizeTextToStreamAsync(mytext);

                // Send the stream to the media object.
                mediaplayer.SetSource(stream, stream.ContentType);
                mediaplayer.Play();
            }
        }
        string Lat, Lng;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void btnGetWeather_Click(object sender, RoutedEventArgs e)
        {
            var x = await random.Randomf();
            if(x!=null)
            {
                fact.Text = string.Format(x.text);
            }





            prgActivity.IsActive = true;
            var geoLocator = new Geolocator();
            geoLocator.DesiredAccuracy = PositionAccuracy.High;
            Geoposition pos = await geoLocator.GetGeopositionAsync();

            Lat = pos.Coordinate.Point.Position.Latitude.ToString();
            Lng = pos.Coordinate.Point.Position.Longitude.ToString();

            var data = await OpenWeatherMapService.GetWeather2(Lat, Lng);
            if (data != null)
            {
                txtCity.Text = $"{data.name},{data.sys.country}";
               // txtLastUpdate.Text = $"Last updated : {DateTime.Now.ToString("dd MMMM yyyy HH:mm")}";

                // BitmapImage image = new BitmapImage(new Uri($"http://openweathermap.org/img/w/{data.weather[0].icon}.png", UriKind.Absolute));
                // imgWeather.Source = image;

                txtDescription.Text = $"{data.weather[0].description}";
                txtHumidity.Text = $"{data.main.humidity}% Humidity";
                //txtsea.Text = $"sealevel : {data.main.sea_level}";

             //   txtTime.Text = $"{OpenWeatherMapService.ConvertUnixTimeToDateTime(data.sys.sunrise).ToString("HH:mm")}/ {OpenWeatherMapService.ConvertUnixTimeToDateTime(data.sys.sunset).ToString("HH:mm")}";

                txtCel.Text = $"{data.main.temp} °C";

                var weatherText = "Hey there!,The current temperature in {0} is {1}°C, Humidity percent is about {2} ,  its mostly {3},!,Stay cool!";
                string weatherMessage = string.Format(weatherText, data.name, (int)data.main.temp, (int)data.main.humidity, data.weather[0].description);
                ReadText(weatherMessage);

            }
            prgActivity.IsActive = false;
        }




    }
}
