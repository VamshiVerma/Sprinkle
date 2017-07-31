using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace WeatherTalk
{
    class forecasthelp
    {


        public static string API_LINK = "http://api.openweathermap.org/data/2.5/forecast";
        public static string API_KEY = "9f85dd96a0e5e49f4292e43c9795bddc";
        public static string APIRequest(string lat, string lon)
        {
            StringBuilder strBuilder = new StringBuilder(API_LINK);
            //units= metric to convert temp to Celsius
            strBuilder.AppendFormat("?lat={0}&lon={1}&APPID={2}&units=metric", lat, lon, API_KEY);
            return strBuilder.ToString();
        }



        public static async Task<RootObject> Getforecast(string lat, string lon)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(APIRequest(lat, lon));
                var resultText = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK) // 200 - OK
                {
                    try
                    {
                        var users = JsonConvert.DeserializeObject<RootObject>(resultText);
                        return users;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(resultText);
                        return null;
                    }
                }
                return null;
            }


        }
        public static async Task Populate(ObservableCollection<Forecast> fcast)
        {
            try
            {
                var geoLocator = new Geolocator();
                geoLocator.DesiredAccuracy = PositionAccuracy.High;
                Geoposition pos = await geoLocator.GetGeopositionAsync();

                var Lat = pos.Coordinate.Point.Position.Latitude.ToString();
                var Lng = pos.Coordinate.Point.Position.Longitude.ToString();



                var data = await forecasthelp.Getforecast(Lat, Lng);
                if (data != null)
                {
                    // txtCity.Text = $"{data.name},{data.sys.country}";
                    // txtLastUpdate.Text = $"Last updated : {DateTime.Now.ToString("dd MMMM yyyy HH:mm")}";

                    // BitmapImage image = new BitmapImage(new Uri($"http://openweathermap.org/img/w/{data.weather[0].icon}.png", UriKind.Absolute));
                    // imgWeather.Source = image;
                    //const double AbsoluteZero = -273.15;
                    foreach (var forecast in data.list)
                    {

                        fcast.Add(forecast);

                    }


                }
            }
            catch (Exception)
            {
                return;
            }
        }




    }




    }


