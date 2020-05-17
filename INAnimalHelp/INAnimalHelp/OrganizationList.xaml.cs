using INAnimalHelp.Models;
using INAnimalHelp.Models.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace INAnimalHelp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrganizationList : ContentPage
    {
        //модель для обновления информации
        readonly RefreshOrganizationListModel model;
        bool isbegining;
        /// <summary>
        /// конструктор для общего списка организаций
        /// </summary>
        /// <param name="organization">тип организации</param>
        /// <param name="animal">тип животных организации</param>
        public OrganizationList(string organization, string animal)
        {
            InitializeComponent();

            BindingContext = model = new RefreshOrganizationListModel() { animal = animal, organization = organization };

        }
        /// <summary>
        /// Конструктор для списка организаций пользователя
        /// </summary>
        public OrganizationList()
        {
            InitializeComponent();
            BindingContext = model = new RefreshOrganizationListModel() { isUser = true };
        }

        public OrganizationList(bool isbegining)
        {
            this.isbegining = isbegining;
            InitializeComponent();
            //кнопки карты и фильтра
            ToolbarItem map = new ToolbarItem()
            {
                IconImageSource = "map.png"
            };
            map.Clicked += async (s, e) =>
            {
                await Navigation.PushAsync(new Map(null, null));
            };
            ToolbarItem f = new ToolbarItem()
            {
                IconImageSource = "filter.png"
            };
            f.Clicked += async (s, e) =>
            {
                await Navigation.PushAsync(new Filter());
            };

            ToolbarItems.Add(map);
            ToolbarItems.Add(f);

            BindingContext = model = new RefreshOrganizationListModel() { animal = null, organization = null };
        }
        /// <summary>
        /// Переход на страницу организации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ToOrganizationPage(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new OrganizationPage(await App.Database.GetOrganizationAsync(((Organization)list.SelectedItem).Id)));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            List<Organization> orgs;
            //организации пользователя
            if (model.isUser)
            {
                orgs = await App.Database.GetUserOrganizations();
            }
            else if (isbegining)
            {
                if (Connectivity.NetworkAccess != NetworkAccess.None)
                {
                    orgs = await App.ASPDatabase.GetOrganizations();
                }
                else
                {
                    DependencyService.Get<IToast>().Show("Internet connection is unavailable. The page can't be updated at this time.");
                    orgs = await App.Database.GetOrganizationsAsync();
                }
            }
            else
            {
                //организации, соответствующие параметрам
                orgs = await App.Database.GetOrganizationsAsync();

                if (model.animal == null && model.organization != null)
                {
                    orgs = orgs.Where(item => item.OrganizationType == model.organization).ToList();
                }

                else if (model.organization == null && model.animal != null)
                {
                    orgs = orgs.Where(item => item.AnimalType == model.animal).ToList();

                }
                else if (model.animal != null && model.organization != null)
                {
                    orgs = orgs.Where((item) => item.AnimalType == model.animal && item.OrganizationType == model.organization).ToList();
                }
            }

            orgs.ForEach(o => o.ImageUrl = o.ImageId == 0 ? o.ImageUrl : Path.Combine(App.ASPDatabase.storage, o.ImageUrl));
            list.ItemsSource = model.Organizations = new ObservableCollection<Organization>(orgs);

        }
    }
}