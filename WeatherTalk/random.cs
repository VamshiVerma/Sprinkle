using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeatherTalk
{
    class random
    {
        public const string a = "http://numbersapi.com/random/year?json";
        public static async Task<numjson> Randomf()
        {
            var client = new HttpClient();
            var json = await client.GetStringAsync(a);
            if (string.IsNullOrWhiteSpace(json))
                return null;
            return JsonConvert.DeserializeObject<numjson>(json);
        }
    }
}
