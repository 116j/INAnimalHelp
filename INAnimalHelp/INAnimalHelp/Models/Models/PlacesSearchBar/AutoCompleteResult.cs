using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace INAnimalHelp.Models.PlacesSearchBar
{
    public class AutoCompleteResult : EventArgs
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("predictions")]
        public List<AutoCompletePrediction> AutoCompletePlaces { get; set; }

        public static AutoCompleteResult FromJson(JObject result)
        {
            var r = new AutoCompleteResult
            {
                Status = result["status"].Value<string>(),

                AutoCompletePlaces = new List<AutoCompletePrediction>()
            };
            foreach (var obj in result["predictions"].Value<JArray>())
            {
                r.AutoCompletePlaces.Add(AutoCompletePrediction.FromJson(obj.Value<JObject>()));
            }

            return r;
        }
    }
}
