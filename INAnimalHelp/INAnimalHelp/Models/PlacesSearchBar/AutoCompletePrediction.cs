using Newtonsoft.Json.Linq;

namespace INAnimalHelp.Models.PlacesSearchBar
{
    public class AutoCompletePrediction
    {
        public string ID { get; set; }

        public string Place_ID { get; set; }

        public string MainText { get; set; }

        public string SecondaryText { get; set; }


        public static AutoCompletePrediction FromJson(JObject json)
        {
            var r = new AutoCompletePrediction
            {
                ID = json["id"].Value<string>(),
                Place_ID = json["place_id"].Value<string>(),
                MainText = json["structured_formatting"]["main_text"].Value<string>(),
                SecondaryText = json["structured_formatting"]["secondary_text"].Value<string>(),
            };

            return r;
        }
    }
}