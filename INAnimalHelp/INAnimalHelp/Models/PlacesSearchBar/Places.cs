using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace INAnimalHelp.Models.PlacesSearchBar
{
    public static class Places
    {
        /// <summary>
        /// Возвращает место.
        /// </summary>
        /// <returns>Место.</returns>
        /// <param name="placeID">Идентификатор места.</param>
        /// <param name="apiKey">API key.</param>
        /// <param name="fields">поля места</param>
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
        /// Создает запрос URI для получения места.
        /// </summary>
        /// <returns>Запрос с деталями.</returns>
        /// <param name="place_id">Идентификатор места.</param>
        /// <param name="apiKey">API key.</param>
        /// <param name="fields">поля места</param>
        private static string CreateDetailsRequestUri(string place_id, string apiKey, PlacesFieldList fields)
        {
            var url = "https://maps.googleapis.com/maps/api/place/details/json";
            url += $"?placeid={Uri.EscapeUriString(place_id)}";
            url += $"&key={apiKey}";
            if (!fields.IsEmpty()) url += $"&fields={fields}";
            return url;
        }

        /// <summary>
        /// Обращается к Google Places API для получения информации о местах по запросу
        /// </summary>
        /// <returns>Места.</returns>
        /// <param name="newTextValue">Запрос пользователя.</param>
        /// <param name="apiKey">The API key</param>
        public static async Task<AutoCompleteResult> GetPlaces(string newTextValue, string apiKey)
        {
            try
            {
                var requestURI = CreatePredictionsUri(newTextValue, apiKey);
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
        /// Создает запрос URI для получения информации о местах по запросу.
        /// </summary>
        /// <returns>Запрос с деталями.</returns>
        /// <param name="newTextValue">Запрос пользователя.</param>
        /// <param name="apiKey">The API key</param>
        private static string CreatePredictionsUri(string newTextValue, string apiKey)
        {
            var url = "https://maps.googleapis.com/maps/api/place/autocomplete/json";
            var input = Uri.EscapeUriString(newTextValue);
            var pType = "address";

            var constructedUrl = $"{url}?input={input}&types={pType}&key={apiKey}";
            return constructedUrl;
        }
    }
}