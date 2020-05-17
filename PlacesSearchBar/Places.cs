using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PlacesSearchBar
{
	/// <summary>
	/// Places.
	/// </summary>
	public static class Places
	{
		/// <summary>
		/// Gets the place.
		/// </summary>
		/// <returns>The place.</returns>
		/// <param name="placeID">Place identifier.</param>
		/// <param name="apiKey">API key.</param>
		/// <param name="fields">The fields to query (see https://developers.google.com/places/web-service/details#fields )</param>
		public static async Task<Place> GetPlace(string placeID, string apiKey, PlacesFieldList fields = null)
		{
			fields = fields ?? PlacesFieldList.ALL; // default = ALL fields

			try
			{
				var requestURI = CreateDetailsRequestUri(placeID, apiKey, fields);
				var client = new HttpClient();
				var request = new HttpRequestMessage(HttpMethod.Get, requestURI);
				var response = await client.SendAsync(request);

				if (!response.IsSuccessStatusCode)
				{
					Debug.WriteLine("PlacesBar HTTP request denied.");
					return null;
				}

				var result = await response.Content.ReadAsStringAsync();

				if (result == "ERROR")
				{
					Debug.WriteLine("PlacesSearchBar Google Places API returned ERROR");
					return null;
				}

				return new Place(JObject.Parse(result));
			}
			catch (Exception ex)
			{
				Debug.WriteLine("PlacesBar HTTP issue: {0} {1}", ex.Message, ex);
				return null;
			}
		}

		/// <summary>
		/// Creates the details request URI.
		/// </summary>
		/// <returns>The details request URI.</returns>
		/// <param name="place_id">Place identifier.</param>
		/// <param name="apiKey">API key.</param>
		/// <param name="fields">The fields to query (see https://developers.google.com/places/web-service/details#fields )</param>
		private static string CreateDetailsRequestUri(string place_id, string apiKey, PlacesFieldList fields)
		{
			var url = "https://maps.googleapis.com/maps/api/place/details/json";
			url += $"?placeid={Uri.EscapeUriString(place_id)}";
			url += $"&key={apiKey}";
			if (!fields.IsEmpty()) url += $"&fields={fields}";
			return url;
		}

		/// <summary>
		/// Calls the Google Places API to retrieve autofill suggestions
		/// </summary>
		/// <returns>The places.</returns>
		/// <param name="newTextValue">New text value.</param>
		/// <param name="apiKey">The API key</param>
		/// <param name="bias">The location bias (can be NULL)</param>
		/// <param name="components">The components (can be NULL)</param>
		/// <param name="type">Filter for the returning types </param>
		/// <param name="language">The language of the results</param>
		public static async Task<AutoCompleteResult> GetPlaces(string newTextValue, string apiKey, LocationBias bias, Components components, PlaceType type, GoogleAPILanguage language)
		{
			if (string.IsNullOrEmpty(apiKey))
			{
				throw new Exception("You have not assigned a Google API key to PlacesBar");
			}

			try
			{
				var requestURI = CreatePredictionsUri(newTextValue, apiKey, bias, components, type, language);
				var client = new HttpClient();
				var request = new HttpRequestMessage(HttpMethod.Get, requestURI);
				var response = await client.SendAsync(request);

				if (!response.IsSuccessStatusCode)
				{
					Debug.WriteLine("PlacesBar HTTP request denied.");
					return null;
				}

				var result = await response.Content.ReadAsStringAsync();

				if (result == "ERROR")
				{
					Debug.WriteLine("PlacesSearchBar Google Places API returned ERROR");
					return null;
				}

				return AutoCompleteResult.FromJson(JObject.Parse(result));
			}
			catch (Exception ex)
			{
				Debug.WriteLine("PlacesBar HTTP issue: {0} {1}", ex.Message, ex);
				return null;
			}
		}

		/// <summary>
		/// Creates the predictions URI.
		/// </summary>
		/// <returns>The predictions URI.</returns>
		/// <param name="newTextValue">New text value.</param>
		/// <param name="apiKey">The API key</param>
		/// <param name="bias">The location bias (can be NULL)</param>
		/// <param name="components">The components (can be NULL)</param>
		/// <param name="type">Filter for the returning types </param>
		/// <param name="language">The language of the results</param>
		private static string CreatePredictionsUri(string newTextValue, string apiKey, LocationBias bias, Components components, PlaceType type, GoogleAPILanguage language)
		{
			var url = "https://maps.googleapis.com/maps/api/place/autocomplete/json";
			var input = Uri.EscapeUriString(newTextValue);
			var pType = PlaceTypeValue(type);

			var constructedUrl = $"{url}?input={input}&types={pType}&key={apiKey}";

			if (bias != null)
				constructedUrl = constructedUrl + bias;

			if (components != null)
				constructedUrl += components;

			if (language != GoogleAPILanguage.Unset)
				constructedUrl += "&Language=" + GoogleAPILanguageHelper.ToAPIString(language);

			return constructedUrl;
		}

		/// <summary>
		/// Returns a string representation of the specified place type.
		/// </summary>
		/// <returns>The type value.</returns>
		/// <param name="type">Type.</param>
		private static string PlaceTypeValue(PlaceType type)
		{
			switch (type)
			{
				case PlaceType.All:
					return "";
				case PlaceType.Geocode:
					return "geocode";
				case PlaceType.Address:
					return "address";
				case PlaceType.Establishment:
					return "establishment";
				case PlaceType.Regions:
					return "(regions)";
				case PlaceType.Cities:
					return "(cities)";
				default:
					return "";
			}
		}
	}
}