using Newtonsoft.Json.Linq;

namespace INAnimalHelp.Models.PlacesSearchBar
{
    public class Place
    {
        public string ID { get; set; }

        public string Place_ID { get; set; }

        public string Name { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string FormattedAddress { get; set; }

        public Place(JObject jsonObject)
        {
            ID = jsonObject["result"]["id"]?.Value<string>();
            Place_ID = jsonObject["result"]["place_id"]?.Value<string>() ?? string.Empty;
            Name = jsonObject["result"]["name"]?.Value<string>() ?? string.Empty;
            Latitude = jsonObject["result"]?["geometry"]?["location"]?["lat"]?.Value<double>();
            Longitude = jsonObject["result"]?["geometry"]?["location"]?["lng"]?.Value<double>();
            FormattedAddress = jsonObject["result"]["formatted_address"]?.Value<string>() ?? string.Empty;
        }
    }
}