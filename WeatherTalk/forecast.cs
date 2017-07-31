using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherTalk
{
    public class MainDetail
    {
        public double temp { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public double pressure { get; set; }
        public double sea_level { get; set; }
        public double grnd_level { get; set; }
        public int humidity { get; set; }
        public double temp_kf { get; set; }
    }

    public class Weather1
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Clouds1
    {
        public int all { get; set; }
    }

    public class Wind1
    {
        public double speed { get; set; }
        public double deg { get; set; }
    }

    public class Rain
    {
        public double rain { get; set; }
    }

    public class Sys1
    {
        public string pod { get; set; }
    }

    public class Forecast
    {
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime dt { get; set; }
        public MainDetail main { get; set; }
        public List<Weather1> weather { get; set; }
        public Clouds1 clouds { get; set; }
        public Wind1 wind { get; set; }
        public Rain rain { get; set; }
        public Sys1 sys { get; set; }
        public string dt_txt { get; set; }
    }

    public class Coord1
    {
        public double lat { get; set; }
        public double lon { get; set; }
    }

    public class City
    {
        public int id { get; set; }
        public string name { get; set; }
        public Coord1 coord { get; set; }
        public string country { get; set; }
    }

    public class RootObject
    {
        public string cod { get; set; }
        public double message { get; set; }
        public int cnt { get; set; }
        public List<Forecast> list { get; set; }
        public City city { get; set; }
    }

}
public class UnixTimestampConverter : Newtonsoft.Json.JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTime);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            .AddSeconds((long)reader.Value);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}