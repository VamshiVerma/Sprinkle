using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace WeatherTalk
{
   
    class OpenWeatherMapService
    {
        // TO DO: Get your own API key from http://openweathermap.org, don't use mine
        private const string APIKey = "b843f18aa40c5e8ad3471058123600f3";
        // URI used to get basic weather data. The API key is optional but your request may get denied
        // if you do not include one.
        private const string APIUrl = "http://api.openweathermap.org/data/2.5/weather?q={0}&units=metric&APPID=" + APIKey;
        private const string APIUrl2 = "http://api.openweathermap.org/data/2.5/forecast?q={0}&units=metric&APPID=" + APIKey;
        private const string APIUrl3 = " http://api.openweathermap.org/data/2.5/weather?q={0},{1}units=metric&APPID=" + APIKey;
        public static string API_LINK = "http://api.openweathermap.org/data/2.5/weather";
        public static string API_KEY = "9f85dd96a0e5e49f4292e43c9795bddc";

        /// <summary>
        /// Method that returns a WeatherRoot data object for a specific location from OpenWeatherMap.org
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<WeatherRoot> GetWeather(string location)
        {
            var client = new HttpClient();
            var url = string.Format(APIUrl, location);
            var json = await client.GetStringAsync(url);

            if (string.IsNullOrWhiteSpace(json))
                return null;

            // Deserialize the JSON results into a WeatherRoot object using JSON.NET
            return JsonConvert.DeserializeObject<WeatherRoot>(json);
        }
        public async Task<WeatherRoot> GetWeatherZ(string code,string country)
        {
            var client = new HttpClient();
            var url = string.Format(APIUrl, code,country);
            var json = await client.GetStringAsync(url);

            if (string.IsNullOrWhiteSpace(json))
                return null;

            // Deserialize the JSON results into a WeatherRoot object using JSON.NET
            return JsonConvert.DeserializeObject<WeatherRoot>(json);
        }

        public static string APIRequest(string lat, string lon)
        {
            StringBuilder strBuilder = new StringBuilder(API_LINK);
            //units= metric to convert temp to Celsius
            strBuilder.AppendFormat("?lat={0}&lon={1}&APPID={2}&units=metric", lat, lon, API_KEY);
            return strBuilder.ToString();
        }

        public static async Task<WeatherRoot> GetWeather2(string lat, string lon)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(APIRequest(lat, lon));
                var resultText = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK) // 200 - OK
                {
                    try
                    {
                        var users = JsonConvert.DeserializeObject<WeatherRoot>(resultText);
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
        public static async Task<RootObject> GetWeather3(string lat, string lon)
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
        public static DateTime ConvertUnixTimeToDateTime(double unix)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dt = dt.AddSeconds(unix).ToLocalTime();
            return dt;
        }

    }
}
