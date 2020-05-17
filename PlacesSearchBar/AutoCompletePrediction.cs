using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PlacesSearchBar
{
	/// <summary>
	/// Auto complete prediction.
	/// </summary>
	public class AutoCompletePrediction
	{
		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public string ID { get; set; }

		/// <summary>
		/// Gets or sets the place identifier.
		/// </summary>
		/// <value>The place identifier.</value>
		public string Place_ID { get; set; }

		/// <summary>
		/// Gets or sets the reference.
		/// </summary>
		/// <value>The reference.</value>
		public string Reference { get; set; }

		/// <summary>
		/// Gets the main text for UI display
		/// </summary>
		/// <value>The main text.</value>
		public string MainText { get; set; }

		/// <summary>
		/// Gets the secondary text for UI display
		/// </summary>
		/// <value>The secondary text.</value>
		public string SecondaryText { get; set; }

		/// <summary>
		/// Gets the individual terms of this prediction
		/// </summary>
		/// <value>The terms.</value>
		public List<string> Terms { get; set; }

		/// <summary>
		/// Gets the types of this prediction
		/// see https://developers.google.com/places/web-service/supported_types
		/// </summary>
		/// <value>The types of the prediction.</value>
		public List<string> Types { get; set; }

		public static AutoCompletePrediction FromJson(JObject json)
		{
			var r = new AutoCompletePrediction
			{
				Description = json["description"].Value<string>(),
				ID = json["id"].Value<string>(),
				Place_ID = json["place_id"].Value<string>(),
				Reference = json["reference"].Value<string>(),
				MainText = json["structured_formatting"]["main_text"].Value<string>(),
				SecondaryText = json["structured_formatting"]["secondary_text"].Value<string>(),
				Terms = json["terms"].Value<JArray>().Select(p => p["value"].Value<string>()).ToList(),
				Types = json["types"].Value<JArray>().Select(p => p.Value<string>()).ToList()
			};

			return r;
		}
	}

}