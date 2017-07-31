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
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.Text.RegularExpressions;




// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WeatherTalk
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<Forecast> fc { get; set; }

        public string DeviceFamily = "";
        public string Dimensions { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // Define a member variable for storing the signed-in user. 
        //private MobileServiceUser user;

        // Define the OpenWeatherMap service object for REST weather data calls
        OpenWeatherMapService owms;

                DateTime now = DateTime.Now; // <-- Value is copied into local
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
            nation.Visibility = Visibility.Collapsed;
            zip.Visibility = Visibility.Collapsed;
            zipbutton.Visibility = Visibility.Collapsed;
            map.Visibility = Visibility.Collapsed;
            la.Visibility = Visibility.Collapsed;
            lo.Visibility = Visibility.Collapsed;
            txtLocation2.Visibility = Visibility.Collapsed;
            but.Visibility = Visibility.Collapsed;
            latbut.Visibility = Visibility.Collapsed;

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

            ShowWeather1(location);
        }

        public async void ShowWeather(string location)
        {
            prgActivity.IsActive = true;
            bool m = location.Any(char.IsDigit);
            if (m == true)
            {
                var dlg = new Windows.UI.Popups.MessageDialog("Make sure you provide a correct city name and state before asking for a weather report.");
                await dlg.ShowAsync();
                prgActivity.IsActive = false;

                return;
            }
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
        // Function to Check for AlphaNumeric.
       

        public async void ShowWeather1(string location)
        {
            prgActivity.IsActive = true;
            bool m = location.Any(char.IsDigit);
            if(m==true)
            {
                var dlg = new Windows.UI.Popups.MessageDialog("Make sure you provide a correct city name and state before asking for a weather report.");
                await dlg.ShowAsync();
                prgActivity.IsActive = false;

                return;
            }
            try
            {
                
                




                App.TelemetryClient.TrackEvent("ShowWeather", new Dictionary<string, string>()
                {
                    { "Location", location }
                });


                var wr = await owms.GetWeather(location);
                if (wr != null)
                {
                    var weatherText = "The current temperature in {0} is {1}°C, with a high today of {2}°C and a low of {3}°C, Humidity level is {4}, The velocity of wind is running at {5}";
                    string weatherMessage = string.Format(weatherText, wr.name, (int)wr.main.temp, (int)wr.main.temp_max, (int)wr.main.temp_min, (int)wr.main.humidity, (double)wr.wind.speed);
                    lblTempHigh.Text = string.Format("High: {0}°C", (double)wr.main.temp_max);
                    lblTempLow.Text = string.Format("Low: {0}°C", (double)wr.main.temp_min);
                    lblTemp.Text = string.Format("{0}°", (double)wr.main.temp);
                    // hi4.Text = $"{OpenWeatherMapService.ConvertUnixTimeToDateTime(wr.sys.sunrise).ToString("HH:mm~")}/ {OpenWeatherMapService.ConvertUnixTimeToDateTime(wr.sys.sunset).ToString("HH:mm")}";
                    hi4.Text = now.ToString();

                    hi.Text = string.Format("{0}", wr.weather[0].description);
                    hi2.Text = $"{wr.main.humidity}% Humidity";
                    hi1.Text = $"Wind Velocity : {wr.wind.speed} m/s";
                    hi3.Text = $"Pressure : {wr.main.pressure} ";
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
        public async void ShowWeatherx(string location)
        {
            prgActivity.IsActive = true;
            bool m = location.Any(char.IsDigit);
            if (m == true)
            {
                var dlg = new Windows.UI.Popups.MessageDialog("Make sure you provide a correct city name and state before asking for a weather report.");
                await dlg.ShowAsync();
                prgActivity.IsActive = false;

                return;
            }
            try
            {






                App.TelemetryClient.TrackEvent("ShowWeather", new Dictionary<string, string>()
                {
                    { "Location", location }
                });


                var wr = await owms.GetWeather(location);
                if (wr != null)
                {
                    var weatherText = "The current temperature in {0} is {1}°C, with a high today of {2}°C and a low of {3}°C, Humidity level is {4}, The velocity of wind is running at {5}";
                    string weatherMessage = string.Format(weatherText, wr.name, (int)wr.main.temp, (int)wr.main.temp_max, (int)wr.main.temp_min, (int)wr.main.humidity, (double)wr.wind.speed);
                    lblTempHigh.Text = string.Format("High: {0}°C", (double)wr.main.temp_max);
                    lblTempLow.Text = string.Format("Low: {0}°C", (double)wr.main.temp_min);
                    lblTemp.Text = string.Format("{0}°", (double)wr.main.temp);
                    // hi4.Text = $"{OpenWeatherMapService.ConvertUnixTimeToDateTime(wr.sys.sunrise).ToString("HH:mm~")}/ {OpenWeatherMapService.ConvertUnixTimeToDateTime(wr.sys.sunset).ToString("HH:mm")}";
                    hi4.Text = now.ToString();

                    hi.Text = string.Format("{0}", wr.weather[0].description);
                    hi2.Text = $"{wr.main.humidity}% Humidity";
                    hi1.Text = $"Wind Velocity : {wr.wind.speed} m/s";
                    hi3.Text = $"Pressure : {wr.main.pressure} ";
                    lblLocation.Text = wr.name;

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
        public async void ShowWeathery(string location)
        {
            prgActivity.IsActive = true;
            bool m = location.Any(char.IsDigit);
            if (m == true)
            {
                var dlg = new Windows.UI.Popups.MessageDialog("Make sure you provide a correct city name and state before asking for a weather report.");
                await dlg.ShowAsync();
                prgActivity.IsActive = false;

                return;
            }
            try
            {

                App.TelemetryClient.TrackEvent("ShowWeather", new Dictionary<string, string>()
                {
                    { "Location", location }
                });


                var data = await owms.GetWeather(location);

                if (data != null)
                {
                    txtCity.Text = $"{data.name},{data.sys.country}";
                    // txtLastUpdate.Text = $"Last updated : {DateTime.Now.ToString("dd MMMM yyyy HH:mm")}";

                    BitmapImage image = new BitmapImage(new Uri($"http://api.wunderground.com/api/9270a990901ad2ce/animatedradar/animatedsatellite/image.gif?num=5&delay=50&rad.maxlat=47.709&rad.maxlon=-69.263&rad.minlat=31.596&rad.minlon=-97.388&rad.width=640&rad.height=480&rad.rainsnow=1&rad.reproj.automerc=1&rad.num=5&sat.maxlat=47.709&sat.maxlon=-69.263&sat.minlat=31.596&sat.minlon=-97.388&sat.width=640&sat.height=480&sat.key=sat_ir4_bottom&sat.gtt=107&sat.proj=me&sat.timelabel=0&sat.num=5", UriKind.Absolute));
                    //hello.Source = image;

                    txtDescription.Text = $"{data.weather[0].description}";
                    txtHumidity.Text = $"{data.main.humidity}% Humidity";
                    z.Text = $"Wind : {data.wind.speed} m/s";
                    y.Text = $"Pressure : {data.main.pressure} ";
                    double f = data.wind.speed;
                    /*   string toTextualDescription(var f)
                       {
                           if (f > 337.5) return "Northerly";
                           if (f > 292.5) return "North Westerly";
                           if (f > 247.5) return "Westerly";
                           if (f > 202.5) return "South Westerly";
                           if (f > 157.5) return "Southerly";
                           if (f > 122.5) return "South Easterly";
                           if (f > 67.5) return "Easterly";
                           if (f > 22.5) { return "North Easterly"; }
                           return "Northerly";
                       }   */
                    // hi5.Text = $"{OpenWeatherMapService.ConvertUnixTimeToDateTime(data.sys.sunrise).ToString("HH:mm~")}/ {OpenWeatherMapService.ConvertUnixTimeToDateTime(data.sys.sunset).ToString("HH:mm")}";
                    hi5.Text = now.ToString();

                    txtCel.Text = $"{data.main.temp} °C";
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
        public static string Lat, Lng;

      

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(layer));

            BitmapImage image = new BitmapImage(new Uri($"http://api.wunderground.com/api/9270a990901ad2ce/animatedradar/animatedsatellite/image.gif?num=5&delay=50&rad.maxlat=47.709&rad.maxlon=-69.263&rad.minlat=31.596&rad.minlon=-97.388&rad.width=640&rad.height=480&rad.rainsnow=1&rad.reproj.automerc=1&rad.num=5&sat.maxlat=47.709&sat.maxlon=-69.263&sat.minlat=31.596&sat.minlon=-97.388&sat.width=640&sat.height=480&sat.key=sat_ir4_bottom&sat.gtt=107&sat.proj=me&sat.timelabel=0&sat.num=5", UriKind.Absolute));


        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(MainPage));
        }

        public async void zipbutton_Click(object sender, RoutedEventArgs e)
        {
           


                prgActivity.IsActive = true;


            var w = nation.Text;
            var l = zip.Text;
                var q = await owms.GetWeatherZ(l, w);


                if (q != null)

                {

                bool m = w.Any(char.IsLetter);
                bool xx = l.Any(char.IsDigit);
                if (m == false|| xx==false)
                {
                    var dlg = new Windows.UI.Popups.MessageDialog("Make sure you provide a correct details and state before asking for a weather report.");
                    await dlg.ShowAsync();
                    prgActivity.IsActive = false;

                    return;
                }



                txtCity.Text = $"{q.name},{q.sys.country}";
                    // txtLastUpdate.Text = $"Last updated : {DateTime.Now.ToString("dd MMMM yyyy HH:mm")}";

                    BitmapImage image = new BitmapImage(new Uri($"http://api.wunderground.com/api/9270a990901ad2ce/animatedradar/animatedsatellite/image.gif?num=5&delay=50&rad.maxlat=47.709&rad.maxlon=-69.263&rad.minlat=31.596&rad.minlon=-97.388&rad.width=640&rad.height=480&rad.rainsnow=1&rad.reproj.automerc=1&rad.num=5&sat.maxlat=47.709&sat.maxlon=-69.263&sat.minlat=31.596&sat.minlon=-97.388&sat.width=640&sat.height=480&sat.key=sat_ir4_bottom&sat.gtt=107&sat.proj=me&sat.timelabel=0&sat.num=5", UriKind.Absolute));
                   // hello.Source = image;
                //   hi5.Text = $"{q.sys.sunrise.ToString("HH:mm~")}/ {OpenWeatherMapService.ConvertUnixTimeToDateTime(q.sys.sunset).ToString("HH:mm")}";
                hi5.Text = now.ToString();

                txtDescription.Text = $"{q.weather[0].description}";
                    txtHumidity.Text = $"{q.main.humidity}% Humidity";
                    z.Text = $"Wind : {q.wind.speed} m/s";
                    y.Text = $"Pressure : {q.main.pressure} ";
                    double f = q.wind.speed;
                    /*   string toTextualDescription(var f)
                       {
                           if (f > 337.5) return "Northerly";
                           if (f > 292.5) return "North Westerly";
                           if (f > 247.5) return "Westerly";
                           if (f > 202.5) return "South Westerly";
                           if (f > 157.5) return "Southerly";
                           if (f > 122.5) return "South Easterly";
                           if (f > 67.5) return "Easterly";
                           if (f > 22.5) { return "North Easterly"; }
                           return "Northerly";
                       }   
                    //   txtTime.Text = $"{OpenWeatherMapService.ConvertUnixTimeToDateTime(data.sys.sunrise).ToString("HH:mm~")}/ {OpenWeatherMapService.ConvertUnixTimeToDateTime(data.sys.sunset).ToString("HH:mm")}";
                    */
                    txtCel.Text = $"{q.main.temp} °C";

                    var weatherText = "The current temperature in {0} is {1}°C, Humidity percent is about {2} , Winds running at {3} meter per second, its mostly {4},!";
                    string weatherMessage = string.Format(weatherText, q.name, (int)q.main.temp, (int)q.main.humidity, (double)q.wind.speed, q.weather[0].description);
                    ReadText(weatherMessage);

                }
                prgActivity.IsActive = false;
            }

        private void c(object sender, RoutedEventArgs e)
        {
            nation.Visibility = Visibility.Collapsed;
            zip.Visibility = Visibility.Collapsed;
            zipbutton.Visibility = Visibility.Collapsed;
            map.Visibility = Visibility.Collapsed;
            txtLocation.Visibility = Visibility.Visible;
            la.Visibility = Visibility.Collapsed;
            lo.Visibility = Visibility.Collapsed;
            latbut.Visibility = Visibility.Collapsed;
            txtLocation2.Visibility = Visibility.Collapsed;
            but.Visibility = Visibility.Collapsed;
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            nation.Visibility = Visibility.Collapsed;
            zip.Visibility = Visibility.Collapsed;
            zipbutton.Visibility = Visibility.Collapsed;
            map.Visibility = Visibility.Visible;
            txtLocation.Visibility = Visibility.Collapsed;
            la.Visibility = Visibility.Collapsed;
            lo.Visibility = Visibility.Collapsed;
            latbut.Visibility = Visibility.Collapsed;
            txtLocation2.Visibility = Visibility.Collapsed;
            but.Visibility = Visibility.Collapsed;
        }

        private void latlon(object sender, RoutedEventArgs e)
        {
            nation.Visibility = Visibility.Collapsed;
            zip.Visibility = Visibility.Collapsed;
            zipbutton.Visibility = Visibility.Collapsed;
            map.Visibility = Visibility.Collapsed;
            txtLocation.Visibility = Visibility.Collapsed;
            la.Visibility = Visibility.Visible;
            lo.Visibility = Visibility.Visible;
            latbut.Visibility = Visibility.Visible;
            txtLocation2.Visibility = Visibility.Collapsed;
            but.Visibility = Visibility.Collapsed;

        }

        private void Zip_Click(object sender, RoutedEventArgs e)
        {
            nation.Visibility = Visibility.Visible;
            zip.Visibility = Visibility.Visible;
            zipbutton.Visibility = Visibility.Visible;
            map.Visibility = Visibility.Collapsed;
            txtLocation.Visibility = Visibility.Collapsed;
            la.Visibility = Visibility.Collapsed;
            lo.Visibility = Visibility.Collapsed;
            latbut.Visibility = Visibility.Collapsed;
            txtLocation2.Visibility = Visibility.Collapsed;
            but.Visibility = Visibility.Collapsed;

        }
        private void my(object sender, RoutedEventArgs e)
        {
            nation.Visibility = Visibility.Collapsed;
            zip.Visibility = Visibility.Collapsed;
            zipbutton.Visibility = Visibility.Collapsed;
            map.Visibility = Visibility.Collapsed;
            txtLocation.Visibility = Visibility.Visible;
            la.Visibility = Visibility.Collapsed;
            lo.Visibility = Visibility.Collapsed;
            latbut.Visibility = Visibility.Collapsed;
            txtLocation2.Visibility = Visibility.Visible;
            but.Visibility = Visibility.Visible;

        }
        public async void btnGetWeather_Click(object sender, RoutedEventArgs e)
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

                 BitmapImage image = new BitmapImage(new Uri($"http://api.wunderground.com/api/9270a990901ad2ce/animatedradar/animatedsatellite/image.gif?num=5&delay=50&rad.maxlat=47.709&rad.maxlon=-69.263&rad.minlat=31.596&rad.minlon=-97.388&rad.width=640&rad.height=480&rad.rainsnow=1&rad.reproj.automerc=1&rad.num=5&sat.maxlat=47.709&sat.maxlon=-69.263&sat.minlat=31.596&sat.minlon=-97.388&sat.width=640&sat.height=480&sat.key=sat_ir4_bottom&sat.gtt=107&sat.proj=me&sat.timelabel=0&sat.num=5", UriKind.Absolute));
                 //hello.Source = image;
                
                txtDescription.Text = $"{data.weather[0].description}";
                  txtHumidity.Text = $"{data.main.humidity}% Humidity";
                  z.Text= $"Wind : {data.wind.speed} m/s";
                y.Text = $"Pressure : {data.main.pressure} ";
                double f = data.wind.speed;
                /*   string toTextualDescription(var f)
                   {
                       if (f > 337.5) return "Northerly";
                       if (f > 292.5) return "North Westerly";
                       if (f > 247.5) return "Westerly";
                       if (f > 202.5) return "South Westerly";
                       if (f > 157.5) return "Southerly";
                       if (f > 122.5) return "South Easterly";
                       if (f > 67.5) return "Easterly";
                       if (f > 22.5) { return "North Easterly"; }
                       return "Northerly";
                   }   */
                // hi5.Text = $"{OpenWeatherMapService.ConvertUnixTimeToDateTime(data.sys.sunrise).ToString("HH:mm~")}/ {OpenWeatherMapService.ConvertUnixTimeToDateTime(data.sys.sunset).ToString("HH:mm")}";
                hi5.Text = now.ToString();

                txtCel.Text = $"{data.main.temp} °C";

                  var weatherText = "The current temperature in {0} is {1}°C, Humidity percent is about {2} , Winds running at {3} meter per second, its mostly {4},!";
                  string weatherMessage = string.Format(weatherText, data.name, (int)data.main.temp, (int)data.main.humidity,(double)data.wind.speed, data.weather[0].description);
                  ReadText(weatherMessage);

              }
              prgActivity.IsActive = false;
          }

        public void clear()
        {
            lblTempHigh.Text = "";
            lblTempLow.Text = "";
            lblTemp.Text = "";
            hi.Text = "";
            hi1.Text = "";
            hi2.Text = "";
            hi3.Text = "";
            txtCel.Text = "";
            txtDescription.Text = "";
            z.Text = "";
            y.Text = "";
            txtHumidity.Text = "";
            hi5.Text = "";
            txtCity.Text = "";
        }

        private void myclick(object sender, RoutedEventArgs e)
        {
            clear();
            string loc = txtLocation2.Text.Trim();
            string location = txtLocation.Text.Trim();
            ShowWeatherx(location);
            ShowWeathery(loc);
        }

        public async void geoclick(object sender, RoutedEventArgs e)
        {
            var x = await random.Randomf();
            if (x != null)
            {
                fact.Text = string.Format(x.text);
            }





            prgActivity.IsActive = true;
           

            Lat = la.Text.ToString();
            Lng = lo.Text.ToString();

            var data = await OpenWeatherMapService.GetWeather2(Lat, Lng);


            if (data != null)
            {
                txtCity.Text = $"{data.name},{data.sys.country}";
                // txtLastUpdate.Text = $"Last updated : {DateTime.Now.ToString("dd MMMM yyyy HH:mm")}";

                BitmapImage image = new BitmapImage(new Uri($"http://api.wunderground.com/api/9270a990901ad2ce/animatedradar/animatedsatellite/image.gif?num=5&delay=50&rad.maxlat=47.709&rad.maxlon=-69.263&rad.minlat=31.596&rad.minlon=-97.388&rad.width=640&rad.height=480&rad.rainsnow=1&rad.reproj.automerc=1&rad.num=5&sat.maxlat=47.709&sat.maxlon=-69.263&sat.minlat=31.596&sat.minlon=-97.388&sat.width=640&sat.height=480&sat.key=sat_ir4_bottom&sat.gtt=107&sat.proj=me&sat.timelabel=0&sat.num=5", UriKind.Absolute));
                //.hello.Source = image;

                txtDescription.Text = $"{data.weather[0].description}";
                txtHumidity.Text = $"{data.main.humidity}% Humidity";
                z.Text = $"Wind : {data.wind.speed} m/s";
                y.Text = $"Pressure : {data.main.pressure} ";
                double f = data.wind.speed;
                /*   string toTextualDescription(var f)
                   {
                       if (f > 337.5) return "Northerly";
                       if (f > 292.5) return "North Westerly";
                       if (f > 247.5) return "Westerly";
                       if (f > 202.5) return "South Westerly";
                       if (f > 157.5) return "Southerly";
                       if (f > 122.5) return "South Easterly";
                       if (f > 67.5) return "Easterly";
                       if (f > 22.5) { return "North Easterly"; }
                       return "Northerly";
                   }   */
                hi5.Text = $"{OpenWeatherMapService.ConvertUnixTimeToDateTime(data.sys.sunrise).ToString("HH:mm~")}/ {OpenWeatherMapService.ConvertUnixTimeToDateTime(data.sys.sunset).ToString("HH:mm")}";

                txtCel.Text = $"{data.main.temp} °C";

                var weatherText = "The current temperature in {0} is {1}°C, Humidity percent is about {2} , Winds running at {3} meter per second, its mostly {4},!";
                string weatherMessage = string.Format(weatherText, data.name, (int)data.main.temp, (int)data.main.humidity, (double)data.wind.speed, data.weather[0].description);
                ReadText(weatherMessage);

            }
            prgActivity.IsActive = false;
        }



    }
}

     





