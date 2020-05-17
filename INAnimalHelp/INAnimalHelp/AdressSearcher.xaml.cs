using INAnimalHelp.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INAnimalHelp.Models.PlacesSearchBar;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace INAnimalHelp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdressSearcher : ContentPage
    {
		//ключ доступа к API
		private const string GooglePlacesApiKey = "GooglePlacesApiKey";
		readonly Event ev;
		readonly bool settings;
		readonly Organization organization;
		/// <summary>
		/// Конструктор для организации
		/// </summary>
		/// <param name="isSetting">настройки или создание</param>
		/// <param name="org">организация</param>
		public AdressSearcher(bool isSetting, Organization org)
		{
			InitializeComponent();

			organization = org;
			settings = isSetting;
			search_bar.ApiKey = GooglePlacesApiKey;
			search_bar.PlacesRetrieved += Search_Bar_PlacesRetrieved;
			search_bar.TextChanged += Search_Bar_TextChanged;
			search_bar.MinimumSearchText = 2;
			results_list.ItemSelected += Results_List_ItemSelected;
		}
		/// <summary>
		/// Конструктор для мероприятия
		/// </summary>
		/// <param name="ev">мероприятие</param>
		public AdressSearcher(Event ev)
		{
			InitializeComponent();

			this.ev = ev;
			search_bar.ApiKey = GooglePlacesApiKey;
			search_bar.PlacesRetrieved += Search_Bar_PlacesRetrieved;
			search_bar.TextChanged += Search_Bar_TextChanged;
			search_bar.MinimumSearchText = 2;
			results_list.ItemSelected += Results_List_ItemSelected;
		}
		/// <summary>
		/// Установка полученного списка мест
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="result">переменная со списком мест</param>
		void Search_Bar_PlacesRetrieved(object sender, AutoCompleteResult result)
		{
			results_list.ItemsSource = result.AutoCompletePlaces;
			spinner.IsRunning = false;
			spinner.IsVisible = false;

			if (result.AutoCompletePlaces != null && result.AutoCompletePlaces.Count > 0)
				results_list.IsVisible = true;
		}
		/// <summary>
		/// Проверка ввода
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Search_Bar_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.NewTextValue))
			{
				results_list.IsVisible = false;
				spinner.IsVisible = true;
				spinner.IsRunning = true;
			}
			else
			{
				results_list.IsVisible = true;
				spinner.IsRunning = false;
				spinner.IsVisible = false;
			}
		}
		/// <summary>
		/// Выбор места
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		async void Results_List_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
				return;
			//выбранный элемент
			var prediction = (AutoCompletePrediction)e.SelectedItem;
			results_list.SelectedItem = null;
			//получение места
			var place = await Places.GetPlace(prediction.Place_ID, GooglePlacesApiKey);

			if (place != null)
			{
				if (ev != null)
				{
					//адрес для мероприятия
					ev.Adress = place.Name;
					ev.Lattitude = place.Latitude.ToString();
					ev.Longitude = place.Longitude.ToString();
					ev.AdressSet = true;
					await Navigation.PopAsync();
				}
				else
				{
					//адрес для организации
					App.CurrentOrganization.Adress = place.Name;
					organization.Adress = place.Name;
					organization.Longitude = place.Longitude.ToString();
					organization.Lattitude = place.Latitude.ToString();
					App.CurrentOrganization.Lattitude = place.Latitude.ToString();
					App.CurrentOrganization.Longitude = place.Longitude.ToString();

					await Navigation.PushModalAsync(new OrganizationCreation(settings, true, organization));
				}
			}
		}
	}
}