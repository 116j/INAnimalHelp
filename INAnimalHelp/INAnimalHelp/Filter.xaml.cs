using INAnimalHelp.Models.Models;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace INAnimalHelp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Filter : ContentPage
    {
        string animal;
        string organization;
        List<Organization> organizations;
        readonly string[] orgPicker = { "Shelter", "Team", "Fund" };
        readonly string[] anPicker = { "Cats", "Dogs", "Horses", "Rodents", "Reptiles", "Farm animals", "Wild animals" };

        public Filter()
        {
            InitializeComponent();
            //установка контента пикеров
            organizationPicker.ItemsSource = orgPicker;

            animalPicker.ItemsSource = anPicker;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //все организации
            organizations = await App.Database.GetOrganizationsAsync();
        }
        /// <summary>
        /// Выбор типа организации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Picker_SelectedOrganization(object sender, EventArgs e)
        {
            organization = organizationPicker.SelectedItem.ToString();
        }
        /// <summary>
        /// Выбор типа животных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Picker_SelectedAnimal(object sender, EventArgs e)
        {
            animal = animalPicker.SelectedItem.ToString();
        }
        /// <summary>
        /// Поиск организации по имени
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchOrganization(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue == "" || e.NewTextValue == null)
            {
                searchResults.IsVisible = false;
                return;
            }
            searchResults.IsVisible = true;

            string normalizedQuery = e.NewTextValue?.ToLower() ?? "";
            searchResults.ItemsSource = organizations.FindAll(f => f.Name.ToLowerInvariant().Contains(normalizedQuery));
        }
        /// <summary>
        /// Переход на страницу организации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GoToOrganization(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new OrganizationPage(searchResults.SelectedItem as Organization));
            searchResults.SelectedItem = null;
        }
        /// <summary>
        /// Переход на страницу со списком организаций, соответсвующих заданным значениям
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ToList(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OrganizationList(organization, animal));
        }
        /// <summary>
        /// Переход на карту с отмеченными рорганизациями, соответсвующими заданным значениям
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ToMap(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Map(organization, animal));
        }
    }
}