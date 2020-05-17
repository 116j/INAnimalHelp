using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlacesSearchBar
{
	/// <summary>
	/// Auto complete result.
	/// </summary>
	//This should really be renamed to AutoCompleteResultEventArgs but did not want to introduce breaking change.
	public class AutoCompleteResult : EventArgs
	{
		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>The status.</value>
		[JsonProperty("status")]
		public string Status { get; set; }

		/// <summary>
		/// Gets or sets the auto complete places.
		/// </summary>
		/// <value>The auto complete places.</value>
		[JsonProperty("predictions")]
		public List<AutoCompletePrediction> AutoCompletePlaces { get; set; }

		public static AutoCompleteResult FromJson(JObject result)
		{
			var r = new AutoCompleteResult();

			r.Status = result["status"].Value<string>();

			r.AutoCompletePlaces = new List<AutoCompletePrediction>();
			foreach (var obj in result["predictions"].Value<JArray>())
			{
				r.AutoCompletePlaces.Add(AutoCompletePrediction.FromJson(obj.Value<JObject>()));
			}

			return r;
		}
	}

}
